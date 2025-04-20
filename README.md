Watchdog
====================
[![License](https://img.shields.io/badge/license-MIT%20License-blue.svg)](http://doge.mit-license.org)

*CSharp Watchdog application. It will monitor applications and restart if necessary.*

Watchdog is an application that can monitor as many applications as you want from the system tray. If an application exits it can be restarted. The watchdog is very configurable.

The watchdog also comes with a single client library that you can integrate in your own application. It implements a heartbeat system. This means if the application is not sending a heartbeat anymore, the Watchdog can restart on this.

# Overview
## System tray
The application lives in the system tray, where it can be enabled, disabled and configured. The application can be configured through Settings and stopped or exited. Note that if your exit the watchdog it may automatically respawn, based on the general settings.  
![Watchdog in system tray](/Screenshots/TaskbarMenu.png)

## Selecting applications to be watched
The application allows watching as many applications as you need.
The Monitored Application demonstrates different manners in which an application can exit, crash or freeze.   
![Watchdog application selection](/Screenshots/ConfigurationForm.png)

## Watchdog settings 
![Watchdog application settings](/Screenshots/ApplicationSettingsMenu.png)  
Multiple watchdog parameters can be modified per application  
* Path         - The path of the application. The working directory is also based on this path
* Process name - Name of the process when running. This is often the application name, but not always. It is used to monitor if the application (and how many) is running. 
* Use Heartbeat  - This refers to the heartbeat library you can implement in your own application
* Max interval heartbeats - Maximum time between hearbeats. If more time occurs between two heartbeats, the watchdog wil restart. Make sure your application sends a heartbeat more often (at least a factor 2)
* Grant kill request - Enables the option to grant a kill request from the client
* Max unresponsive interval - Maximum time that the application may be unresponsive (The application always adds 5 seconds to the unresponsive interval).
* Startup monitor delay - the time between starting an application and the first time that polling occurs. It may take an application some time to start properly and become responsive
* Active / in active - monitoring starts when the application is set to "Active" (and the watchdog is running)

## Copyright

Watchdog is provided under MIT License.  Copyright © 2018, 2019, 2020
