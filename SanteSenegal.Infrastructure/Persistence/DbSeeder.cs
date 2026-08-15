using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(SanteDbContext context)
    {
        if (await context.Structures.AnyAsync())
            return; // Déjà seedé

        await SeedStructuresAsync(context);
        await SeedServicesAsync(context);
        await SeedSousServicesAsync(context);
        await SeedUsersAsync(context);
        await SeedPatientsAsync(context);
        await SeedDisponibilitesAsync(context);
        await SeedArticlesInfoSanteAsync(context);
        await SeedSymptomesAsync(context);
        await SeedRessourcesSanitairesAsync(context);
        await SeedAlertesEpidemiologiquesAsync(context);
    }

    private static async Task SeedStructuresAsync(SanteDbContext context)
    {
        var structures = new List<Structure>
        {
            new()
            {
                Nom = "Hôpital Principal de Dakar",
                Type = TypeStructure.Hopital,
                Adresse = "Avenue Nelson Mandela, Dakar-Plateau",
                Telephone = "+221 33 839 19 19",
                Email = "contact@hpd.sn",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Urgences, Chirurgie, Cardiologie, Pédiatrie, Gynécologie, Radiologie"
            },
            new()
            {
                Nom = "Centre Hospitalier National Fann",
                Type = TypeStructure.Hopital,
                Adresse = "Rue A. Assane Ndoye, Dakar",
                Telephone = "+221 33 869 50 50",
                Email = "fann@chu.sn",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Neurologie, Psychiatrie, Infectiologie, Médecine interne"
            },
            new()
            {
                Nom = "Hôpital Aristide Le Dantec",
                Type = TypeStructure.Hopital,
                Adresse = "Avenue Cheikh Anta Diop, Dakar",
                Telephone = "+221 33 889 38 00",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Pédiatrie, Oncologie, Hématologie, Chirurgie pédiatrique"
            },
            new()
            {
                Nom = "Centre de Santé Mermoz",
                Type = TypeStructure.CentreDeSante,
                Adresse = "Mermoz, Dakar",
                Telephone = "+221 33 820 12 34",
                EstActif = true,
                HorairesOuverture = "08h-20h",
                Description = "Médecine générale, Vaccination, Planning familial"
            },
            new()
            {
                Nom = "Clinique du Cap",
                Type = TypeStructure.Clinique,
                Adresse = "Route de l'Aéroport, Les Mamelles",
                Telephone = "+221 33 869 69 69",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Maternité, Chirurgie, Urgences, Cardiologie"
            },
            new()
            {
                Nom = "Hôpital Régional de Thiès",
                Type = TypeStructure.Hopital,
                Adresse = "Route de Saint-Louis, Thiès",
                Telephone = "+221 33 951 11 11",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Urgences, Chirurgie, Maternité, Pédiatrie"
            },
            new()
            {
                Nom = "Centre de Santé Thiès Est",
                Type = TypeStructure.CentreDeSante,
                Adresse = "Ndaraw, Thiès",
                Telephone = "+221 33 952 22 22",
                EstActif = true,
                HorairesOuverture = "08h-18h",
                Description = "Médecine générale, Dentaire, PMI"
            },
            new()
            {
                Nom = "Cabinet Médical Dakar Plateau",
                Type = TypeStructure.Cabinet,
                Adresse = "Avenue Lamine Guèye, Dakar",
                Telephone = "+221 33 821 23 45",
                EstActif = true,
                HorairesOuverture = "07h-23h",
                Description = "Médicaments, Conseil pharmaceutique, Tests rapides"
            },
            new()
            {
                Nom = "Poste de Santé Liberté 6",
                Type = TypeStructure.PosteDeSante,
                Adresse = "Liberté 6, Dakar",
                Telephone = "+221 33 839 92 00",
                EstActif = true,
                HorairesOuverture = "07h-19h",
                Description = "Analyses médicales, Biologie, Sérologie"
            },
            new()
            {
                Nom = "Clinique Les Maristes",
                Type = TypeStructure.Clinique,
                Adresse = "Liberté 6, Dakar",
                Telephone = "+221 33 860 20 20",
                EstActif = true,
                HorairesOuverture = "24h/24",
                Description = "Médecine générale, Chirurgie, Maternité"
            }
        };

        await context.Structures.AddRangeAsync(structures);
        await context.SaveChangesAsync();
    }

    private static async Task SeedServicesAsync(SanteDbContext context)
    {
        var services = new List<Service>
        {
            new() { Nom = "Urgences", Description = "Service d'urgences 24h/24", EstActif = true },
            new() { Nom = "Médecine Générale", Description = "Consultations générales", EstActif = true },
            new() { Nom = "Pédiatrie", Description = "Soins aux enfants", EstActif = true },
            new() { Nom = "Gynécologie-Obstétrique", Description = "Suivi grossesse et accouchement", EstActif = true },
            new() { Nom = "Cardiologie", Description = "Maladies du cœur", EstActif = true },
            new() { Nom = "Chirurgie", Description = "Interventions chirurgicales", EstActif = true },
            new() { Nom = "Dentaire", Description = "Soins dentaires", EstActif = true },
            new() { Nom = "Ophtalmologie", Description = "Soins des yeux", EstActif = true },
            new() { Nom = "Laboratoire", Description = "Analyses médicales", EstActif = true },
            new() { Nom = "Radiologie", Description = "Imagerie médicale", EstActif = true }
        };

        await context.Services.AddRangeAsync(services);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSousServicesAsync(SanteDbContext context)
    {
        var services = await context.Services.ToListAsync();

        var sousServices = new List<SousService>
        {
            new() { Nom = "Urgences polyvalentes", Prix = 5000, DureeMinutes = 30, Specialite = "Urgentiste", ServiceId = services[0].Id, EstDisponible = true },
            new() { Nom = "Consultation générale", Prix = 3000, DureeMinutes = 20, Specialite = "Médecin généraliste", ServiceId = services[1].Id, EstDisponible = true },
            new() { Nom = "Suivi grossesse", Prix = 5000, DureeMinutes = 30, Specialite = "Gynécologue", ServiceId = services[3].Id, EstDisponible = true },
            new() { Nom = "Accouchement", Prix = 50000, DureeMinutes = 180, Specialite = "Obstétricien", ServiceId = services[3].Id, EstDisponible = true },
            new() { Nom = "Consultation pédiatrique", Prix = 4000, DureeMinutes = 25, Specialite = "Pédiatre", ServiceId = services[2].Id, EstDisponible = true },
            new() { Nom = "Vaccination enfant", Prix = 0, DureeMinutes = 15, Specialite = "Pédiatre", ServiceId = services[2].Id, EstDisponible = true },
            new() { Nom = "Consultation cardiaque", Prix = 10000, DureeMinutes = 45, Specialite = "Cardiologue", ServiceId = services[4].Id, EstDisponible = true },
            new() { Nom = "Électrocardiogramme", Prix = 15000, DureeMinutes = 30, Specialite = "Cardiologue", ServiceId = services[4].Id, EstDisponible = true },
            new() { Nom = "Chirurgie digestive", Prix = 200000, DureeMinutes = 120, Specialite = "Chirurgien digestif", ServiceId = services[5].Id, EstDisponible = true },
            new() { Nom = "Soins dentaires", Prix = 8000, DureeMinutes = 30, Specialite = "Dentiste", ServiceId = services[6].Id, EstDisponible = true },
            new() { Nom = "Extraction dentaire", Prix = 5000, DureeMinutes = 20, Specialite = "Dentiste", ServiceId = services[6].Id, EstDisponible = true },
            new() { Nom = "Consultation ophtalmo", Prix = 8000, DureeMinutes = 30, Specialite = "Ophtalmologiste", ServiceId = services[7].Id, EstDisponible = true },
            new() { Nom = "Bilan sanguin complet", Prix = 12000, DureeMinutes = 15, Specialite = "Biologiste", ServiceId = services[8].Id, EstDisponible = true },
            new() { Nom = "Radiographie", Prix = 8000, DureeMinutes = 20, Specialite = "Radiologue", ServiceId = services[9].Id, EstDisponible = true },
            new() { Nom = "Échographie", Prix = 15000, DureeMinutes = 30, Specialite = "Radiologue", ServiceId = services[9].Id, EstDisponible = true }
        };

        await context.SousServices.AddRangeAsync(sousServices);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(SanteDbContext context)
    {
        var users = new List<User>
        {
            new()
            {
                Email = "admin@santesenegal.sn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026"),
                Nom = "Diallo",
                Prenom = "Mamadou",
                Telephone = "770000001",
                Role = UserRole.Admin,
                EstActif = true
            },
            new()
            {
                Email = "dr.ndiaye@hpd.sn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Medecin@2026"),
                Nom = "Ndiaye",
                Prenom = "Aminata",
                Telephone = "770000002",
                Role = UserRole.Medecin,
                EstActif = true
            },
            new()
            {
                Email = "dr.fall@fann.sn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Medecin@2026"),
                Nom = "Fall",
                Prenom = "Ousmane",
                Telephone = "770000003",
                Role = UserRole.Medecin,
                EstActif = true
            },
            new()
            {
                Email = "agent@thiessante.sn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent@2026"),
                Nom = "Sow",
                Prenom = "Fatou",
                Telephone = "770000004",
                Role = UserRole.Patient,
                EstActif = true
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPatientsAsync(SanteDbContext context)
    {
        var patients = new List<Patient>
        {
            new()
            {
                Nom = "Diallo",
                Prenom = "Ibrahima",
                DateNaissance = new DateTime(1985, 3, 15),
                Telephone = "771234567",
                Email = "ibrahima.diallo@email.sn",
                Adresse = "Médina, Dakar",
                GroupeSanguin = "O+",
                ContactUrgence = "Awa Diallo",
                TelephoneContactUrgence = "778765432"
            },
            new()
            {
                Nom = "Ndiaye",
                Prenom = "Mariama",
                DateNaissance = new DateTime(1990, 7, 22),
                Telephone = "772345678",
                Email = "mariama.ndiaye@email.sn",
                Adresse = "Ouakam, Dakar",
                GroupeSanguin = "A+",
                AntecedentsMedicaux = "Asthme, Hypertension"
            },
            new()
            {
                Nom = "Sow",
                Prenom = "Abdoulaye",
                DateNaissance = new DateTime(1978, 11, 5),
                Telephone = "773456789",
                Adresse = "Liberté 6, Dakar",
                GroupeSanguin = "B+",
                ContactUrgence = "Khadija Sow",
                TelephoneContactUrgence = "779876543"
            },
            new()
            {
                Nom = "Ba",
                Prenom = "Aminata",
                DateNaissance = new DateTime(1995, 1, 10),
                Telephone = "774567890",
                Email = "aminata.ba@email.sn",
                Adresse = "Thiès centre",
                GroupeSanguin = "AB+",
                AntecedentsMedicaux = "Diabète type 2"
            },
            new()
            {
                Nom = "Diop",
                Prenom = "Cheikh",
                DateNaissance = new DateTime(2000, 5, 18),
                Telephone = "775678901",
                Adresse = "Mermoz, Dakar",
                GroupeSanguin = "O-"
            },
            new()
            {
                Nom = "Fall",
                Prenom = "Kadiatou",
                DateNaissance = new DateTime(2015, 9, 3),
                Telephone = "776789012",
                Adresse = "Ngor, Dakar",
                GroupeSanguin = "A-",
                ContactUrgence = "Moussa Fall",
                TelephoneContactUrgence = "771112233"
            }
        };

        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDisponibilitesAsync(SanteDbContext context)
    {
        var sousServices = await context.SousServices.ToListAsync();
        var disponibilites = new List<Disponibilite>();

        foreach (var ss in sousServices)
        {
            for (int i = 1; i <= 7; i++)
            {
                var date = DateTime.Now.AddDays(i).Date;
                disponibilites.Add(new Disponibilite
                {
                    Date = date,
                    HeureDebut = new TimeSpan(8, 0, 0),
                    HeureFin = new TimeSpan(12, 0, 0),
                    EstDisponible = true,
                    SousServiceId = ss.Id
                });
                disponibilites.Add(new Disponibilite
                {
                    Date = date,
                    HeureDebut = new TimeSpan(14, 0, 0),
                    HeureFin = new TimeSpan(18, 0, 0),
                    EstDisponible = true,
                    SousServiceId = ss.Id
                });
            }
        }

        await context.Disponibilites.AddRangeAsync(disponibilites);
        await context.SaveChangesAsync();
    }

    private static async Task SeedArticlesInfoSanteAsync(SanteDbContext context)
    {
        var articles = new List<ArticleInfoSante>
        {
            new()
            {
                Titre = "Prévention du paludisme au Sénégal",
                Contenu = "Le paludisme reste une préoccupation majeure. Utilisez des moustiquaires imprégnées, éliminez les eaux stagnantes et consultez immédiatement en cas de fièvre.",
                Categorie = "Prévention",
                EstPublie = true,
                DatePublication = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Titre = "Vaccination des enfants : calendrier 2026",
                Contenu = "Le calendrier vaccinal sénégalais comprend le BCG, le pentavalent, le polio, la rougeole et le pneumocoque. Respectez les dates pour protéger votre enfant.",
                Categorie = "Vaccination",
                EstPublie = true,
                DatePublication = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Titre = "Gestion du diabète au quotidien",
                Contenu = "Surveillez votre glycémie régulièrement, suivez un régime équilibré, faites de l'exercice physique et prenez vos médicaments à heure fixe.",
                Categorie = "Chronique",
                EstPublie = true,
                DatePublication = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                Titre = "Hygiène alimentaire pendant la saison des pluies",
                Contenu = "Lavez-vous les mains avant de manger, consommez de l'eau potable, conservez les aliments au frais et évitez les vendeurs ambulants.",
                Categorie = "Hygiène",
                EstPublie = true,
                DatePublication = DateTime.UtcNow.AddDays(-2)
            }
        };

        await context.ArticlesInfoSante.AddRangeAsync(articles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSymptomesAsync(SanteDbContext context)
    {
        var symptomes = new List<Symptome>
        {
            new() { Nom = "Fièvre", NomWolof = "Sibbiru", Description = "Température corporelle élevée", Gravite = 2 },
            new() { Nom = "Maux de tête", NomWolof = "Bopp", Description = "Douleur à la tête", Gravite = 1 },
            new() { Nom = "Toux", NomWolof = "Saw", Description = "Toux sèche ou grasse", Gravite = 2 },
            new() { Nom = "Difficulté à respirer", NomWolof = "Dafa sedd", Description = "Essoufflement", Gravite = 4 },
            new() { Nom = "Douleur thoracique", NomWolof = "Bopp", Description = "Douleur à la poitrine", Gravite = 4 },
            new() { Nom = "Vomissements", NomWolof = "Dafa liggéey", Description = "Rejet d'aliments", Gravite = 2 },
            new() { Nom = "Diarrhée", NomWolof = "Dafa tukki", Description = "Selles liquides fréquentes", Gravite = 2 },
            new() { Nom = "Éruption cutanée", NomWolof = "Benn", Description = "Rougeurs sur la peau", Gravite = 1 },
            new() { Nom = "Fatigue", NomWolof = "Dafa sonn", Description = "Grande fatigue", Gravite = 1 },
            new() { Nom = "Saignement", NomWolof = "Dëj", Description = "Perte de sang", Gravite = 4 }
        };

        await context.Symptomes.AddRangeAsync(symptomes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRessourcesSanitairesAsync(SanteDbContext context)
    {
        var structures = await context.Structures.ToListAsync();
        var ressources = new List<RessourceSanitaire>
        {
            new()
            {
                StructureId = structures[0].Id,
                LitsTotal = 200,
                LitsDisponibles = 45,
                LitsReanimationTotal = 20,
                LitsReanimationDisponibles = 5,
                LitsMaterniteTotal = 30,
                LitsMaterniteDisponibles = 8,
                StockSangAPlus = 15, StockSangAMoins = 10,
                StockSangBPlus = 12, StockSangBMoins = 8,
                StockSangABPlus = 5, StockSangABMoins = 3,
                StockSangOPlus = 20, StockSangOMoins = 12,
                ScannerFonctionnel = true,
                RadioFonctionnel = true,
                LaboFonctionnel = true,
                BlocOperatoireFonctionnel = true,
                TempsAttenteUrgencesMinutes = 25
            },
            new()
            {
                StructureId = structures[4].Id,
                LitsTotal = 80,
                LitsDisponibles = 20,
                LitsReanimationTotal = 8,
                LitsReanimationDisponibles = 2,
                LitsMaterniteTotal = 15,
                LitsMaterniteDisponibles = 4,
                StockSangAPlus = 8, StockSangAMoins = 5,
                StockSangBPlus = 6, StockSangBMoins = 4,
                StockSangABPlus = 2, StockSangABMoins = 1,
                StockSangOPlus = 10, StockSangOMoins = 6,
                ScannerFonctionnel = true,
                RadioFonctionnel = true,
                LaboFonctionnel = true,
                BlocOperatoireFonctionnel = true,
                TempsAttenteUrgencesMinutes = 15
            },
            new()
            {
                StructureId = structures[5].Id,
                LitsTotal = 120,
                LitsDisponibles = 35,
                LitsReanimationTotal = 10,
                LitsReanimationDisponibles = 3,
                LitsMaterniteTotal = 20,
                LitsMaterniteDisponibles = 6,
                StockSangAPlus = 10, StockSangAMoins = 7,
                StockSangBPlus = 8, StockSangBMoins = 5,
                StockSangABPlus = 3, StockSangABMoins = 2,
                StockSangOPlus = 14, StockSangOMoins = 9,
                ScannerFonctionnel = true,
                RadioFonctionnel = true,
                LaboFonctionnel = true,
                BlocOperatoireFonctionnel = false,
                TempsAttenteUrgencesMinutes = 40
            }
        };

        await context.RessourcesSanitaires.AddRangeAsync(ressources);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAlertesEpidemiologiquesAsync(SanteDbContext context)
    {
        var alertes = new List<AlerteEpidemiologique>
        {
            new()
            {
                Titre = "Hausse des cas de dengue à Dakar",
                Description = "Augmentation des cas de dengue observée dans les communes de Dakar-Plateau et Médina. Éliminez les eaux stagnantes.",
                Maladie = "Dengue",
                NiveauAlerte = "Orange",
                Region = "Dakar",
                CasConfirmes = 45,
                CasSuspects = 120,
                DateDebut = DateTime.UtcNow.AddDays(-7),
                EstActive = true
            },
            new()
            {
                Titre = "Choléra : vigilance renforcée",
                Description = "Présence de cas de choléra dans la région de Matam. Respectez les règles d'hygiène et consultez en cas de diarrhée sévère.",
                Maladie = "Choléra",
                NiveauAlerte = "Rouge",
                Region = "Matam",
                CasConfirmes = 12,
                CasSuspects = 35,
                Deces = 2,
                DateDebut = DateTime.UtcNow.AddDays(-14),
                DateFin = DateTime.UtcNow.AddDays(-2),
                EstActive = false
            }
        };

        await context.AlertesEpidemiologiques.AddRangeAsync(alertes);
        await context.SaveChangesAsync();
    }
}
