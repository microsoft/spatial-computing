# LUIS Reference for Mixed Reality Applications

This project serves as a reference for integrating the [Language Understanding Intelligence Service](https://www.luis.ai/home) (LUIS) into Mixed Reality applications. Though **Mixed Reality** is in the title, this reference is not exclusive to Windows Mixed Reality devices.


- [Objective](#objective)
- [Minimum requirements](#minimum-requirements)
- [Running the demo scene](#running-the-demo-scene)
- [Adding these components to your project](#setting-up-components)
- [What's next](#next-steps)
 

## Objective
It's quite easy to leverage voice commands in Unity through the [KeywordRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer.html) class. Voice commands are very powerful, but they also present their own set of challenges:

- **Voice commands don't have context** - They work great for global commands like "Save", but targeting voice commands at an object in the scene requires additional code. The [MRTK](https://github.com/Microsoft/MixedRealityToolkit-Unity) attempts to help with this through the [ISpeechHandler](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/Scripts/InputHandlers/ISpeechHandler.cs) interface. This interface allows voice commands to be routed to whichever object the user is gazing at.

- **Voice commands are explicit** - Being able to say "move the box" and "move the cube" requires two different voice commands (even though "box" and "cube" may mean the same object). Similarly, "move the box left" and "move the box right" require two different voice commands just to support those two directions. As you can imagine, the number of objects in the scene and the ways you can interact with them exponentially increases the commands required to support them. 

- 

and dictation as input as part of the MR Toolkit, there is no language understanding as part of that input out of the box. By adding support for LUIS in Unity you can use natural language to control Unity game objects. This project was initially developed during an internal Microsoft Mixed Reality hack.

## Minimum Requirements
- [Unity MRTP5](http://beta.unity3d.com/download/a07ad30bae31/download.html)
- Enable `.NET 4.6 (experimental)` under player settings
- Enable `.NET backend` under player settings

## Running the Demo Scene ##
1. Set up a [LUIS account](https://www.luis.ai/home). Under 'My Apps', select 'Import App' and choose the LuisApp.json file from this repository. Make sure to train the model and publish it to get the App ID and App Key (which you'll need later).  
2. In Unity, open the LUIS folder in this repository as your project. Click on the LUIS-Sample scene in Assets to open the scene. 
3. Click on `LUISDictationManager` in the Hierarchy and enter your [App ID and App Key](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/manage-keys) for the LUIS model you set up in the first step. 
4. Go to the File menu -> Build Settings -> Add Open Scenes. Change the platform to Universal Windows Platform and click 'Switch Platform'. Make sure the build type is D3D and Unity C# Projects is checked. 
5. Click Build, create a separate folder called 'App' and select that folder. 
6. After it Builds, go into your 'App' folder and open the .sln file in Visual Studio. In Visual Studio on the second top toolbar, change the drop down menus from 'Debug' to 'Release', 'ARM' to 'x64', and deploy to 'Local Machine'. You'll want to deploy without debugging, which you can do in Debug -> Start Without Debugging. 
7. Make sure your headset is plugged in and you have at least one motion controller. Click on the Record Button, allow the app to use your microphone, and say a command into your mic. 
Commands include: 
- change the box/ball to \*color\*
- make the box/ball big/medium/small 


## Setting Up Components ##
Here's what you need to do in order to use these tools in your project. 
### UNITY ###
- In Assets -> LUISAssets-> Prefabs, add the `LUISDictationManager` prefab to your scene. After you complete the LUIS steps below, make sure to add your App ID and App Key to `LUISDictationManager`. 
- Make sure you have some way to start/stop dictation. In the sample application, we used a "Record Button" that called `StartRecordingDictation()` and `StopRecordingDictation()` from the `DictationToLUIS` script. 
### LUIS ###
> Note: When training the LUIS model for Unity we reserved one set of entities for object identification and selection. Then we added additional Entity sets for altering the game object's properties (such as color or size).

In order to implement in Unity:  
- Attach the `LuisBehavior` script onto the Unity GameObject you wish to control.
- Enter the Entity type and Entity name used for object identification purposes. eg. The Entity type might be "Shape" and the Entity name id "box".


## Next Steps ##
These are potential ideas for improving this project.  
- Implement raycasting to determine what the player is looking in addition to explicitly stating which GameObject to affect. 
- Create an easy to use UI for `LUISBehavior` so you don't have to hook everything up on the script.  
- Auto import LUIS Intents and Entity catagories so users don't have to explicitly use strings on the `LUISBehavior` script. 
- Investigate use of ScriptableoOjects to hold LUIS behaviors. 
