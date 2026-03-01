using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> SpanishLocalization =
        new Dictionary<string, string>
        {
            [nameof(SunsetLabel)] = "Puesta de sol",
            [nameof(SunriseLabel)] = "Amanecer",
            [nameof(SunsetTransitionStartsAt)] = "La transición del atardecer comienza a las",
            [nameof(SunriseTransitionStartsAt)] = "La transición del amanecer comienza a las",
            [nameof(AndEndsAt)] = "y termina a las",
            [nameof(OffsetTooltipHeader)] =
                "Los valores actuales de temperatura y brillo están ajustados por un desplazamiento:",
            [nameof(TemperatureOffsetLabel)] = "Desplazamiento de temperatura:",
            [nameof(BrightnessOffsetLabel)] = "Desplazamiento de brillo:",
            [nameof(ClickToResetLabel)] = "Clic para restablecer",
            [nameof(OffsetLabel)] = "desplazamiento",
            [nameof(ToggleLightBulbTooltip)] = "Activar/desactivar LightBulb",
            [nameof(HideToTrayTooltip)] = "Minimizar LightBulb en la bandeja del sistema",
            [nameof(PreviewText)] = "VISTA PREVIA",
            [nameof(StopPreviewTooltip)] = "Detener vista previa",
            [nameof(StartPreviewTooltip)] = "Vista previa del ciclo de 24 horas",
            [nameof(SettingsText)] = "AJUSTES",
            [nameof(OpenSettingsTooltip)] = "Abrir ajustes",
            [nameof(AboutText)] = "ACERCA DE",
            [nameof(OpenGitHubTooltip)] = "Abrir LightBulb en GitHub",
            [nameof(ResetButton)] = "RESTABLECER",
            [nameof(ResetTooltip)] = "Restablecer todos los ajustes a sus valores predeterminados",
            [nameof(CancelButton)] = "CANCELAR",
            [nameof(SaveButton)] = "GUARDAR",
            [nameof(GeneralTabName)] = "General",
            [nameof(LocationTabName)] = "Ubicación",
            [nameof(AdvancedTabName)] = "Avanzado",
            [nameof(AppWhitelistTabName)] = "Lista blanca",
            [nameof(HotkeysTabName)] = "Atajos de teclado",
            [nameof(ThemeLabel)] = "Tema",
            [nameof(ThemeTooltip)] = "Tema de interfaz preferido",
            [nameof(LanguageLabel)] = "Idioma",
            [nameof(LanguageTooltip)] = "Idioma de interfaz preferido",
            [nameof(StartWithWindowsLabel)] = "Iniciar con Windows",
            [nameof(StartWithWindowsTooltip)] = "Iniciar LightBulb al arrancar Windows",
            [nameof(AutoUpdateLabel)] = "Actualización automática",
            [nameof(AutoUpdateTooltip)] =
                "Mantener LightBulb actualizado instalando automáticamente nuevas versiones",
            [nameof(DefaultToDayConfigLabel)] = "Usar configuración diurna por defecto",
            [nameof(DefaultToDayConfigTooltip)] =
                "Cuando LightBulb está desactivado o en pausa, restaurar la temperatura y el brillo diurnos configurados en lugar del gamma predeterminado del monitor",
            [nameof(PauseWhenFullscreenLabel)] = "Pausar en pantalla completa",
            [nameof(PauseWhenFullscreenTooltip)] =
                "Pausar LightBulb cuando alguna ventana a pantalla completa esté en primer plano",
            [nameof(GammaSmoothingLabel)] = "Suavizado de gamma",
            [nameof(GammaSmoothingTooltip)] =
                "Transicionar lentamente al activar o desactivar LightBulb para dar tiempo a los ojos a adaptarse",
            [nameof(GammaPollingLabel)] = "Sondeo de gamma",
            [nameof(GammaPollingTooltip)] =
                "Actualizar forzosamente el gamma del monitor a intervalos regulares para evitar que otros programas lo sobreescriban",
            [nameof(DayTemperatureLabel)] = "Temperatura de color diurna:",
            [nameof(DayTemperatureTooltip)] = "Temperatura de color durante el día",
            [nameof(NightTemperatureLabel)] = "Temperatura de color nocturna:",
            [nameof(NightTemperatureTooltip)] = "Temperatura de color durante la noche",
            [nameof(DayBrightnessLabel)] = "Brillo diurno:",
            [nameof(DayBrightnessTooltip)] =
                "Brillo durante el día\n\nEste ajuste de brillo se aplica al gamma de colores, no al brillo real del monitor.\nSi su ordenador ya puede ajustar automáticamente el brillo de la pantalla según las condiciones de iluminación (común en portátiles), se recomienda deshabilitar el control de brillo de LightBulb manteniendo ambos ajustes de brillo al 100%.",
            [nameof(NightBrightnessLabel)] = "Brillo nocturno:",
            [nameof(NightBrightnessTooltip)] =
                "Brillo durante la noche\n\nEste ajuste de brillo se aplica al gamma de colores, no al brillo real del monitor.\nSi su ordenador ya puede ajustar automáticamente el brillo de la pantalla según las condiciones de iluminación (común en portátiles), se recomienda deshabilitar el control de brillo de LightBulb manteniendo ambos ajustes de brillo al 100%.",
            [nameof(TransitionDurationLabel)] = "Duración de transición:",
            [nameof(TransitionDurationTooltip)] =
                "Duración del tiempo que tarda en cambiar entre las configuraciones diurna y nocturna",
            [nameof(TransitionOffsetLabel)] = "Desplazamiento de transición:",
            [nameof(TransitionOffsetTooltip)] =
                "Desplazamiento que especifica cuán temprano o tarde comienza la transición, relativo al amanecer y al atardecer",
            [nameof(SolarConfigLabel)] = "Configuración solar:",
            [nameof(ManualLabel)] = "Manual",
            [nameof(ManualTooltip)] = "Configurar el amanecer y el atardecer manualmente",
            [nameof(LocationBasedLabel)] = "Basado en ubicación",
            [nameof(LocationBasedTooltip)] =
                "Configurar tu ubicación para calcular automáticamente las horas de amanecer y atardecer",
            [nameof(SunriseTimeLabel)] = "Amanecer:",
            [nameof(SunsetTimeLabel)] = "Atardecer:",
            [nameof(YourLocationLabel)] = "Tu ubicación:",
            [nameof(AutoDetectLocationTooltip)] =
                "Intentar detectar la ubicación automáticamente según tu dirección IP",
            [nameof(LocationQueryTooltip)] =
                "Especifica tu ubicación con coordenadas geográficas o una consulta de búsqueda\n\nEjemplos de entradas válidas:\n41.25, -120.9762\n41.25°N, 120.9762°W\nNew York, USA\nAlemania",
            [nameof(SetLocationTooltip)] = "Establecer ubicación",
            [nameof(LocationErrorText)] = "Error al resolver la ubicación, inténtalo de nuevo",
            [nameof(ToggleLightBulbHotkeyLabel)] = "Alternar LightBulb",
            [nameof(ToggleLightBulbHotkeyTooltip)] =
                "Atajo global para activar/desactivar LightBulb",
            [nameof(ToggleWindowLabel)] = "Alternar ventana",
            [nameof(ToggleWindowHotkeyTooltip)] =
                "Atajo global para mostrar/ocultar la ventana principal de LightBulb",
            [nameof(IncreaseTemperatureOffsetLabel)] = "Desplazamiento de temperatura ↑",
            [nameof(IncreaseTemperatureOffsetTooltip)] =
                "Atajo global para aumentar el desplazamiento de temperatura actual",
            [nameof(DecreaseTemperatureOffsetLabel)] = "Desplazamiento de temperatura ↓",
            [nameof(DecreaseTemperatureOffsetTooltip)] =
                "Atajo global para disminuir el desplazamiento de temperatura actual",
            [nameof(IncreaseBrightnessOffsetLabel)] = "Desplazamiento de brillo ↑",
            [nameof(IncreaseBrightnessOffsetTooltip)] =
                "Atajo global para aumentar el desplazamiento de brillo actual",
            [nameof(DecreaseBrightnessOffsetLabel)] = "Desplazamiento de brillo ↓",
            [nameof(DecreaseBrightnessOffsetTooltip)] =
                "Atajo global para disminuir el desplazamiento de brillo actual",
            [nameof(ResetOffsetLabel)] = "Restablecer desplazamiento",
            [nameof(ResetOffsetHotkeyTooltip)] =
                "Atajo global para restablecer los desplazamientos de temperatura y brillo actuales",
            [nameof(AppWhitelistLabel)] = "Lista blanca de aplicaciones",
            [nameof(RefreshAppsTooltip)] = "Actualizar aplicaciones en ejecución",
            [nameof(PauseForWhitelistedTooltip)] =
                "Pausar LightBulb cuando una de las aplicaciones seleccionadas esté en primer plano",
            [nameof(UpdateAvailableTitle)] = "Actualización disponible",
            [nameof(UpdateAvailableMessage)] =
                "La actualización a {0} v{1} ha sido descargada.\n¿Deseas instalarla ahora?",
            [nameof(InstallButton)] = "INSTALAR",
            [nameof(CloseButton)] = "CERRAR",
            [nameof(UkraineSupportTitle)] = "¡Gracias por apoyar a Ucrania!",
            [nameof(UkraineSupportMessage)] =
                "Mientras Rusia libra una guerra genocida contra mi país, estoy agradecido a todos los que continúan apoyando a Ucrania en nuestra lucha por la libertad.\n\nHaz clic en MÁS INFORMACIÓN para encontrar formas de ayudar.",
            [nameof(LearnMoreButton)] = "MÁS INFORMACIÓN",
            [nameof(UnstableBuildTitle)] = "Advertencia de compilación inestable",
            [nameof(UnstableBuildMessage)] =
                "Estás usando una compilación de desarrollo de {0}. Estas compilaciones no han sido probadas exhaustivamente y pueden contener errores.\n\nLas actualizaciones automáticas están desactivadas para compilaciones de desarrollo. Si quieres cambiar a una versión estable, descárgala manualmente.",
            [nameof(SeeReleasesButton)] = "VER VERSIONES",
            [nameof(LimitedGammaRangeTitle)] = "Rango gamma limitado",
            [nameof(LimitedGammaRangeMessage)] =
                "{0} ha detectado que los controles de rango gamma extendido no están habilitados en este sistema.\nEsto puede hacer que algunas configuraciones de color no funcionen correctamente.\n\nPresiona CORREGIR para desbloquear el rango gamma. Pueden requerirse privilegios de administrador.",
            [nameof(FixButton)] = "CORREGIR",
            [nameof(WelcomeTitle)] = "¡Bienvenido!",
            [nameof(WelcomeMessage)] =
                "¡Gracias por instalar {0}!\nPara una experiencia más personalizada, establece tu configuración solar preferida.\n\nPresiona OK para abrir los ajustes.",
            [nameof(OkButton)] = "OK",
        };
}
