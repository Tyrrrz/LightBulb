### v2.4.5 (14-Apr-2022)

- Updated the gamma invalidation strategy to consider gamma stale if the foreground window changed within the last 500 milliseconds. This works around issues where switching to specific windows (for example, Windows Action Center) causes gamma to reset.
- Extended tooltips on brightness sliders to clarify what effect they have.
- Fixed an issue which caused LightBulb to crash occasionally when clicking on its tray icon.

### v2.4.4 (07-Mar-2022)

- Actually fixed it this time.

### v2.4.3 (07-Mar-2022)

- Fixed an issue where the application silently failed to run if the system didn't have .NET Runtime 6.0.2 installed. If you continue seeing this issue, please uninstall all existing .NET runtimes from your computer and then try running the application again.

### v2.4.2 (06-Mar-2022)

- Added message about war in Ukraine.
- Improved the user interface.
- Fixed an issue where manually setting sunrise or sunset time to midnight caused the application to crash.

### v2.4.1 (08-Dec-2021)

- Fixed an issue which caused the decrease brightness hotkey to set the brightness to its lowest value.

### v2.4 (06-Dec-2021)

- Changed target runtime to .NET 6. You don't need to take any action as the application should download and install new prerequisites automatically. In the event it doesn't happen for whatever reason, you can get the runtime here: [Windows x64](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-6.0.0-windows-x64-installer) | [Windows x86](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-6.0.0-windows-x86-installer).
- Fixed an issue which caused yellow-green color glitches on the monitor when the color temperature was set to 500K.
- Fixed an issue which caused color configuration to transition incorrectly to night time if it happened over midnight (00:00).
- Fixed an issue which caused monitor to flicker whenever LightBulb's gamma state went out of sync with the monitor. This happened whenever monitor settings were changed, for example during a transition in wallpaper slideshow.
- Fixed a multitude of issues related to the bootstrapper (prerequisite installer).

### v2.3.3 (15-Jun-2021)

- Fixed an issue where the list of missing components, reported by the prerequisite check at application startup, included Windows updates that were not applicable to the user's system due to the fact that other superseding updates have already been installed. This issue only affected users running LightBulb on Windows 7.
- Fixed a few other minor issues related to the prerequisite installation window.

### v2.3.2 (09-Jun-2021)

- Application will now detect if the required .NET Runtime or any of its prerequisites are missing and prompt the user to download and install them automatically. **Experimental feature, please test it out and report any issues you may find!**
- Fixed performance degradations introduced by the last release.
- Fixed an issue where the application icon was not showing properly in "Programs and Features" window. (Thanks [@derech1e](https://github.com/derech1e))

### v2.3.1 (21-Mar-2021)

- Improved the approach used to refresh gamma after it had been reset by another process. Now, instead of simply re-uploading the last gamma ramp, LightBulb will enter a temporary gamma polling state, causing it to repeatedly force-refresh gamma for a short period of time. This takes care of many remaining cases where gamma would sometimes go out of sync on latest versions of Windows 10.

### v2.3 (12-Nov-2020)

- Fixed an issue where the gamma would sometimes get reset when waking up from sleep or in certain other situations, on latest Windows 10 builds.
- Fixed an issue where opening the settings dialog would hang the UI for 0.5-1s. Also changed the whitelist settings tab to refresh the list of running applications only on user's request, instead of each time it's loaded.
- Fixed an issue where searching for location using a query sometimes didn't work, due to unexpected breaking changes in OpenStreetMap.org.
- Reduced the threshold for minimum gamma changes. This should make the transition period more smooth by performing gamma updates a bit more often.
- Minor UI improvements.

### v2.2 (03-Aug-2020)

- Added configuration transition offset that lets you change how early or late a transition starts relative to the corresponding sunrise or sunset. You can change the offset using a slider in settings, which ranges from 0% (default) to 100% (inverse). The offset defaults to 0%, which resembles the same exact behavior as in previous versions of LightBulb. To understand how exactly the offset affects transitions, please [read this short wiki page](https://github.com/Tyrrrz/LightBulb/wiki/How-the-transition-offset-works).
- Added hotkeys that let you temporarily change current temperature and brightness on the fly using offsets. Once enabled in settings, you can use these hotkeys to increase or decrease current temperature or brightness relative to the projected color configuration. Current offsets are reflected on the main screen of the UI, where you can also instantly reset them. Additionally, you can set up another hotkey to reset the offsets from anywhere. Offsets are not persisted between sessions and will be reset automatically when you exit LightBulb or shut down your computer.
- Improved the update loop performance slightly by eliminating a superfluous timer and synchronization lock.
- Changed minimum temperature configurable from the UI to 500K. In order to set temperature lower than what the slider lets you, type in the value in the text box manually.
- Changed the behavior responsible for discarding insignificant gamma changes, so that the target color configuration is still reflected on the UI regardless of whether the gamma was updated or not.
- Fixed an issue where the UI sometimes locked up for over a second when opening the settings window.
- Fixed an issue where the non-portable version of the application changed storage directory for settings when launched with administrator privileges. This caused settings to reset when launching the application from the installer, as it's usually running in an administrative context.
- Fixed an issue where the current locale wasn't correctly used when parsing user-provided time values in settings. 

### v2.1 (19-Apr-2020)

- Added gamma polling option. When enabled, LightBulb will force-refresh gamma every few seconds to make sure it's in sync. In most cases, you should not need to enable it, but if you're experiencing issues then this option may help.
- Added an option to disable auto-updates. If you prefer not to have the latest version of LightBulb, you now have the option to do so.
- Fixed an issue where LightBulb didn't pick up new monitors if they were connected after it was launched.
- Fixed an issue where the time was shown using the invariant culture, instead of the current system's locale. Now the time format should reflect what's configured on the system. 
- Fixed an issue where the application would work abnormally or crash when there was no full sunrise/sunset for that particular day.
- Minor UI improvements.
- LightBulb installer will now detect if the required .NET Core Runtime installation is missing and download and run the appropriate installer if so.

### v2.0 (23-Nov-2019)

The long awaited new release brings a complete rework of the entire application along with 30+ new features and bug fixes.

> **Warning**:
> LightBulb is now built against .NET Core 3.
> In order to run this and future versions you will need to **[download and install .NET Core 3 runtime](https://dotnet.microsoft.com/download/dotnet-core/current/runtime)** (follow the steps for "Run desktop apps").

Due to substantial changes, your settings from previous versions will be reset. It's also recommended to uninstall LightBulb 1.x first, if you have it installed.

**Changes in this release**:

- Brand new design, new UI, new idea.
- Main window is now bigger and will no longer hide when it loses focus.
- Added the dashboard screen. This is now the screen you will see when you open LightBulb. It shows the sundial which represents the 24-hour day cycle partitioned into day, night and transition phases, along with your current position. It also displays current color configuration, current time, time of sunrise and sunset, and has buttons to start 24-hour preview or to access settings.
- Added application auto-update. LightBulb will automatically check for updates and install them when it starts.
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
