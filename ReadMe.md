# LightBulb 💡

[![Build](https://github.com/Tyrrrz/LightBulb/workflows/CI/badge.svg?branch=master)](https://github.com/Tyrrrz/LightBulb/actions)
[![Release](https://img.shields.io/github/release/Tyrrrz/LightBulb.svg)](https://github.com/Tyrrrz/LightBulb/releases)
[![Downloads](https://img.shields.io/github/downloads/Tyrrrz/LightBulb/total.svg)](https://github.com/Tyrrrz/LightBulb/releases)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)

**Project status: maintenance mode** (bug fixes only).

LightBulb is an application that reduces eyestrain produced by staring at a computer screen when working late hours. As the day goes on, it continuously adjusts gamma, transitioning the display color temperature from cold blue in the afternoon to warm yellow during the night. Its primary objective is to match the color of the screen to the light sources of your surrounding environment - typically, sunlight during the day and artificial light during the night. LightBulb has minimal impact on performance and offers many customization options.

Have questions or need help? Check out the [wiki](https://github.com/Tyrrrz/LightBulb/wiki).

## Download

- **[Latest release](https://github.com/Tyrrrz/LightBulb/releases/latest)**
- [WinGet](https://github.com/microsoft/winget-cli): `winget install Tyrrrz.LightBulb`
- [CI build](https://github.com/Tyrrrz/LightBulb/actions)

Note: This application requires .NET v3.1 Desktop Runtime, which you can [download here](https://dotnet.microsoft.com/download/dotnet/3.1/runtime). The required dependency will be installed automatically, unless you're using the portable distribution.

Supported operating system: Windows 7 or higher.

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
