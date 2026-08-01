using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> ChineseTraditionalLocalization =
        new Dictionary<string, string>
        {
            // Dashboard
            [nameof(SunsetLabel)] = "日落",
            [nameof(SunriseLabel)] = "日出",
            [nameof(SunsetTransitionTooltip)] = "日落過渡效果開始於 **{0}**，結束於 **{1}**",
            [nameof(SunriseTransitionTooltip)] = "日出過渡效果開始於 **{0}**，結束於 **{1}**",
            [nameof(OffsetTooltipHeader)] = "目前的色溫與亮度值已透過微調值進行調整：",
            [nameof(TemperatureOffsetLabel)] = "色溫微調：",
            [nameof(BrightnessOffsetLabel)] = "亮度微調：",
            [nameof(ClickToResetLabel)] = "點擊重設",
            [nameof(OffsetLabel)] = "微調",
            // Main window
            [nameof(ToggleLightBulbTooltip)] = "切換 LightBulb 開啟/關閉",
            [nameof(HideToTrayTooltip)] = "將 LightBulb 隱藏至系統工作列",
            [nameof(PreviewText)] = "預覽",
            [nameof(StopPreviewTooltip)] = "停止預覽",
            [nameof(StartPreviewTooltip)] = "預覽 24 小時週期",
            [nameof(SettingsText)] = "設定",
            [nameof(OpenSettingsTooltip)] = "開啟設定",
            // Settings dialog
            [nameof(ResetButton)] = "重設",
            [nameof(ResetTooltip)] = "將所有設定重設為預設值",
            [nameof(CancelButton)] = "取消",
            [nameof(SaveButton)] = "儲存",
            // Settings tabs
            [nameof(GeneralTabName)] = "一般",
            [nameof(LocationTabName)] = "位置",
            [nameof(AdvancedTabName)] = "進階",
            [nameof(AppWhitelistTabName)] = "應用程式白名單",
            [nameof(HotkeysTabName)] = "快捷鍵",
            // Advanced settings tab
            [nameof(ThemeLabel)] = "主題",
            [nameof(ThemeTooltip)] = "偏好的使用者介面主題",
            [nameof(LanguageLabel)] = "語言",
            [nameof(LanguageTooltip)] = "偏好的使用者介面語言",
            [nameof(StartWithWindowsLabel)] = "隨 Windows 啟動",
            [nameof(StartWithWindowsTooltip)] = "在 Windows 啟動時自動執行 LightBulb",
            [nameof(AutoUpdateLabel)] = "自動更新",
            [nameof(AutoUpdateTooltip)] = "當有新版本可用時自動安裝，保持 LightBulb 為最新狀態",
            [nameof(DefaultToDayConfigLabel)] = "預設採用白天設定",
            [nameof(DefaultToDayConfigTooltip)] =
                "當 LightBulb 停用或暫停時，還原為設定的白天色溫與亮度，而非預設的顯示器 Gamma 值",
            [nameof(PauseWhenFullscreenLabel)] = "全螢幕時暫停",
            [nameof(PauseWhenFullscreenTooltip)] = "當前景有任何全螢幕視窗時暫停 LightBulb",
            [nameof(GammaSmoothingLabel)] = "Gamma 平滑化",
            [nameof(GammaSmoothingTooltip)] =
                "在啟用或停用 LightBulb 時緩慢過渡，以便讓眼睛有時間適應",
            [nameof(GammaPollingLabel)] = "Gamma 輪詢",
            [nameof(GammaPollingTooltip)] =
                "定期強制重新整理顯示器 Gamma 值，以防止其他程式覆蓋此設定",
            // General settings tab
            [nameof(DayTemperatureLabel)] = "白天色溫：",
            [nameof(DayTemperatureTooltip)] = "白天時的色溫",
            [nameof(NightTemperatureLabel)] = "夜晚色溫：",
            [nameof(NightTemperatureTooltip)] = "夜晚時的色溫",
            [nameof(DayBrightnessLabel)] = "白天亮度：",
            [nameof(DayBrightnessTooltip)] = """
                白天時的亮度

                請注意，此亮度設定適用於色彩 Gamma 值，而非顯示器的實際硬體亮度。
                如果您的電腦已經能夠根據光線條件自動調整螢幕亮度（常見於筆記型電腦），建議將兩種亮度設定都保持在 100% 以停用 LightBulb 的亮度控制。
                """,
            [nameof(NightBrightnessLabel)] = "夜晚亮度：",
            [nameof(NightBrightnessTooltip)] = """
                夜晚時的亮度

                請注意，此亮度設定適用於色彩 Gamma 值，而非顯示器的實際硬體亮度。
                如果您的電腦已經能夠根據光線條件自動調整螢幕亮度（常見於筆記型電腦），建議將兩種亮度設定都保持在 100% 以停用 LightBulb 的亮度控制。
                """,
            [nameof(TransitionDurationLabel)] = "過渡時間：",
            [nameof(TransitionDurationTooltip)] = "在白天和夜晚設定之間切換所需的時間長度",
            [nameof(TransitionOffsetLabel)] = "過渡偏移：",
            [nameof(TransitionOffsetTooltip)] =
                "相對於日出和日落，指定過渡效果提前或延後開始的偏移時間",
            // Location settings tab
            [nameof(SolarConfigLabel)] = "太陽週期設定：",
            [nameof(ManualLabel)] = "手動",
            [nameof(ManualTooltip)] = "手動設定日出與日落時間",
            [nameof(LocationBasedLabel)] = "基於位置",
            [nameof(LocationBasedTooltip)] = "設定您的位置並自動計算日出與日落時間",
            [nameof(SunriseTimeLabel)] = "日出：",
            [nameof(SunsetTimeLabel)] = "日落：",
            [nameof(YourLocationLabel)] = "您的位置：",
            [nameof(AutoDetectLocationTooltip)] = "嘗試根據您的 IP 位址自動偵測位置",
            [nameof(LocationQueryTooltip)] = """
                使用地理座標或搜尋關鍵字來指定您的位置

                有效輸入範例：
                41.25, -120.9762
                41.25°N, 120.9762°W
                New York, USA
                Germany
                """,
            [nameof(SetLocationTooltip)] = "設定位置",
            [nameof(LocationErrorText)] = "解析位置時發生錯誤，請再試一次",
            // Hot key settings tab
            [nameof(ToggleLightBulbHotkeyLabel)] = "切換 LightBulb",
            [nameof(ToggleLightBulbHotkeyTooltip)] = "全域快捷鍵，用於開啟/關閉 LightBulb",
            [nameof(ToggleWindowLabel)] = "切換視窗",
            [nameof(ToggleWindowHotkeyTooltip)] = "全域快捷鍵，用於顯示/隱藏 LightBulb 主視窗",
            [nameof(IncreaseTemperatureOffsetLabel)] = "色溫微調 ↑",
            [nameof(IncreaseTemperatureOffsetTooltip)] = "全域快捷鍵，用於增加目前的色溫微調值",
            [nameof(DecreaseTemperatureOffsetLabel)] = "色溫微調 ↓",
            [nameof(DecreaseTemperatureOffsetTooltip)] = "全域快捷鍵，用於減少目前的色溫微調值",
            [nameof(IncreaseBrightnessOffsetLabel)] = "亮度微調 ↑",
            [nameof(IncreaseBrightnessOffsetTooltip)] = "全域快捷鍵，用於增加目前的亮度微調值",
            [nameof(DecreaseBrightnessOffsetLabel)] = "亮度微調 ↓",
            [nameof(DecreaseBrightnessOffsetTooltip)] = "全域快捷鍵，用於減少目前的亮度微調值",
            [nameof(ResetOffsetLabel)] = "重設微調",
            [nameof(ResetOffsetHotkeyTooltip)] = "全域快捷鍵，用於重設目前的色溫與亮度微調值",
            // Application whitelist settings tab
            [nameof(AppWhitelistLabel)] = "應用程式白名單",
            [nameof(RefreshAppsTooltip)] = "重新整理執行中的應用程式",
            [nameof(PauseForWhitelistedTooltip)] = "當選定的應用程式之一在前景執行時暫停 LightBulb",
            // Tray icon context menu
            [nameof(TrayShowMenuItem)] = "顯示",
            [nameof(TrayHideMenuItem)] = "隱藏",
            [nameof(TraySettingsMenuItem)] = "設定",
            [nameof(TrayEnableMenuItem)] = "啟用",
            [nameof(TrayDisableMenuItem)] = "停用",
            [nameof(TrayDisableTemporarilyMenuItem)] = "暫時停用...",
            [nameof(TrayDisableUntilSunriseMenuItem)] = "直到日出",
            [nameof(TrayDisableFor1DayMenuItem)] = "停用 1 天",
            [nameof(TrayDisableFor12HoursMenuItem)] = "停用 12 小時",
            [nameof(TrayDisableFor6HoursMenuItem)] = "停用 6 小時",
            [nameof(TrayDisableFor3HoursMenuItem)] = "停用 3 小時",
            [nameof(TrayDisableFor1HourMenuItem)] = "停用 1 小時",
            [nameof(TrayDisableFor30MinutesMenuItem)] = "停用 30 分鐘",
            [nameof(TrayDisableFor15MinutesMenuItem)] = "停用 15 分鐘",
            [nameof(TrayDisableFor5MinutesMenuItem)] = "停用 5 分鐘",
            [nameof(TrayDisableFor1MinuteMenuItem)] = "停用 1 分鐘",
            [nameof(TrayExitMenuItem)] = "結束",
            [nameof(TrayTooltipDisabled)] = "已停用",
            // Dialog messages
            [nameof(UpdateAvailableTitle)] = "有可用更新",
            [nameof(UpdateAvailableMessage)] = """
                {0} v{1} 更新已下載完成。
                您現在要安裝嗎？
                """,
            [nameof(InstallButton)] = "安裝",
            [nameof(CloseButton)] = "關閉",
            [nameof(UkraineSupportTitle)] = "感謝您支援烏克蘭！",
            [nameof(UkraineSupportMessage)] = """
                在俄羅斯對我國發動種族滅絕戰爭之際，我非常感謝所有在我們爭取自由的鬥爭中繼續與烏克蘭站在一起的人。

                點擊瞭解更多以尋找您可以提供幫助的方式。
                """,
            [nameof(LearnMoreButton)] = "瞭解更多",
            [nameof(UnstableBuildTitle)] = "不穩定版本警告",
            [nameof(UnstableBuildMessage)] = """
                您正在使用 {0} 的開發版本。這些版本尚未經過充分測試，可能包含 Bug。

                開發版本已停用自動更新。如果您想切換至穩定發行版，請手動下載。
                """,
            [nameof(SeeReleasesButton)] = "查看發行版本",
            [nameof(LimitedGammaRangeTitle)] = "Gamma 範圍受限",
            [nameof(LimitedGammaRangeMessage)] = """
                {0} 偵測到此系統上未啟用延伸 Gamma 範圍控制。
                這可能會導致某些色彩設定無法正常運作。

                按一下修復以解鎖 Gamma 範圍。可能需要系統管理員權限。
                """,
            [nameof(FixButton)] = "修復",
            [nameof(WelcomeTitle)] = "歡迎！",
            [nameof(WelcomeMessage)] = """
                感謝您安裝 {0}！
                為了獲得最個人化的體驗，請設定您偏好的太陽週期設定。

                按一下確定以開啟設定。
                """,
            [nameof(OkButton)] = "確定",
        };
}
