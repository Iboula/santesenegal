using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Notifications;

public static class MessageTemplates
{
    // ─── RAPPEL RENDEZ-VOUS ───
    public static string RappelRendezVousFr(string nom, string structure, string date, string heure, string service) =>
        $"Bonjour {nom}, rappel: vous avez un rendez-vous le {date} a {heure} au {structure} ({service}). Merci de vous presenter 15min avant. SanteSenegal";

    public static string RappelRendezVousWo(string nom, string structure, string date, string heure, string service) =>
        $"Salaamalekum {nom}, dangay am beneen rendez-vous {date} ci {heure} ci {structure} ({service}). Nangu ko 15min yepp. SanteSenegal";

    // ─── CONFIRMATION RENDEZ-VOUS ───
    public static string ConfirmationRdvFr(string nom, string structure, string date, string heure, string reference) =>
        $"Bonjour {nom}, votre rendez-vous au {structure} est confirme pour le {date} a {heure}. Ref: {reference}. SanteSenegal";

    public static string ConfirmationRdvWo(string nom, string structure, string date, string heure, string reference) =>
        $"Salaamalekum {nom}, sa rendez-vous ci {structure} dafa ame {date} ci {heure}. Ref: {reference}. SanteSenegal";

    // ─── CONFIRMATION PAIEMENT ───
    public static string ConfirmationPaiementFr(string nom, decimal montant, string mode, string reference) =>
        $"Bonjour {nom}, votre paiement de {montant:F0} FCFA via {mode} a ete recu. Ref: {reference}. Merci! SanteSenegal";

    public static string ConfirmationPaiementWo(string nom, decimal montant, string mode, string reference) =>
        $"Salaamalekum {nom}, sa khalis {montant:F0} FCFA ci {mode} dafa ame. Ref: {reference}. Jerejef! SanteSenegal";

    // ─── ALERTE EPIDEMIOLOGIQUE ───
    public static string AlerteEpidemioFr(string titre, string conseils) =>
        $"ALERTE SANTE - {titre}. {conseils}. Pour plus d'infos appelez le 800 00 50 50. SanteSenegal";

    public static string AlerteEpidemioWo(string titre, string conseils) =>
        $"WARNATI SANTE - {titre}. {conseils}. Xamle bu bees: 800 00 50 50. SanteSenegal";

    // ─── ANNULATION RENDEZ-VOUS ───
    public static string AnnulationRdvFr(string nom, string date, string structure) =>
        $"Bonjour {nom}, votre rendez-vous du {date} au {structure} a ete annule. Pour reprogrammer: 800 00 50 50. SanteSenegal";

    public static string AnnulationRdvWo(string nom, string date, string structure) =>
        $"Salaamalekum {nom}, sa rendez-vous {date} ci {structure} dafa neex. Boo bëggee waxtu bu bees: 800 00 50 50. SanteSenegal";

    // ─── RESULTAT EXAMEN ───
    public static string ResultatExamenFr(string nom, string typeExamen, string structure) =>
        $"Bonjour {nom}, vos resultats {typeExamen} sont disponibles au {structure}. Veuillez vous presenter avec votre carte. SanteSenegal";

    // ─── RAPPEL VACCIN ───
    public static string RappelVaccinFr(string nomEnfant, string vaccin, string date) =>
        $"Rappel vaccin: {nomEnfant} doit recevoir le vaccin {vaccin} le {date}. N'oubliez pas le carnet! SanteSenegal";

    public static string RappelVaccinWo(string nomEnfant, string vaccin, string date) =>
        $"Waxtu vaccin: {nomEnfant} am na ko vaccin {vaccin} {date}. Bul may ko kart bi! SanteSenegal";

    // Helper
    public static string GetMessage(TypeNotification type, string langue, params object[] args)
    {
        var isWolof = langue == "wo";
        return type switch
        {
            TypeNotification.RappelRendezVous => isWolof
                ? RappelRendezVousWo((string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4])
                : RappelRendezVousFr((string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4]),
            TypeNotification.ConfirmationPaiement => isWolof
                ? ConfirmationPaiementWo((string)args[0], (decimal)args[1], (string)args[2], (string)args[3])
                : ConfirmationPaiementFr((string)args[0], (decimal)args[1], (string)args[2], (string)args[3]),
            TypeNotification.AlerteEpidemiologique => isWolof
                ? AlerteEpidemioWo((string)args[0], (string)args[1])
                : AlerteEpidemioFr((string)args[0], (string)args[1]),
            TypeNotification.ConfirmationRendezVous => isWolof
                ? ConfirmationRdvWo((string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4])
                : ConfirmationRdvFr((string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4]),
            TypeNotification.AnnulationRendezVous => isWolof
                ? AnnulationRdvWo((string)args[0], (string)args[1], (string)args[2])
                : AnnulationRdvFr((string)args[0], (string)args[1], (string)args[2]),
            TypeNotification.RappelVaccin => isWolof
                ? RappelVaccinWo((string)args[0], (string)args[1], (string)args[2])
                : RappelVaccinFr((string)args[0], (string)args[1], (string)args[2]),
            _ => throw new NotSupportedException($"Template non supporté: {type}")
        };
    }
}
