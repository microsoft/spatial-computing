# LUIS for XR

LUIS for XR is a reference implementation for Natural Language Understanding in XR applications using [LUIS](https://www.luis.ai/home).

- [Why Natural Language](#why-natural-language)
- [Getting Started](#getting-started)
- [System Requirements](#system-requirements)
- [Special Thanks](#special-thanks)
 

## Why Natural Language

For an overview of Natural Language and why it's more powerful than traditional voice commands, please see:

[Natural Language for Simulations](http://www.roadtomr.com/2018/05/08/2508/natural-language-for-simulations)

[![](http://www.roadtomr.com/wp-content/uploads/2018/05/NLSimFeature.png)](http://www.roadtomr.com/2018/05/08/2508/natural-language-for-simulations)


## Getting Started

This reference is easy to use and is distributed as a [Unity Package](http://aka.ms/mrluispack). For a complete step-by-step guide on how to get started using it in your own applications, please see the article:

[NLU For XR With LUIS](http://www.roadtomr.com/2018/05/08/2555/nlu-for-xr-with-luis)

[![](http://www.roadtomr.com/wp-content/uploads/2018/05/LuisXRFeat.png)](http://www.roadtomr.com/2018/05/08/2555/nlu-for-xr-with-luis)


## System Requirements
- Unity 2017.2 or later
- `.NET 4.6` under player settings
- `.NET backend` under player settings

Though you will see "Mixed Reality" mentioned throughout this reference, it is not exclusive to Windows Mixed Reality devices. This code will work with all Unity projects that support .NET 4.6. The only exception to this rule is the **Luis Dictation Manager** which needs Windows 10 APIs to access the microphone. This component is optional and will work on all devices that run Windows 10, including HoloLens.


## Special Thanks ##
The [earliest prototype](https://github.com/ashanhol/LUISMR) of this project was developed during an internal Microsoft hack in December 2017. That team consisted of [Adina Shanholtz](http://adinashanholtz.com), [Anna Fear](https://www.linkedin.com/in/avfear) and [David Douglas](http://www.deadlyfingers.net). The Reference Architecture here evolved out that prototype during a Microsoft + Partner hack in February 2018. The primary contributors to this reference included [Jared Bienz](https://www.linkedin.com/in/jbienz) from [Microsoft](http://www.microsoft.com), [Michael House](https://www.linkedin.com/in/housemichael) from [Object Theory](http://objecttheory.com) and [Stephen Hodgson](https://www.linkedin.com/in/stephenjhodgson) from [Valorem](https://www.valorem.com).
