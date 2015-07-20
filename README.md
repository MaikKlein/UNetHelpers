#Multistart(Windows only)

##Important
WindowTitle needs to be set to your window title. The window title is your product name wich can be found and set in 'File -> BuildSettings -> PlayerSettings'

Your standalone build should start in window mode.

Uncheck PlayerSettings->DefaultIsFullScreen

Your should also disable the unity start up dialog.

Select disable in PlayerSettings->DisplayResoultionDialog

##Scene
The scene that you want to build

##Width / Height
The resoultion of your monitor.

##Timout
How long it should look for the windows.

##Horizonal Offset
The horizonal offset will control how far the windows are spawned from 0,0.

For example if you have a resoultion of 1920x1080 and you set your horizonal offset to 1920, the windows will spawn on your second monitor.

##Relative Path
The folder where you want to save the build. The path is relative to your project root folder.

#AutoConnect
The AutoConnect script will automatically start the server in the editor and start a client in the standalone build.

Just put it on your NetworkManager.