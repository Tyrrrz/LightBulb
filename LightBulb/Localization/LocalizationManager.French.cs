using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> FrenchLocalization = new Dictionary<
        string,
        string
    >
    {
        [nameof(SunsetLabel)] = "Coucher de soleil",
        [nameof(SunriseLabel)] = "Lever de soleil",
        [nameof(SunsetTransitionStartsAt)] = "La transition du coucher de soleil commence à",
        [nameof(SunriseTransitionStartsAt)] = "La transition du lever de soleil commence à",
        [nameof(AndEndsAt)] = "et se termine à",
        [nameof(OffsetTooltipHeader)] =
            "Les valeurs actuelles de température et de luminosité sont ajustées par un décalage :",
        [nameof(TemperatureOffsetLabel)] = "Décalage de température :",
        [nameof(BrightnessOffsetLabel)] = "Décalage de luminosité :",
        [nameof(ClickToResetLabel)] = "Cliquez pour réinitialiser",
        [nameof(OffsetLabel)] = "décalage",
        [nameof(ToggleLightBulbTooltip)] = "Activer/désactiver LightBulb",
        [nameof(HideToTrayTooltip)] = "Réduire LightBulb dans la barre des tâches",
        [nameof(PreviewText)] = "APERÇU",
        [nameof(StopPreviewTooltip)] = "Arrêter l'aperçu",
        [nameof(StartPreviewTooltip)] = "Aperçu du cycle de 24 heures",
        [nameof(SettingsText)] = "PARAMÈTRES",
        [nameof(OpenSettingsTooltip)] = "Ouvrir les paramètres",
        [nameof(AboutText)] = "GITHUB",
        [nameof(OpenGitHubTooltip)] = "Ouvrir LightBulb sur GitHub",
        [nameof(ResetButton)] = "RÉINITIALISER",
        [nameof(ResetTooltip)] = "Réinitialiser tous les paramètres à leurs valeurs par défaut",
        [nameof(CancelButton)] = "ANNULER",
        [nameof(SaveButton)] = "ENREGISTRER",
        [nameof(GeneralTabName)] = "Général",
        [nameof(LocationTabName)] = "Emplacement",
        [nameof(AdvancedTabName)] = "Avancé",
        [nameof(AppWhitelistTabName)] = "Liste blanche",
        [nameof(HotkeysTabName)] = "Raccourcis",
        [nameof(ThemeLabel)] = "Thème",
        [nameof(ThemeTooltip)] = "Thème d'interface préféré",
        [nameof(LanguageLabel)] = "Langue",
        [nameof(LanguageTooltip)] = "Langue d'interface préférée",
        [nameof(StartWithWindowsLabel)] = "Démarrer avec Windows",
        [nameof(StartWithWindowsTooltip)] = "Lancer LightBulb au démarrage de Windows",
        [nameof(AutoUpdateLabel)] = "Mise à jour automatique",
        [nameof(AutoUpdateTooltip)] =
            "Maintenir LightBulb à jour en installant automatiquement les nouvelles versions",
        [nameof(DefaultToDayConfigLabel)] = "Par défaut : configuration diurne",
        [nameof(DefaultToDayConfigTooltip)] =
            "Quand LightBulb est désactivé ou en pause, restaurer la température et la luminosité diurnes configurées au lieu du gamma par défaut du moniteur",
        [nameof(PauseWhenFullscreenLabel)] = "Pause en plein écran",
        [nameof(PauseWhenFullscreenTooltip)] =
            "Mettre LightBulb en pause quand une fenêtre plein écran est au premier plan",
        [nameof(GammaSmoothingLabel)] = "Lissage gamma",
        [nameof(GammaSmoothingTooltip)] =
            "Transition en douceur lors de l'activation ou désactivation de LightBulb pour laisser le temps aux yeux de s'adapter",
        [nameof(GammaPollingLabel)] = "Interrogation gamma",
        [nameof(GammaPollingTooltip)] =
            "Forcer le rafraîchissement du gamma du moniteur à intervalles réguliers pour empêcher d'autres programmes de le remplacer",
        [nameof(DayTemperatureLabel)] = "Température de couleur diurne :",
        [nameof(DayTemperatureTooltip)] = "Température de couleur pendant la journée",
        [nameof(NightTemperatureLabel)] = "Température de couleur nocturne :",
        [nameof(NightTemperatureTooltip)] = "Température de couleur pendant la nuit",
        [nameof(DayBrightnessLabel)] = "Luminosité diurne :",
        [nameof(DayBrightnessTooltip)] = """
            Luminosité pendant la journée

            Ce réglage de luminosité s'applique au gamma des couleurs, pas à la luminosité réelle du moniteur.
            Si votre ordinateur est déjà capable d'ajuster automatiquement la luminosité de l'écran en fonction des conditions d'éclairage (courant sur les ordinateurs portables), il est recommandé de désactiver le contrôle de luminosité de LightBulb en maintenant les deux réglages à 100%.
            """,
        [nameof(NightBrightnessLabel)] = "Luminosité nocturne :",
        [nameof(NightBrightnessTooltip)] = """
            Luminosité pendant la nuit

            Ce réglage de luminosité s'applique au gamma des couleurs, pas à la luminosité réelle du moniteur.
            Si votre ordinateur est déjà capable d'ajuster automatiquement la luminosité de l'écran en fonction des conditions d'éclairage (courant sur les ordinateurs portables), il est recommandé de désactiver le contrôle de luminosité de LightBulb en maintenant les deux réglages à 100%.
            """,
        [nameof(TransitionDurationLabel)] = "Durée de transition :",
        [nameof(TransitionDurationTooltip)] =
            "Durée nécessaire pour passer de la configuration diurne à nocturne",
        [nameof(TransitionOffsetLabel)] = "Décalage de transition :",
        [nameof(TransitionOffsetTooltip)] =
            "Décalage qui spécifie l'avance ou le retard du début de la transition par rapport au lever et coucher de soleil",
        [nameof(SolarConfigLabel)] = "Configuration solaire :",
        [nameof(ManualLabel)] = "Manuel",
        [nameof(ManualTooltip)] = "Configurer manuellement le lever et coucher de soleil",
        [nameof(LocationBasedLabel)] = "Basé sur la localisation",
        [nameof(LocationBasedTooltip)] =
            "Configurer votre emplacement pour calculer automatiquement les heures de lever et coucher de soleil",
        [nameof(SunriseTimeLabel)] = "Lever de soleil :",
        [nameof(SunsetTimeLabel)] = "Coucher de soleil :",
        [nameof(YourLocationLabel)] = "Votre emplacement :",
        [nameof(AutoDetectLocationTooltip)] =
            "Essayer de détecter automatiquement l'emplacement via votre adresse IP",
        [nameof(LocationQueryTooltip)] = """
            Spécifiez votre emplacement avec des coordonnées géographiques ou une requête de recherche

            Exemples d'entrées valides :
            **41.25, -120.9762**
            **41.25°N, 120.9762°W**
            **New York, USA**
            **Germany**
            """,
        [nameof(SetLocationTooltip)] = "Définir l'emplacement",
        [nameof(LocationErrorText)] = "Erreur lors de la résolution de l'emplacement, réessayez",
        [nameof(ToggleLightBulbHotkeyLabel)] = "Basculer LightBulb",
        [nameof(ToggleLightBulbHotkeyTooltip)] =
            "Raccourci global pour activer/désactiver LightBulb",
        [nameof(ToggleWindowLabel)] = "Basculer la fenêtre",
        [nameof(ToggleWindowHotkeyTooltip)] =
            "Raccourci global pour afficher/masquer la fenêtre principale de LightBulb",
        [nameof(IncreaseTemperatureOffsetLabel)] = "Décalage de température ↑",
        [nameof(IncreaseTemperatureOffsetTooltip)] =
            "Raccourci global pour augmenter le décalage de température actuel",
        [nameof(DecreaseTemperatureOffsetLabel)] = "Décalage de température ↓",
        [nameof(DecreaseTemperatureOffsetTooltip)] =
            "Raccourci global pour diminuer le décalage de température actuel",
        [nameof(IncreaseBrightnessOffsetLabel)] = "Décalage de luminosité ↑",
        [nameof(IncreaseBrightnessOffsetTooltip)] =
            "Raccourci global pour augmenter le décalage de luminosité actuel",
        [nameof(DecreaseBrightnessOffsetLabel)] = "Décalage de luminosité ↓",
        [nameof(DecreaseBrightnessOffsetTooltip)] =
            "Raccourci global pour diminuer le décalage de luminosité actuel",
        [nameof(ResetOffsetLabel)] = "Réinitialiser le décalage",
        [nameof(ResetOffsetHotkeyTooltip)] =
            "Raccourci global pour réinitialiser les décalages de température et de luminosité actuels",
        [nameof(AppWhitelistLabel)] = "Liste blanche d'applications",
        [nameof(RefreshAppsTooltip)] = "Actualiser les applications en cours d'exécution",
        [nameof(PauseForWhitelistedTooltip)] =
            "Mettre LightBulb en pause quand une des applications sélectionnées est au premier plan",
        [nameof(UpdateAvailableTitle)] = "Mise à jour disponible",
        [nameof(UpdateAvailableMessage)] = """
            La mise à jour vers {0} v{1} a été téléchargée.
            Voulez-vous l'installer maintenant ?
            """,
        [nameof(InstallButton)] = "INSTALLER",
        [nameof(CloseButton)] = "FERMER",
        [nameof(UkraineSupportTitle)] = "Merci de soutenir l'Ukraine !",
        [nameof(UkraineSupportMessage)] = """
            Alors que la Russie mène une guerre génocidaire contre mon pays, je suis reconnaissant envers tous ceux qui continuent à soutenir l'Ukraine dans notre combat pour la liberté.

            Cliquez sur EN SAVOIR PLUS pour trouver des moyens d'aider.
            """,
        [nameof(LearnMoreButton)] = "EN SAVOIR PLUS",
        [nameof(UnstableBuildTitle)] = "Avertissement de build instable",
        [nameof(UnstableBuildMessage)] = """
            Vous utilisez un build de développement de {0}. Ces builds n'ont pas été soigneusement testés et peuvent contenir des bogues.

            Les mises à jour automatiques sont désactivées pour les builds de développement. Si vous souhaitez passer à une version stable, veuillez la télécharger manuellement.
            """,
        [nameof(SeeReleasesButton)] = "VOIR LES VERSIONS",
        [nameof(LimitedGammaRangeTitle)] = "Plage gamma limitée",
        [nameof(LimitedGammaRangeMessage)] = """
            {0} a détecté que les contrôles de plage gamma étendue ne sont pas activés sur ce système.
            Cela peut entraîner un dysfonctionnement de certaines configurations de couleurs.

            Appuyez sur CORRIGER pour déverrouiller la plage gamma. Des droits d'administrateur peuvent être requis.
            """,
        [nameof(FixButton)] = "CORRIGER",
        [nameof(WelcomeTitle)] = "Bienvenue !",
        [nameof(WelcomeMessage)] = """
            Merci d'avoir installé {0} !
            Pour une expérience plus personnalisée, veuillez définir votre configuration solaire préférée.

            Appuyez sur OK pour ouvrir les paramètres.
            """,
        [nameof(OkButton)] = "OK",
    };
}
