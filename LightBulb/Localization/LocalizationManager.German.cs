using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> GermanLocalization = new Dictionary<
        string,
        string
    >
    {
        [nameof(SunsetLabel)] = "Sonnenuntergang",
        [nameof(SunriseLabel)] = "Sonnenaufgang",
        [nameof(SunsetTransitionStartsAt)] = "Sonnenuntergangsübergang beginnt um",
        [nameof(SunriseTransitionStartsAt)] = "Sonnenaufgangsübergang beginnt um",
        [nameof(AndEndsAt)] = "und endet um",
        [nameof(OffsetTooltipHeader)] =
            "Aktuelle Temperatur- und Helligkeitswerte werden durch einen Versatz angepasst:",
        [nameof(TemperatureOffsetLabel)] = "Temperaturversatz:",
        [nameof(BrightnessOffsetLabel)] = "Helligkeitsversatz:",
        [nameof(ClickToResetLabel)] = "Zum Zurücksetzen klicken",
        [nameof(OffsetLabel)] = "Versatz",
        [nameof(ToggleLightBulbTooltip)] = "LightBulb ein-/ausschalten",
        [nameof(HideToTrayTooltip)] = "LightBulb in den Infobereich minimieren",
        [nameof(PreviewText)] = "VORSCHAU",
        [nameof(StopPreviewTooltip)] = "Vorschau beenden",
        [nameof(StartPreviewTooltip)] = "24-Stunden-Zyklus vorschauen",
        [nameof(SettingsText)] = "EINSTELLUNGEN",
        [nameof(OpenSettingsTooltip)] = "Einstellungen öffnen",
        [nameof(AboutText)] = "ÜBER",
        [nameof(OpenGitHubTooltip)] = "LightBulb auf GitHub öffnen",
        [nameof(ResetButton)] = "ZURÜCKSETZEN",
        [nameof(ResetTooltip)] = "Alle Einstellungen auf die Standardwerte zurücksetzen",
        [nameof(CancelButton)] = "ABBRECHEN",
        [nameof(SaveButton)] = "SPEICHERN",
        [nameof(GeneralTabName)] = "Allgemein",
        [nameof(LocationTabName)] = "Standort",
        [nameof(AdvancedTabName)] = "Erweitert",
        [nameof(AppWhitelistTabName)] = "App-Whitelist",
        [nameof(HotkeysTabName)] = "Tastenkombinationen",
        [nameof(ThemeLabel)] = "Design",
        [nameof(ThemeTooltip)] = "Bevorzugtes Oberflächendesign",
        [nameof(LanguageLabel)] = "Sprache",
        [nameof(LanguageTooltip)] = "Bevorzugte Sprache der Benutzeroberfläche",
        [nameof(StartWithWindowsLabel)] = "Mit Windows starten",
        [nameof(StartWithWindowsTooltip)] = "LightBulb beim Windows-Start automatisch starten",
        [nameof(AutoUpdateLabel)] = "Automatische Updates",
        [nameof(AutoUpdateTooltip)] =
            "LightBulb aktuell halten, indem neue Versionen automatisch installiert werden",
        [nameof(DefaultToDayConfigLabel)] = "Standard: Tageskonfiguration",
        [nameof(DefaultToDayConfigTooltip)] =
            "Wenn LightBulb deaktiviert oder pausiert ist, die konfigurierte Tagestemperatur und -helligkeit anstelle des Standard-Monitor-Gammas wiederherstellen",
        [nameof(PauseWhenFullscreenLabel)] = "Bei Vollbild pausieren",
        [nameof(PauseWhenFullscreenTooltip)] =
            "LightBulb pausieren, wenn ein Vollbildfenster im Vordergrund ist",
        [nameof(GammaSmoothingLabel)] = "Gamma-Glättung",
        [nameof(GammaSmoothingTooltip)] =
            "Beim Ein- oder Ausschalten von LightBulb langsam übergehen, um den Augen Zeit zur Anpassung zu geben",
        [nameof(GammaPollingLabel)] = "Gamma-Abfrage",
        [nameof(GammaPollingTooltip)] =
            "Monitor-Gamma in regelmäßigen Abständen auffrischen, um andere Programme daran zu hindern, es zu überschreiben",
        [nameof(DayTemperatureLabel)] = "Tages-Farbtemperatur:",
        [nameof(DayTemperatureTooltip)] = "Farbtemperatur tagsüber",
        [nameof(NightTemperatureLabel)] = "Nacht-Farbtemperatur:",
        [nameof(NightTemperatureTooltip)] = "Farbtemperatur nachts",
        [nameof(DayBrightnessLabel)] = "Tageshelligkeit:",
        [nameof(DayBrightnessTooltip)] =
            "Helligkeit tagsüber\n\nDiese Helligkeitseinstellung wirkt auf das Farbgamma, nicht auf die tatsächliche Bildschirmhelligkeit.\nWenn Ihr Computer bereits in der Lage ist, die Bildschirmhelligkeit automatisch anzupassen (häufig bei Laptops), wird empfohlen, die Helligkeitssteuerung von LightBulb zu deaktivieren, indem beide Helligkeitswerte auf 100% gesetzt werden.",
        [nameof(NightBrightnessLabel)] = "Nachthelligkeit:",
        [nameof(NightBrightnessTooltip)] =
            "Helligkeit nachts\n\nDiese Helligkeitseinstellung wirkt auf das Farbgamma, nicht auf die tatsächliche Bildschirmhelligkeit.\nWenn Ihr Computer bereits in der Lage ist, die Bildschirmhelligkeit automatisch anzupassen (häufig bei Laptops), wird empfohlen, die Helligkeitssteuerung von LightBulb zu deaktivieren, indem beide Helligkeitswerte auf 100% gesetzt werden.",
        [nameof(TransitionDurationLabel)] = "Übergangsdauer:",
        [nameof(TransitionDurationTooltip)] =
            "Zeitdauer für den Wechsel zwischen Tages- und Nachtkonfiguration",
        [nameof(TransitionOffsetLabel)] = "Übergangsversatz:",
        [nameof(TransitionOffsetTooltip)] =
            "Versatz, der angibt, wie früh oder spät der Übergang relativ zu Sonnenaufgang und Sonnenuntergang beginnt",
        [nameof(SolarConfigLabel)] = "Solarkonfiguration:",
        [nameof(ManualLabel)] = "Manuell",
        [nameof(ManualTooltip)] = "Sonnenaufgang und Sonnenuntergang manuell konfigurieren",
        [nameof(LocationBasedLabel)] = "Standortbasiert",
        [nameof(LocationBasedTooltip)] =
            "Standort konfigurieren und zur automatischen Berechnung der Sonnenzeiten nutzen",
        [nameof(SunriseTimeLabel)] = "Sonnenaufgang:",
        [nameof(SunsetTimeLabel)] = "Sonnenuntergang:",
        [nameof(YourLocationLabel)] = "Ihr Standort:",
        [nameof(AutoDetectLocationTooltip)] =
            "Standort automatisch anhand Ihrer IP-Adresse erkennen",
        [nameof(LocationQueryTooltip)] =
            "Geben Sie Ihren Standort mit geografischen Koordinaten oder einer Suchanfrage an\n\nBeispiele gültiger Eingaben:\n**41.25, -120.9762**\n**41.25°N, 120.9762°W**\n**New York, USA**\n**Deutschland**",
        [nameof(SetLocationTooltip)] = "Standort festlegen",
        [nameof(LocationErrorText)] = "Fehler beim Ermitteln des Standorts, bitte erneut versuchen",
        [nameof(ToggleLightBulbHotkeyLabel)] = "LightBulb umschalten",
        [nameof(ToggleLightBulbHotkeyTooltip)] =
            "Globale Tastenkombination zum Ein-/Ausschalten von LightBulb",
        [nameof(ToggleWindowLabel)] = "Fenster umschalten",
        [nameof(ToggleWindowHotkeyTooltip)] =
            "Globale Tastenkombination zum Anzeigen/Ausblenden des LightBulb-Fensters",
        [nameof(IncreaseTemperatureOffsetLabel)] = "Temperaturversatz ↑",
        [nameof(IncreaseTemperatureOffsetTooltip)] =
            "Globale Tastenkombination zum Erhöhen des aktuellen Temperaturversatzes",
        [nameof(DecreaseTemperatureOffsetLabel)] = "Temperaturversatz ↓",
        [nameof(DecreaseTemperatureOffsetTooltip)] =
            "Globale Tastenkombination zum Verringern des aktuellen Temperaturversatzes",
        [nameof(IncreaseBrightnessOffsetLabel)] = "Helligkeitsversatz ↑",
        [nameof(IncreaseBrightnessOffsetTooltip)] =
            "Globale Tastenkombination zum Erhöhen des aktuellen Helligkeitsversatzes",
        [nameof(DecreaseBrightnessOffsetLabel)] = "Helligkeitsversatz ↓",
        [nameof(DecreaseBrightnessOffsetTooltip)] =
            "Globale Tastenkombination zum Verringern des aktuellen Helligkeitsversatzes",
        [nameof(ResetOffsetLabel)] = "Versatz zurücksetzen",
        [nameof(ResetOffsetHotkeyTooltip)] =
            "Globale Tastenkombination zum Zurücksetzen der Temperatur- und Helligkeitsversätze",
        [nameof(AppWhitelistLabel)] = "Anwendungs-Whitelist",
        [nameof(RefreshAppsTooltip)] = "Laufende Anwendungen aktualisieren",
        [nameof(PauseForWhitelistedTooltip)] =
            "LightBulb pausieren, wenn eine der ausgewählten Anwendungen im Vordergrund ist",
        [nameof(UpdateAvailableTitle)] = "Update verfügbar",
        [nameof(UpdateAvailableMessage)] =
            "Update auf {0} v{1} wurde heruntergeladen.\nMöchten Sie es jetzt installieren?",
        [nameof(InstallButton)] = "INSTALLIEREN",
        [nameof(CloseButton)] = "SCHLIESSEN",
        [nameof(UkraineSupportTitle)] = "Danke für Ihre Unterstützung der Ukraine!",
        [nameof(UkraineSupportMessage)] =
            "Während Russland einen Vernichtungskrieg gegen mein Land führt, bin ich jedem dankbar, der weiterhin an der Seite der Ukraine in unserem Kampf für die Freiheit steht.\n\nKlicken Sie auf MEHR ERFAHREN, um Möglichkeiten der Unterstützung zu finden.",
        [nameof(LearnMoreButton)] = "MEHR ERFAHREN",
        [nameof(UnstableBuildTitle)] = "Warnung: Instabiler Build",
        [nameof(UnstableBuildMessage)] =
            "Sie verwenden einen Entwicklungs-Build von {0}. Diese Builds wurden nicht gründlich getestet und können Fehler enthalten.\n\nAutomatische Updates sind für Entwicklungs-Builds deaktiviert. Wenn Sie zu einem stabilen Release wechseln möchten, laden Sie ihn bitte manuell herunter.",
        [nameof(SeeReleasesButton)] = "RELEASES ANZEIGEN",
        [nameof(LimitedGammaRangeTitle)] = "Begrenzter Gamma-Bereich",
        [nameof(LimitedGammaRangeMessage)] =
            "{0} hat festgestellt, dass erweiterte Gamma-Bereichssteuerungen auf diesem System nicht aktiviert sind.\nDies kann dazu führen, dass einige Farbkonfigurationen nicht korrekt funktionieren.\n\nDrücken Sie BEHEBEN, um den Gamma-Bereich zu entsperren. Administratorrechte können erforderlich sein.",
        [nameof(FixButton)] = "BEHEBEN",
        [nameof(WelcomeTitle)] = "Willkommen!",
        [nameof(WelcomeMessage)] =
            "Danke für die Installation von {0}!\nFür das beste personalisierte Erlebnis legen Sie bitte Ihre bevorzugte Solarkonfiguration fest.\n\nDrücken Sie OK, um die Einstellungen zu öffnen.",
        [nameof(OkButton)] = "OK",
    };
}
