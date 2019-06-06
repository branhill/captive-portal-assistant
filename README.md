# Captive Portal Assistant

> Automatic Wi-Fi captive portal login app for Windows 10

This app instead of the web browser to handle captive portal login. When you connect to a public Wi-Fi hotspot that using captive portal, the system will launch this app. If the login form saved for the hotspot, it will auto login without any manual operation.

## How does it works

After Windows 10 Creators Update (Build 15063), the system using an exclusive domain (www.msftconnecttest.com) for captive portal login flow. So we can use UWP Apps for Websites feature to associates it.

Unfortunately, website ownership validation required for the associated domain, so this app cannot be published to Microsoft Store, you need to build it from source.

## Prerequisites

* Windows 10 Creators Update (Build 15063) or higher
* Visual Studio 2019 with Universal Windows Platform development workload and Windows 10 SDK 18362

## Installing

You can build it in Visual Studio.

## License

This project is licensed under the [MIT License](LICENSE)

Copyright (c) 2018 [Brandon Hill](https://branhill.com/)
