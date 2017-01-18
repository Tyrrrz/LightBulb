LightBulb
===================


Background application that adjusts screen gamma, making the colors appear warmer at night, reducing strain on your eyes.


**Download:**
To download the latest realease - head to the release page

**Available Settings:**

- Maximum color temperature (day-time temperature) [6600]
- Minimum color temperature (night-time temperature) [3900]
- Transition duration (how long it takes to transition from day to night temperature) [90 mins]
- Gamma polling (regularly refresh gamma) [enabled]
- Disable when fullscreen (pause gamma control when a fullscreen application is in foreground) [disabled]
- Sunrise time (when night turns to day) [07:20]
- Sunset time (when day turns to night) [16:30]
- Internet geo sync (updates sunrise/sunset from internet for current location) [enabled]
- \* Temperature update interval (how often is the temperature updated) [1 minute]
- \* Gamma polling interval (when polling is enabled, how often does it happen) [5 seconds]
- \* Internet sync interval (how often are the sunrise/sunset times synchronized) [6 hours]
- \* Default monitor temperature (monitor's color temperature when LightBulb is disabled) [6600]
- \* Temperature epsilon (by how much should color temperature change to warrant gamma refresh) [50]
- \* Temperature smoothing (temperature changes are smoothed out to give time for eyes to adjust) [enabled]
- \* Minimum smoothing delta temperature (by how much should color temperature change to warrant smoothing) [400]
- \* Temperature smoothing duration (when smoothing, how long does it take to finish) [2 seconds]

\* - cannot be changed via UI

**Dependencies:**

 - [GalaSoft.MVVMLight](http://www.mvvmlight.net/) - MVVM rapid development
 - [Json.NET](http://www.newtonsoft.com/json) - for deserialization of API responses
 - [MaterialDesignXAML](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) - MaterialDesign UI
 - [TaskbarNotification](http://www.hardcodet.net/wpf-notifyicon) - tray icon in WPF
 - [NLExtensions](https://www.nuget.org/packages/NLExtensions/) - my set of various extensions for rapid development
 - [NLWPFExtensions](https://www.nuget.org/packages/NLWPFExtensions) - my set of various WPF extensions for rapid development
 - [NLSettings](https://www.nuget.org/packages/NLSettings) - my settings manager

**Screenshots:**

![](http://www.tyrrrz.me/projects/images/lb_1.png)
![](http://www.tyrrrz.me/projects/images/lb_2.png)
![](http://www.tyrrrz.me/projects/images/lb_3.png)