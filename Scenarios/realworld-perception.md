+ **Name**: Realworld perception
+ **Version**: 1.0

## Description

Using AI and Cognitive Services to understand and interact with a user's environment within a Mixed Reality experience. This includes Environmental Understanding through AI services. The goal is to enable natural interactions with the application and device through voice or image recognition.

The scenario include 4 main areas:
- Speech Recognition
- Natural Language Understanding
- Vision and image classification
- Text to Speech

## Application examples

This scenario and the technical features associated to it are currently used by dev teams in the following cases:
- **Better Hololens vocal commands**: When creating an application for Hololens, you have access to keywords. Using keywords you can add vocal commands so users can easily interact with your application. These are great but have limits and are not dynamic. The tech and services used in this scenario enable you to use the power of Natural Language understanding to add dynamics vocal commands to you application. A user could say : "Change the cube color to green" and your application will now what the user wants to change the color of a specific object to a specific color. You can use this for number of cases in any of your applications.
- **Image classification**: Image classification in Hololens application can be used to recognize specific domain object that the user is currently looking at to give specific instruction related to that object.
- **Better and natural voice**: The new text to speech service currently in Preview enables you to provide natural voices to give feedback to users

## Targeted platforms

The current version of this scenario has been tested and proven out with various dev teams using :
- Windows Mixed Reality (Mainly Hololens)
- [LUIS.ai](LUIS.ai) for Natural Language Understanding
- [DictationRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html) for Speech recognition (which is using Cognitive Services Speech under the hood)
- [customvision.ai](customvision.ai) for Image classification
- [Cognitive Services Text to Speech](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/)

## Documentation and how to

To prove out this sceanario, we coded with a lot of different teams. Here are all the resources that are useful if you want to try this and implement it in your application. These were tested multiple times but in case something is not working the way it should, don't hesitate to [create an issue](https://github.com/Microsoft/mixedreality-azure-samples/issues/new) in this repository.

- Natural Language Understanding using LUIS: 
    - [LUIS.ai website](LUIS.ai)
    - [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/home)
    - [LUIS for XR reference architecture](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Reference-Architecture/Client/MixedReality-Azure-Unity/Assets/MixedRealityAzure/LUIS)
    - [Why Luis in XR](http://www.roadtomr.com/2018/05/08/2555/nlu-for-xr-with-luis/)

- LUIS Caching service solution:
    - *Note: this is ready to deploy solution that you can deploy in on the Microsoft Azure platform to cache queries to the LUIS service*
    - [LUIS Caching service code](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Solutions/LUIS-CachingService) (include a **Deploy to Azure** button you can use to automatically configure the solution in your subscription)
    
- Custom Vision :
    - [CustomVision.ai website](customvision.ai)
    - [Custom Vision documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/home)
    - [Academy lab 302b - Custom vision in Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/mr-azure-302b) (using unity web request, better integration coming soon)
    
- Cognitive Services Text to Speech :
    - [Text to Speech website](https://azure.microsoft.com/en-us/services/cognitive-services/text-to-speech/)
    - [Text to Speech documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/text-to-speech)
    - [Text to Speech implementation example in Unity](https://github.com/Microsoft/mixedreality-azure-samples/tree/master/Standalone-Samples/Unity-Text-to-Speech)