NHKey
=====

## What is it?

A Windows hotkey manager made with C# and WPF (.net 4.0). Overrides the window main procedure (WndProc) and uses WinAPI functions through P/Invoke (RegisterHotkey and UnregisterHotkey) to register the hotkeys globally.

## Installation
Just copy the Binary folder in an accesible folder and it's ready to be used.


----------------------------------------------

## Screenshots

##### Here is the main window, you can also drag and drop a program to the list and the hotkey creation window will be called with the program route.
![Main Window](Readme_Resources/hotkey_mainwindow.PNG?raw=true)
=======

##### You can add and edit hotkeys specifying the program and also add parameters:
![Create/Edit Hotkey Window](Readme_Resources/hotkey_creation_window.PNG?raw=true)
=======

##### Here is how the hotkey looks in the list. You can also execute the program by double clicking.
![Filled Main Window](Readme_Resources/filled_main_window.png?raw=true)
=======

##### And you can specify to start this hotkey manager with Windows (adds itself to windows registry) and make it start minimized in the notification area (right corner of taskbar).
![Options Window](Readme_Resources/hotkey_optionmenu.png?raw=true)
=======

##### New import/export of hotkeys function from upper menus.
![Import_Export_Menu](Readme_Resources/hotkey_menustrip_hotkeys.png?raw=true)
=======

##### The menu from Option menu now is also available as a drop down menu.
![Import_Export_Menu](Readme_Resources/hotkey_menustrip_options.png?raw=true)
=======


Currently available Spanish and English.
Feel free to pull request with language resource files for your language of choice. :)
