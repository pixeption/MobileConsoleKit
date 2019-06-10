# v2.0.3
## Improvements
- [Share Log] When sharing all log, it will be written to a file in persistent folder and shared as a file. This will help to avoid 1 MB litmit on Android when sharing text. On Editor, you can open the log folder in `Tools > Mobile Console > Open Log Folder` 
- [Editor] Suppress all warning 0649 (variable is never assigned)

## Fixes
- [Log] Channel regex wasn't correct in some cases
- [Command] There is now a message that states `PlayerPrefsCommand` isn't supported on Window with .NET Standard 2.0.
- [Native Helper] Rename some native functions to avoid conflict with Unity 2019


# v2.0.2
## Features
- [Android] Add support for Android Back button, you can now back and close views
- [Command] Get a command instance by its type `LogConsole.GetCommand`
- [View] Add `LogConsole.OnVisibilityChanged` event

## Fixes
- [Command] Press the Action button causes exception
- [View] ScrollView doesn't save scroll position position correctly
- [View] A pixel off between top bar and center view on some devices
- [Command] Put unsupported public field type (Vector2, ...) in command causes exception and the tool stop working

## Improvements
- [Command] `OnVariableValueLoaded` and `OnValueChanged(string varName)` functions no longer deprecated. You'll decide what to use
- [Command/Dropdown] Improve the way long string values are displayed in dropdown cell
- [Command/Dropdown] Assign default value to dropdown field if it hasn't been initialized yet
- [Command] Warning about unsupported field type in Command


# v2.0.1
## Fixes
- [Native Helper] A small typo causes error when building iOS
- [Share Log] Share log on Android didn't open Chooser on subsequent sharing

## Improvemments
- [Editor] Enable Mobile Console Kit also enables development build
- [Command/App and device info] Add more useful infos including: CPU, memory, GPU, screen size


# v2.0
## Features
- [Log] Add support for `Log Channel`
- [Command] Add support for dropdown control with string, int and float
- [Command] Add new `Search Game Object` command that you can search for game object in scene via name/tag/component and inspect it
- [Command] Add new `PlayerPref Inspector` command that show all player preferences. Support for Mac & Window editor, iOS and Android Runntime
- [Command] Add new `Persistent Data Inspector` command that show all files under persistent data folder and inspect it
- [Command] `OnVariableValueLoaded` and `OnValueChanged(string varName)` have been deprecated and will be removed. Added new `[Variable]` attribute as a replacement
- [Command] Add support for dynamic button in Command via `[Button]` attribute
- [View] The tool UI has been rewritten as `ViewBuilder` to support dynamic UI so you can create your own UI easily.
- [Setting] The console background transparency can be controlled in `Setting/Console/Background Transparency`
- [Setting] Add timescale setting to `Setting/Console/Time Scale`

## Improvemments
- [Sharing log] now includes App & Device information
- [View] Add color for command group for easily navigation
- [View] Command Group expand/collapse state now is saved between sessions
- [View] Add `Expand/Collapse All` to all the view that has group

## Fixes
- [Command View] Command sorting is correctly now
- [Command] Application.BundleIdentifier is not exist on Unity 5.6 and newer
- [Command/Dropdown] An exception will be thrown if selected enum value was changed or removed
- [Command/Input] An exception will be thrown if input is empty string and field type is numeric
- [Command/Input] An exception will be thrown if typing a float number to field type Int
- [Command Group] Group doesn't expand/collapse correctly when the tree has more than 2 levels


# Version 1.0
Initial release
