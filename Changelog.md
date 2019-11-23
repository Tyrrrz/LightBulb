### v2.0 (23-Nov-2019)

The long awaited new release brings a complete rework of the entire application along with 30+ new features and bug fixes.

>**Important**: LightBulb is now built against .NET Core 3. In order to run this and future versions you will need to **[download and install .NET Core 3 runtime](https://dotnet.microsoft.com/download/dotnet-core/3.0/runtime)** (follow the steps for "Run desktop apps").

Due to substantial changes, your settings from previous versions will be reset. It's also recommended to uninstall LightBulb 1.x first, if you have it installed.

**Changes in this release**:

- Brand new design, new UI, new idea.
- Main window is now bigger and will no longer hide when it loses focus.
- Added the dashboard screen. This is now the screen you will see when you open LightBulb. It shows the sundial which represents the 24-hour day cycle partitioned into day, night and transition phases, along with your current position. It also displays current color configuration, current time, time of sunrise and sunset, and has buttons to start 24-hour preview or to access settings.
- Added application autoupdate. LightBulb will automatically check for updates and install them when it starts.
- Added brightness configuration. Color temperature and brightness now go hand in hand, automatically changing at sunrise and sunset, providing even better blue light reduction. Your eyes will be thankful. [#28](https://github.com/Tyrrrz/LightBulb/issues/28)
- Added editable text boxes for all settings, letting you rigorously fine-tune every detail. Want to go below 2500K at night -- simply type in your desired temperature. Note that values that exceed slider ranges are not tested so use them at your own risk. [#55](https://github.com/Tyrrrz/LightBulb/issues/55) [#90](https://github.com/Tyrrrz/LightBulb/issues/90)
- Added manual location configuration on top of the manual sunrise and sunset configuration that was previously available. You can now specify your coordinates manually or click a button to try detect them automatically from your IP. Besides raw coordinates, you can specify any human-readable location and LightBulb will try to search for the corresponding coordinates. [#59](https://github.com/Tyrrrz/LightBulb/issues/59)
- LightBulb no longer requires consistent internet connection to detect location and calculate sunrise and sunset times. Internet connection is now only required once and only if you autodetect or search location. Sunrise and sunset times are now calculated using an internal algorithm. If you connect to internet through proxy or VPN, there is no need to hack the configuration file anymore, instead you can simply copy-paste your coordinates and you're good to go.
- Added "Start with Windows" option directly to the application settings. Now, if you want, you can enable/disable autostart without having to open regedit or Task Manager. [#73](https://github.com/Tyrrrz/LightBulb/issues/73)
- Added "Default to day-time configuration" option which instructs LightBulb to use your day-time configuration as default configuration (when it's disabled or paused). [#66](https://github.com/Tyrrrz/LightBulb/issues/66)
- Added "Application whitelist" which lets you select applications that will cause LightBulb to pause while they're in foreground. Useful when you are working with color-sensitive applications (e.g. Lightroom, Photoshop) and don't want to manually toggle LightBulb every time. [#39](https://github.com/Tyrrrz/LightBulb/issues/39)
- Added "Disable until sunrise" tray menu item under "Disable for...". This will turn off LightBulb until the next sunrise. [#108](https://github.com/Tyrrrz/LightBulb/issues/108)
- Added stale gamma detection and removed gamma polling. Display gamma now updates only when it actually changes. [#121](https://github.com/Tyrrrz/LightBulb/pull/121)
- Added a check at startup that will verify that extended gamma range is enabled on your computer and, if not, prompt you to enable it automatically. This also means that portable distributions of LightBulb no longer require you to change registry yourself.
- Fixed an issue where LightBulb didn't work properly with multi-monitor setup due to a bug in Windows 10 v1903. [#100](https://github.com/Tyrrrz/LightBulb/issues/100)
- Fixed an issue where LightBulb didn't work properly or reset itself after a display awoke from sleep. [#102](https://github.com/Tyrrrz/LightBulb/issues/102)
- Fixed an issue where LightBulb worked incorrectly around the moment the system clock is adjusted for DST. [#95](https://github.com/Tyrrrz/LightBulb/issues/95)
- Improved overall stability and performance of the application.
- ... and also fixed a dozen other issues that were not mentioned.

**Big thanks** to all of the LightBulb beta testers who helped catch bugs before anyone else (alphabetical order):

- [@aure2006](https://github.com/aure2006)
- [@ChrisLMerrill](https://github.com/ChrisLMerrill)
- [@Dainius14](https://github.com/Dainius14)
- [@mirh](https://github.com/mirh)
- [@NomDeMorte](https://github.com/NomDeMorte)
- [@spacecheese](https://github.com/spacecheese)
- [@ultrakiller2](https://github.com/ultrakiller2)
- [@wswday](https://github.com/wswday)

If you have any problems, check out the [troubleshooting guide](https://github.com/Tyrrrz/LightBulb/wiki/Troubleshooting) and don't hesitate to raise an [issue](https://github.com/Tyrrrz/LightBulb/issues).

### v1.6.4.1 (15-Oct-2018)

- Changed to a different geoip provider, again. This fixes issues with LightBulb not being able to acquire location and sunrise/sunset times automatically since Oct 8th.
- Fixed "Configure" tray menu item not doing anything.

### v1.6.4 (23-Jul-2018)

- Improved the temperature calculation algorithm so that it works for those cases when sunset happens at night.
- Changed the sunrise/sunset sliders in the UI to allow wider selection of options and with higher precision.
- LightBulb will now become visible when the user clicks on the tray icon once, instead of twice. This change brings this behavior in line with most other applications.
- Fixed an issue where LightBulb would consider some system windows as fullscreen when the 'Automatically hide taskbar' option is enabled.
- Fixed an issue where LightBulb couldn't pull geolocational information (and as a result sunrise/sunset times too). Switched to a different service provider because the old one closed down.