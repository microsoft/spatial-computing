# LUIS Reference for Mixed Reality Applications

This project serves as a reference for integrating the [Language Understanding Intelligence Service](https://www.luis.ai/home) (LUIS) into Mixed Reality applications. Though **Mixed Reality** is in the title, this reference is not exclusive to Windows Mixed Reality devices.


- [Objective](#objective)
- [Minimum requirements](#minimum-requirements)
- [Running the demo scene](#running-the-demo-scene)
- [Adding these components to your project](#setting-up-components)
- [What's next](#next-steps)
 

## Objective

### Challenges with Voice Commands
It's quite easy to leverage voice commands in Unity through the [KeywordRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer.html) class. Voice commands are very powerful, but they also present their own set of challenges:

- **Voice commands don't have context** - They work great for global commands like "Save", but targeting voice commands at an object in the scene requires additional code. The [MRTK](https://github.com/Microsoft/MixedRealityToolkit-Unity) attempts to help with this through the [ISpeechHandler](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/Scripts/InputHandlers/ISpeechHandler.cs) interface. This interface allows voice commands to be routed to whichever object the user is gazing at, but gaze is currently the only way to route or target voice commands. 

- **Voice commands are explicit** - Being able to say "move the box" and "move the cube" requires two different voice commands (even though "box" and "cube" may mean the same object). Similarly, "move the box left" and "move the box right" require two different voice commands just to support those two directions. As you can imagine, the number of objects in the scene and the ways you can interact with them exponentially increases the commands required to support them. 

### How Natural Language Solves These Problems
By adding support for LUIS in Unity you can use natural language to control Unity game objects. 

TODO: More background 


## Minimum Requirements
- Unity 2017.2+
- Enable `.NET 4.6 (experimental)` under player settings
- Enable `.NET backend` under player settings

## Running the Demo Scene ##
The primary demo scene is **MixedRealityAzure-Examples\LUIS\Scenes\LUISTest.unity**. You will need to provide your AppID and AppKey on the LuisManager component (which is on the LUIS GameObject). It is safe to ignore the warning about missing values when the scene starts. This is a bug we need to address. 

TODO: More info about the demo scene

## Setting Up Components ##
TODO
 
### UNITY ###
TODO

### LUIS ###
TODO

## Next Steps ##
TODO


## Special Thanks ##
The [earliest prototype](https://github.com/ashanhol/LUISMR) of this project was developed during an internal Microsoft Mixed Reality hack in December of 2017. That team consisted of [Adina Shanholtz](http://adinashanholtz.com), [Anna Fear](https://www.linkedin.com/in/avfear) and  [David Douglas](http://www.deadlyfingers.net).

This reference architecture evolve from that prototype during a Microsoft + Partner hack in February 2018. The primary contributors included [Jared Bienz](http://www.roadtomr.com) (Microsoft), [Michael House](https://www.linkedin.com/in/housemichael) (Object Theory) and [Stephen Hodgson](https://www.linkedin.com/in/stephenjhodgson) (Valorem).