# Version 2.0
## Features
- [Log] Add support for `Log Channel`
- [Command] Add support for dropdown control with string, int and float
- [Command] Add new `Search Game Object` command that you can search for game object in scene via name/tag/component and inspect it
- [Command] Add new `PlayerPref Inspector` command that show all player preferences. Support for Mac & Window editor, iOS and Android Runntime
- [Command] Add new `Persistent Data Inspector` command that show all files under persistent data folder and inspect it
- [Command] Command: `OnVariableValueLoaded` and `OnValueChanged(string varName)` have been deprecated and will be removed. Added new `[Variable]` attribute as a replacement
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