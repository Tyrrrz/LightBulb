using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> SimplifiedChineseLocalization =
        new Dictionary<string, string>
        {
            // Dashboard (仪表盘)
            [nameof(SunsetLabel)] = "日落",
            [nameof(SunriseLabel)] = "日出",
            [nameof(SunsetTransitionStartsAt)] = "日落过渡开始于",
            [nameof(SunriseTransitionStartsAt)] = "日出过渡开始于",
            [nameof(AndEndsAt)] = "结束于",
            [nameof(OffsetTooltipHeader)] = "当前的色温和亮度值已应用偏移量：",
            [nameof(TemperatureOffsetLabel)] = "色温偏移：",
            [nameof(BrightnessOffsetLabel)] = "亮度偏移：",
            [nameof(ClickToResetLabel)] = "点击重置",
            [nameof(OffsetLabel)] = "偏移",

            // Main window (主窗口)
            [nameof(ToggleLightBulbTooltip)] = "开启/关闭 LightBulb",
            [nameof(HideToTrayTooltip)] = "隐藏 LightBulb 到系统托盘",
            [nameof(PreviewText)] = "预览",
            [nameof(StopPreviewTooltip)] = "停止预览",
            [nameof(StartPreviewTooltip)] = "预览 24 小时循环",
            [nameof(SettingsText)] = "设置",
            [nameof(OpenSettingsTooltip)] = "打开设置",
            [nameof(AboutText)] = "GITHUB",
            [nameof(OpenGitHubTooltip)] = "在 GitHub 上打开 LightBulb",

            // Settings dialog (设置对话框)
            [nameof(ResetButton)] = "重置",
            [nameof(ResetTooltip)] = "将所有设置恢复为默认值",
            [nameof(CancelButton)] = "取消",
            [nameof(SaveButton)] = "保存",

            // Settings tabs (设置选项卡)
            [nameof(GeneralTabName)] = "常规",
            [nameof(LocationTabName)] = "位置",
            [nameof(AdvancedTabName)] = "高级",
            [nameof(AppWhitelistTabName)] = "应用白名单",
            [nameof(HotkeysTabName)] = "快捷键",

            // Advanced settings tab (高级设置选项卡)
            [nameof(ThemeLabel)] = "主题",
            [nameof(ThemeTooltip)] = "首选用户界面主题",
            [nameof(LanguageLabel)] = "语言",
            [nameof(LanguageTooltip)] = "首选用户界面语言",
            [nameof(StartWithWindowsLabel)] = "随 Windows 启动",
            [nameof(StartWithWindowsTooltip)] = "在 Windows 启动时运行 LightBulb",
            [nameof(AutoUpdateLabel)] = "自动更新",
            [nameof(AutoUpdateTooltip)] = "自动安装新版本以保持 LightBulb 为最新状态",
            [nameof(DefaultToDayConfigLabel)] = "默认为日间配置",
            [nameof(DefaultToDayConfigTooltip)] = "当 LightBulb 被禁用或暂停时，恢复配置的日间色温和亮度，而不是默认的显示器伽马值",
            [nameof(PauseWhenFullscreenLabel)] = "全屏时暂停",
            [nameof(PauseWhenFullscreenTooltip)] = "当任何全屏窗口处于前台时暂停 LightBulb",
            [nameof(GammaSmoothingLabel)] = "伽马平滑",
            [nameof(GammaSmoothingTooltip)] = "在启用或禁用 LightBulb 时缓慢过渡，给眼睛适应的时间",
            [nameof(GammaPollingLabel)] = "伽马轮询",
            [nameof(GammaPollingTooltip)] = "定期强制刷新显示器伽马值，防止其他程序覆盖它",

            // General settings tab (常规设置选项卡)
            [nameof(DayTemperatureLabel)] = "日间色温：",
            [nameof(DayTemperatureTooltip)] = "白天的色温",
            [nameof(NightTemperatureLabel)] = "夜间色温：",
            [nameof(NightTemperatureTooltip)] = "夜晚的色温",
            [nameof(DayBrightnessLabel)] = "日间亮度：",
            [nameof(DayBrightnessTooltip)] = """
                白天的亮度

                注意：此亮度设置应用于颜色伽马，而非显示器的实际物理亮度。
                如果您的计算机已经能够根据光照条件自动调整屏幕亮度（笔记本电脑常见），建议将两个亮度设置都保持在 100%，从而禁用 LightBulb 的亮度控制。
                """,
            [nameof(NightBrightnessLabel)] = "夜间亮度：",
            [nameof(NightBrightnessTooltip)] = """
                夜晚的亮度

                注意：此亮度设置应用于颜色伽马，而非显示器的实际物理亮度。
                如果您的计算机已经能够根据光照条件自动调整屏幕亮度（笔记本电脑常见），建议将两个亮度设置都保持在 100%，从而禁用 LightBulb 的亮度控制。
                """,
            [nameof(TransitionDurationLabel)] = "过渡时长：",
            [nameof(TransitionDurationTooltip)] = "从日间配置切换到夜间配置所需的时间",
            [nameof(TransitionOffsetLabel)] = "过渡偏移：",
            [nameof(TransitionOffsetTooltip)] = "指定过渡开始时间相对于日出和日落的提前或延后量",

            // Location settings tab (位置设置选项卡)
            [nameof(SolarConfigLabel)] = "太阳配置：",
            [nameof(ManualLabel)] = "手动",
            [nameof(ManualTooltip)] = "手动配置日出和日落时间",
            [nameof(LocationBasedLabel)] = "基于位置",
            [nameof(LocationBasedTooltip)] = "配置您的位置，并以此自动计算日出和日落时间",
            [nameof(SunriseTimeLabel)] = "日出：",
            [nameof(SunsetTimeLabel)] = "日落：",
            [nameof(YourLocationLabel)] = "您的位置：",
            [nameof(AutoDetectLocationTooltip)] = "尝试根据您的 IP 地址自动检测位置",
            [nameof(LocationQueryTooltip)] = """
                使用地理坐标或搜索查询指定您的位置

                有效输入示例：
                **41.25, -120.9762**
                **41.25°N, 120.9762°W**
                **New York, USA (美国纽约)**
                **Germany (德国)**
                """,
            [nameof(SetLocationTooltip)] = "设置位置",
            [nameof(LocationErrorText)] = "解析位置出错，请重试",

            // Hot key settings tab (快捷键设置选项卡)
            [nameof(ToggleLightBulbHotkeyLabel)] = "切换 LightBulb",
            [nameof(ToggleLightBulbHotkeyTooltip)] = "用于开启/关闭 LightBulb 的全局快捷键",
            [nameof(ToggleWindowLabel)] = "切换窗口",
            [nameof(ToggleWindowHotkeyTooltip)] = "用于显示/隐藏 LightBulb 主窗口的全局快捷键",
            [nameof(IncreaseTemperatureOffsetLabel)] = "增加色温偏移 ↑",
            [nameof(IncreaseTemperatureOffsetTooltip)] = "用于增加当前色温偏移量的全局快捷键",
            [nameof(DecreaseTemperatureOffsetLabel)] = "减少色温偏移 ↓",
            [nameof(DecreaseTemperatureOffsetTooltip)] = "用于减少当前色温偏移量的全局快捷键",
            [nameof(IncreaseBrightnessOffsetLabel)] = "增加亮度偏移 ↑",
            [nameof(IncreaseBrightnessOffsetTooltip)] = "用于增加当前亮度偏移量的全局快捷键",
            [nameof(DecreaseBrightnessOffsetLabel)] = "减少亮度偏移 ↓",
            [nameof(DecreaseBrightnessOffsetTooltip)] = "用于减少当前亮度偏移量的全局快捷键",
            [nameof(ResetOffsetLabel)] = "重置偏移",
            [nameof(ResetOffsetHotkeyTooltip)] = "用于重置当前色温和亮度偏移量的全局快捷键",

            // Application whitelist settings tab (应用白名单设置选项卡)
            [nameof(AppWhitelistLabel)] = "应用白名单",
            [nameof(RefreshAppsTooltip)] = "刷新正在运行的应用程序",
            [nameof(PauseForWhitelistedTooltip)] = "当选定的应用程序之一处于前台时暂停 LightBulb",

            // Dialog messages (对话框消息)
            [nameof(UpdateAvailableTitle)] = "可用更新",
            [nameof(UpdateAvailableMessage)] = """
                已下载更新至 {0} v{1}。
                您想现在安装吗？
                """,
            [nameof(InstallButton)] = "安装",
            [nameof(CloseButton)] = "关闭",
            [nameof(UkraineSupportTitle)] = "感谢您支持乌克兰！",
            [nameof(UkraineSupportMessage)] = """
                当俄罗斯对我的国家发动种族灭绝战争时，我感激每一位继续站在乌克兰一边、支持我们争取自由斗争的人。

                大眼仔选择保持中立，受伤害的最终是平民百姓。点击“了解更多”查找您可以提供帮助的方式。
                """,
            [nameof(LearnMoreButton)] = "了解更多",
            [nameof(UnstableBuildTitle)] = "不稳定版本警告",
            [nameof(UnstableBuildMessage)] = """
                您正在使用 {0} 的开发版本。这些版本未经过充分测试，可能包含错误。

                开发版本已禁用自动更新。如果您想切换到稳定版本，请手动下载。
                """,
            [nameof(SeeReleasesButton)] = "查看发布版本",
            [nameof(LimitedGammaRangeTitle)] = "伽马范围受限",
            [nameof(LimitedGammaRangeMessage)] = """
                {0} 检测到该系统未启用扩展伽马范围控制。
                这可能导致某些颜色配置无法正常工作。

                按下“修复”以解锁伽马范围。可能需要管理员权限。
                """,
            [nameof(FixButton)] = "修复",
            [nameof(WelcomeTitle)] = "欢迎使用！",
            [nameof(WelcomeMessage)] = """
                感谢您安装 {0}！
                为了获得最个性化的体验，请设置您首选的太阳配置。

                按下“确定”打开设置。
                """,
            [nameof(OkButton)] = "确定",
        };
}