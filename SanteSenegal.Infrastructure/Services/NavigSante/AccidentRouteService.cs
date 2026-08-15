using Microsoft.EntityFrameworkCore;
using SanteSenegal.Application.DTOs.NavigSante;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Infrastructure.Services.NavigSante;

public interface IAccidentRouteService
{
    Task<AccidentRoute> SignalerAccidentAsync(SignalerAccidentRequest request);
    Task<AccidentRoute?> GetByIdAsync(int id);
    Task<List<AccidentResponse>> GetAccidentsActifsAsync();
    Task<List<AccidentResponse>> GetAccidentsParZoneAsync(double lat, double lng, double rayonKm);
    Task<AccidentRoute?> MettreAJourStatutAsync(int id, MettreAJourStatutAccidentRequest request);
    Task<List<ZoneNoireDto>> GetZonesNoiresAsync();
    Task<StatistiquesAccidentDto> GetStatistiquesAsync(DateTime? dateDebut, DateTime? dateFin);
    Task<List<AccidentResponse>> GetHistoriqueAsync(int page, int pageSize);
    Task<List<AccidentResponse>> GetAccidentsAujourdhuiAsync();
}

public class AccidentRouteService : IAccidentRouteService
{
    private readonly SanteDbContext _context;

    public AccidentRouteService(SanteDbContext context)
    {
        _context = context;
    }

    public async Task<AccidentRoute> SignalerAccidentAsync(SignalerAccidentRequest request)
    {
        var accident = new AccidentRoute
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            AdresseApproximative = request.AdresseApproximative,
            Route = request.Route,
            PointKilometrique = request.PointKilometrique,
            Description = request.Description,
            NombreVictimesEstime = request.NombreVictimesEstime,
            TypeAccident = request.TypeAccident,
            TypeVehicule = request.TypeVehicule,
            RisqueIncendie = request.RisqueIncendie,
            FuiteCarburant = request.FuiteCarburant,
            RouteBloquee = request.RouteBloquee,
            ConditionsMeteo = request.ConditionsMeteo,
            NomSignaleur = request.NomSignaleur,
            TelephoneSignaleur = request.TelephoneSignaleur,
            PhotoUrl = request.PhotoUrl,
            DateAccident = DateTime.UtcNow,
            Statut = StatutAccident.Signale
        };

        await _context.AccidentsRoute.AddAsync(accident);
        await _context.SaveChangesAsync();

        return accident;
    }

    public async Task<AccidentRoute?> GetByIdAsync(int id)
        => await _context.AccidentsRoute.FindAsync(id);

    public async Task<List<AccidentResponse>> GetAccidentsActifsAsync()
    {
        var actifs = await _context.AccidentsRoute
            .Where(a => a.Statut != StatutAccident.Cloture)
            .OrderByDescending(a => a.DateAccident)
            .Take(50)
            .ToListAsync();

        return actifs.Select(MapToResponse).ToList();
    }

    public async Task<List<AccidentResponse>> GetAccidentsParZoneAsync(double lat, double lng, double rayonKm)
    {
        // Formule simplifiée de distance (approximation)
        var accidents = await _context.AccidentsRoute
            .Where(a => a.Statut != StatutAccident.Cloture)
            .ToListAsync();

        return accidents
            .Where(a => CalculerDistance(lat, lng, a.Latitude, a.Longitude) <= rayonKm)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AccidentRoute?> MettreAJourStatutAsync(int id, MettreAJourStatutAccidentRequest request)
    {
        var accident = await _context.AccidentsRoute.FindAsync(id);
        if (accident == null) return null;

        accident.Statut = request.NouveauStatut;
        accident.StructureIdIntervention = request.StructureIdIntervention;
        accident.NotesIntervention = request.NotesIntervention;

        if (request.NouveauStatut == StatutAccident.PriseEnCharge)
            accident.DatePriseEnCharge = DateTime.UtcNow;

        if (request.NouveauStatut == StatutAccident.Cloture)
            accident.DateCloture = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return accident;
    }

    public async Task<List<ZoneNoireDto>> GetZonesNoiresAsync()
    {
        var douzeMois = DateTime.UtcNow.AddMonths(-12);
        
        var zones = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= douzeMois)
            .GroupBy(a => new { a.Route, a.PointKilometrique, a.Latitude, a.Longitude })
            .Select(g => new ZoneNoireDto(
                g.Key.Route ?? "Route inconnue",
                g.Key.PointKilometrique,
                g.Key.Latitude,
                g.Key.Longitude,
                g.Count(),
                g.Sum(a => a.NombreDeces),
                g.Sum(a => a.NombreBlessesGraves),
                CalculerNiveauRisque(g.Count(), g.Sum(a => a.NombreDeces))
            ))
            .Where(z => z.NombreAccidents >= 3)
            .OrderByDescending(z => z.NombreAccidents)
            .Take(50)
            .ToListAsync();

        return zones;
    }

    public async Task<StatistiquesAccidentDto> GetStatistiquesAsync(DateTime? dateDebut, DateTime? dateFin)
    {
        var debut = dateDebut ?? DateTime.UtcNow.AddYears(-1);
        var fin = dateFin ?? DateTime.UtcNow;
        
        var query = _context.AccidentsRoute
            .Where(a => a.DateAccident >= debut && a.DateAccident <= fin);

        var total = await query.CountAsync();
        var deces = await query.SumAsync(a => a.NombreDeces);
        var blesses = await query.SumAsync(a => a.NombreBlessesGraves);
        
        var aujourdhui = await _context.AccidentsRoute
            .Where(a => a.DateAccident.Date == DateTime.UtcNow.Date)
            .CountAsync();
        
        var semaine = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= DateTime.UtcNow.AddDays(-7))
            .CountAsync();
        
        var mois = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= DateTime.UtcNow.AddMonths(-1))
            .CountAsync();

        var routeDangereuse = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= debut && a.DateAccident <= fin)
            .GroupBy(a => a.Route)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        var heureDangereuse = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= debut && a.DateAccident <= fin)
            .GroupBy(a => a.DateAccident.Hour)
            .OrderByDescending(g => g.Count())
            .Select(g => $"{g.Key:00}h-{g.Key + 1:00}h")
            .FirstOrDefaultAsync();

        var parRegion = await _context.AccidentsRoute
            .Where(a => a.DateAccident >= debut && a.DateAccident <= fin)
            .GroupBy(a => a.AdresseApproximative ?? "Inconnu")
            .Select(g => new AccidentParRegionDto(
                g.Key,
                g.Count(),
                g.Sum(a => a.NombreDeces)
            ))
            .OrderByDescending(r => r.NombreAccidents)
            .Take(10)
            .ToListAsync();

        return new StatistiquesAccidentDto(
            total,
            deces,
            blesses,
            total > 0 ? Math.Round((double)deces / total * 100, 1) : 0,
            aujourdhui,
            semaine,
            mois,
            routeDangereuse,
            heureDangereuse,
            parRegion
        );
    }

    public async Task<List<AccidentResponse>> GetHistoriqueAsync(int page, int pageSize)
    {
        var accidents = await _context.AccidentsRoute
            .OrderByDescending(a => a.DateAccident)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return accidents.Select(MapToResponse).ToList();
    }

    public async Task<List<AccidentResponse>> GetAccidentsAujourdhuiAsync()
    {
        var aujourdhui = DateTime.UtcNow.Date;
        var accidents = await _context.AccidentsRoute
            .Where(a => a.DateAccident.Date == aujourdhui)
            .OrderByDescending(a => a.DateAccident)
            .ToListAsync();

        return accidents.Select(MapToResponse).ToList();
    }

    // Helpers
    private static AccidentResponse MapToResponse(AccidentRoute a) => new(
        a.Id,
        a.Latitude,
        a.Longitude,
        a.AdresseApproximative,
        a.Route,
        a.DateAccident,
        a.NombreVictimesEstime,
        a.NombreBlessesGraves,
        a.NombreDeces,
        a.TypeAccident,
        a.TypeVehicule,
        a.RisqueIncendie,
        a.FuiteCarburant,
        a.RouteBloquee,
        a.ConditionsMeteo,
        a.Statut.ToString(),
        a.DatePriseEnCharge,
        a.DateCloture
    );

    private static double CalculerDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371; // Rayon de la Terre en km
        var dLat = ToRad(lat2 - lat1);
        var dLng = ToRad(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;

    private static string CalculerNiveauRisque(int accidents, int deces)
    {
        if (deces >= 5 || accidents >= 10) return "Rouge";
        if (deces >= 2 || accidents >= 5) return "Orange";
        return "Jaune";
    }
}
