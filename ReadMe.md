# LightBulb 💡

[![Build](https://github.com/Tyrrrz/LightBulb/workflows/CI/badge.svg?branch=master)](https://github.com/Tyrrrz/LightBulb/actions)
[![Release](https://img.shields.io/github/release/Tyrrrz/LightBulb.svg)](https://github.com/Tyrrrz/LightBulb/releases)
[![Downloads](https://img.shields.io/github/downloads/Tyrrrz/LightBulb/total.svg)](https://github.com/Tyrrrz/LightBulb/releases)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)

**Project status: maintenance mode** (bug fixes only).

LightBulb is an application that reduces eyestrain produced by staring at a computer screen when working late hours. As the day goes on, it continuously adjusts gamma, transitioning the display color temperature from cold blue in the afternoon to warm yellow during the night. Its primary objective is to match the color of the screen to the light sources of your surrounding environment - typically, sunlight during the day and artificial light during the night. LightBulb has minimal impact on performance and offers many customization options.

**If you have questions or issues, please check out the [wiki](https://github.com/Tyrrrz/LightBulb/wiki).

## Download

LightBulb provides an installer (`LightBulb-Installer.exe`), a portable distribution (`LightBulb.zip`), and is also available as a package on WinGet.

- Recommended: [Download `LightBulb-Installer.exe` from latest release](https://github.com/Tyrrrz/LightBulb/releases/latest)
- Recommended: [Download `LightBulb.zip` from latest release](https://github.com/Tyrrrz/LightBulb/releases/latest) [requires .NET runtime]
- [Download `LightBulb-Installer.exe` from latest CI build](https://github.com/Tyrrrz/LightBulb/actions?query=workflow%3ACI)
- [Download `LightBulb.zip` from latest CI build](https://github.com/Tyrrrz/LightBulb/actions?query=workflow%3ACI) [requires .NET runtime]
- [Install from WinGet](https://github.com/microsoft/winget-cli): `winget install Tyrrrz.LightBulb` [community-maintained]

**Important**: This application requires **.NET Core v3.1** runtime in order to run. Some download options come with the runtime pre-packaged, but those marked with `[requires .NET runtime]` do not. To install the runtime, find the suitable download option below:

- [Windows x64](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-3.1.10-windows-x64-installer)
- [Windows x86](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-3.1.10-windows-x86-installer)

Note: All download options are compatible with Windows 7 or higher. Other operating systems are not supported.

## Features

- Extensive customization options
- Automatically calculate sunrise & sunset times based on configured location
- Manually set desired sunrise & sunset times
- Smooth gamma transitions that give time for eyes to adjust
- Avoid changing gamma in games or other full-screen applications
- Application whitelist for color-sensitive applications
- Global hotkey to toggle the application from anywhere
- Absolutely minimal impact on performance
- Works without internet connection

## Screenshots

![dashboard](.screenshots/dashboard.png)
![settings](.screenshots/settings.png)
