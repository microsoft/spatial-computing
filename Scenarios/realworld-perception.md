+ **Name**: Real-World Perception
+ **Version**: 1.0

## Description

Using Artificial Intelligence (AI), Cognitive Services and other cloud services to understand and interact with a user's environment within a Mixed Reality (MR) experience. [Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/mixed-reality) encompasses the full spectrum between physical and digital realities, including Augmented Reality (AR) and Virtual Reality (VR). This scenario includes Environmental Understanding through AI services. The goal is to enable natural interactions between a user and an application running on any Mixed Reality device using voice and Computer Vision.

The scenario include 4 main areas:
- Speech Recognition (aka Speech-to-Text)
- Natural Language Understanding
- Speech Synthesis (aka Text-to-Speech)
- Computer Vision and Image Classification

## Application Examples

This scenario and the technical features associated with it are currently used by dev teams in the following cases:
- **Natural Voice Commands**: When creating an application for HoloLens, users can leverage keywords to issue voice commands in order to easily interact with a Mixed Reality application. Keywords are very useful but have limitations, can be quite "rigid" and need to be baked (i.e. hard-coded) in the application. The innovations and services used in this scenario allow the developer to rely on Natural Language Understanding to provide more fluid and adaptable voice commands in an application. A user could say: "Change the cube color to green" and your application will now what the user wants to change the color of a specific object to a specific color. You can use this for number of cases in any of your applications.
- **Computer Vision and Image Classification**: Image classification in Hololens application can be used to recognize specific domain object that the user is currently looking at to give specific instruction related to that object.
- **Better sounding, more natural voices**: The new Text-to-Speech service currently in Preview enables you to provide more natural voices to give feedback to users. These voices provide better consistency across client platforms, can speak in more languages without worrying about device-side "language packs", and sound less "robotic" than the on-board speech synthesizers commonly found on most mobile devices and headsets. 

## Targeted Platforms & Services

The current version of this scenario has been tested and proven out with various dev teams using :
- [Windows Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/), primarily on Microsoft HoloLens but this also covers immersive VR headsets.
- [LUIS.ai](LUIS.ai) for Natural Language Understanding.
- [DictationRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html) for Speech recognition (which is using Cognitive Services Speech under the hood).
- [customvision.ai](customvision.ai) for Computer Vision and Image Classification.
- [Cognitive Services Text-to-Speech](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/) for Speech Synthesis.

## Documentation and How-to Guides

To prove out this sceanario, our engineers spent time coding with a diverse set of different Mixed Reality dev teams. Below are all the resources that are useful if you want to try this and implement it in your own applications. These were tested multiple times but in case something is not working the way it should, don't hesitate to [create an issue](https://github.com/Microsoft/mixedreality-azure-samples/issues/new) in this repository.

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
