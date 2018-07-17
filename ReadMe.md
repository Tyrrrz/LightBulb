# LightBulb 💡

[![Build](https://img.shields.io/appveyor/ci/Tyrrrz/LightBulb/master.svg)](https://ci.appveyor.com/project/Tyrrrz/LightBulb)
[![Release](https://img.shields.io/github/release/Tyrrrz/LightBulb.svg)](https://github.com/Tyrrrz/LightBulb/releases)
[![Downloads](https://img.shields.io/github/downloads/Tyrrrz/LightBulb/total.svg)](https://github.com/Tyrrrz/LightBulb/releases)

LightBulb is an application that reduces eyestrain produced by staring at a computer screen when working late hours. As the day goes on, it continuously adjusts gamma, transitioning the display color temperature from cold blue in the afternoon to warm yellow during the night. Its primary objective is to match the color of the screen to the light sources of your surrounding environment - typically, sunlight during the day and artificial light during the night. LightBulb has minimal impact on performance and offers many customization options.

_Currently only compatible with Windows 7 and higher._

## Screenshots

![](http://www.tyrrrz.me/Projects/LightBulb/Images/1.png)
![](http://www.tyrrrz.me/Projects/LightBulb/Images/2.png)
![](http://www.tyrrrz.me/Projects/LightBulb/Images/3.png)

## Download

- [Stable releases](https://github.com/Tyrrrz/LightBulb/releases)
- Using Chocolatey: `choco install lightbulb`
- [Continuous integration](https://ci.appveyor.com/project/Tyrrrz/LightBulb)

## Features

- Extensive customization options
- Pull sunrise and sunset times from the Internet or set them manually
- Smooth temperature transition for large changes
- Pause when a fullscreen application or game is in foreground
- Configurable global hotkeys
- Minimal impact on performance

## Customization

LightBulb aims to be very customizable. Refer to [this guide](https://github.com/Tyrrrz/LightBulb/wiki/Settings-explanation) to understand what each setting does.

You can also manually fine-tune every detail using the [configuration file](https://github.com/Tyrrrz/LightBulb/wiki/Config-file).

## Web APIs used

- [Nekudo GeoIP](https://geoip.nekudo.com)
- [SunriseSunset](https://sunrise-sunset.org)
- [GitHub](https://developer.github.com)

## Libraries used

 - [GalaSoft.MVVMLight](http://www.mvvmlight.net)
 - [Newtonsoft.Json](http://www.newtonsoft.com/json)
 - [MaterialDesignInXamlToolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit)
 - [Hardcodet.NotifyIcon.Wpf](http://www.hardcodet.net/wpf-notifyicon)
 - [Tyrrrz.Extensions](https://github.com/Tyrrrz/Extensions)
 - [Tyrrrz.WpfExtensions](https://github.com/Tyrrrz/WpfExtensions)
 - [Tyrrrz.Settings](https://github.com/Tyrrrz/Settings)
