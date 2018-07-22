### v1.6.4 (23-Jul-2018)

- Improved the temperature calculation algorithm so that it works for those cases when sunset happens at night.
- Changed the sunrise/sunset sliders in the UI to allow wider selection of options and with higher precision.
- LightBulb will now become visible when the user clicks on the tray icon once, instead of twice. This change brings this behavior in line with most other applications.
- Fixed an issue where LightBulb would consider some system windows as fullscreen when the 'Automatically hide taskbar' option is enabled.
- Fixed an issue where LightBulb couldn't pull geolocational information (and as a result sunrise/sunset times too). Switched to a different service provider because the old one closed down.