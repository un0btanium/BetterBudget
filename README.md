# BetterBudget
An UI mod for the Cities Skylines game providing more convenient way to access to the budget sliders.


## Steam Workshop
You can find the Steam Workshop mod entry [here](http://steamcommunity.com/sharedfiles/filedetails/?id=420972688).

## Modding Contribution Guide
If you want to contribute, fork and pull the repository. I used Visual Studio to maintain the project. You still have to add (or change the path of) some DLL references manually. The DLL files are located under your Cities Skylines installation folder: *[SteamLibrary]\SteamApps\common\Cities_Skylines\Cities_Data\Managed*

I selected the following DLLs:
* Assembly-CSharp.dll
* ColossalManaged.dll
* ICities.dll
* UnityEngine.dll
* UnityEngine.UI.dll

The project should already come with a post-build event which copies the created DLL to the Local AppData mod folder. You don't need to restart the game after you recompile the project. Just (re)load the savefile.