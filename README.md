<p align="center"> <img src="icon.png" alt="DynamicZoom icon" width="200"/> </p> 
<h1> <p align="center" > DynamicZoom </p> </h1> 

 For more information or release downloads, [check the Thunderstore page.](https://thunderstore.io/c/bomb-rush-cyberfunk/p/goatgirl/DynamicZoom)
## Features
- **Dynamic Zoom**: Impacts the distance from the camera to the player
    - Forces a consistent distance at all angles
- **Dynamic Drag**: Impacts how far behind the camera can drag before catching up with the player
    - If the player moves towards the camera, the camera can still get close up
    - Occasionally ignored by some camera modes
    - Static camera drag settings can be applied as well
- Miscellaneous camera tweaks available to configure
## Building from Source
This plugin requires the following .dlls to be placed in the \lib\ folder to be built:
- A [publicized](https://github.com/CabbageCrow/AssemblyPublicizer) version of the game's code, from BRC's Data folder (Assembly-CSharp.dll)
- 0Harmony.dll and BepInEx.dll from \BepInEx\core

With these files, run "dotnet build" in the project's root folder (same directory as DynamicZoom.csproj) and the .dll will be in the \bin\ folder. 

## Credits
Huge thanks to **LunaCapra** for inspiration and code reference with her DynamicCamera mod
