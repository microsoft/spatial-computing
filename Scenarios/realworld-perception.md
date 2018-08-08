+ **Name**: Real-World Perception
+ **Version**: 1.0

## Objective

The goal of this STE is to improve environmental understanding in [Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/mixed-reality) applications through the use of Artificial Intelligence, Cognitive Services and related cloud services. In this first release we focused on Windows Mixed Reality devices running a wide range of Augmented and Virtual experiences. The capabilities delivered in this iteration focus primarily on human-computer interaction and are provided in 4 key areas: 

- High Quality Speech Recognition (aka Speech-to-Text)
- High Quality Speech Synthesis (aka Text-to-Speech)
- Natural Language Understanding (understanding a users *intent*)
- Computer Vision and Image Classification

## Key Scenarios

The technical assets created for this STE are currently used by dev teams to enable the following scenarios:
- **Natural Voice Commands**: When creating HoloLens and Immersive applications, developers have traditionally leveraged keywords as a way of issuing voice commands in the application. Keywords are useful but have limitations such as being "rigid" and needing to be baked (i.e. hard-coded) into the application. The services and assets created for this scenario allow developers to leverage Natural Language Understanding for more fluid and adaptable voice commands. For example, users might say: "Change the cube to green" or "Make the box blue" and the application will  understand which object and which color in both cases. This scenario is extensible and can be leveraged for any number of commands.
- **Computer Vision and Image Classification**: Image classification can be leveraged in Hololens applications to recognize known objects that the user may be looking at. This can be helpful to identify the users surroundings or to provide instructions about a known object.
- **Better sounding, more natural voices**: The new Text-to-Speech services (currently in Preview) provide far more natural sounding voices for interacting with users. Leveraging these voices not only provide a more consistent user experience across platforms, but it also allows for many spoken languages without depending on device-side "language packs". 

## Targeted Platforms & Services

The current version has been validated on the following devices and platforms:
- [Windows Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/), primarily on Microsoft HoloLens but this also covers immersive VR headsets.
- [LUIS.ai](LUIS.ai) for Natural Language Understanding.
- [DictationRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html) for Speech recognition (which is using Cognitive Services Speech under the hood).
- [customvision.ai](customvision.ai) for Computer Vision and Image Classification.
- [Cognitive Services Text-to-Speech](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/) for Speech Synthesis.

## Documentation and How-to Guides

These assets were built and tested by a diverse set of Mixed Reality teams both internal and external to Microsoft. We hope you find the resources below helpful in implementing any of the scenarios outlined above. If you encounter any problems attempting to leverage any of these resources, please don't hesitate to [create an issue](https://github.com/Microsoft/mixedreality-azure-samples/issues/new) or [open a pull request](open a pull request "https://github.com/Microsoft/mixedreality-azure-samples/pulls").

- Natural Language Understanding using LUIS: 
    - [LUIS.ai website](LUIS.ai)
    - [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/home)
    - [LUIS for XR reference architecture](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Reference-Architecture/Client/MixedReality-Azure-Unity/Assets/MixedRealityAzure/LUIS)
    - [Why Luis in XR](http://www.roadtomr.com/2018/05/08/2555/nlu-for-xr-with-luis/)

- LUIS Caching Service solution:
    - *Note: this is a ready-to-deploy solution that you can quickly publish to the Microsoft Azure cloud platform to cache queries made with the LUIS service*
    - [LUIS Caching service code](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Solutions/LUIS-CachingService) (includes a **Deploy to Azure** button you can use to automatically configure the solution in your Azure subscription)
    
- Custom Vision :
    - [CustomVision.ai website](customvision.ai)
    - [Custom Vision documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/home)
    - [Academy lab 302b - Custom vision in Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/mr-azure-302b) (using unity web request, better integration coming soon)
    
- Cognitive Services Text-to-Speech :
    - [Text-to-Speech website](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/)
    - [Text-to-Speech documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/text-to-speech)
    - [Text-to-Speech implementation example in Unity](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Standalone-Samples/Unity-Text-to-Speech)
