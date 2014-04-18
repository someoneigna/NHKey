NHKey
=====

## What is it?

A Windows hotkey manager made with C# and WPF (.net 4.0). Overrides the window main procedure (WndProc) and uses WinAPI functions through P/Invoke (RegisterHotkey and UnregisterHotkey) to register the hotkeys globally.

## Installation
Just copy the binary in an accesible folder and is ready to be used.
I will provide a installer in the future (I plan to also write one >_<).

----------------------------------------------

## Screenshots

##### Here is the main window, you can also drag and drop a program to the list and the hotkey creation window will be called with the program route.
![Main Window](Readme_Resources/main_window.jpg?raw=true)
=======

##### You can add and edit hotkeys specifying the program and also add parameters:
![Create/Edit Hotkey Window](Readme_Resources/hotkey_creation_window.jpg?raw=true)
=======

##### Here is how the hotkey looks in the list. You can also execute the program by double clicking.
![Filled Main Window](Readme_Resources/filled_main_window.jpg?raw=true)
=======

##### And you can specify to start this hotkey manager with Windows (adds itself to windows registry) and make it start minimized in the notification area (right corner of taskbar).
![Options Window](Readme_Resources/options_window.jpg?raw=true)
======


Will soon add the option to change languages.
