<h1>Vision Treasure Hunt!</h1>
<h3>A demo Unity project that uses the MR Vision package. Works on Hololens, Android, and iOS.</h3>
This demo project is meant to show how to get camera images and send them using the VisionObject, and also to show how handle results to update your virtual objects.
</br>

Search with your device's camera for treasure! The treasure is really any class from a Custom Vision project. In my example the treasure is a coffee cup.
When you find one, cubes will launch from your location as a celebration.


<h2>How To Use</h2>
1: Clone this repo, open the project in Unity (this README's parent directory), and open the scene called "TreasureHuntDemo"

2: Make a Custom Vision project, train it with a class, and get your Prediction URL, Prediction Key, and Project ID.

3: Attached to the VisionManager GameObject that's in the scene, there is a VisionManager component. Set your Prediction URL, Prediction Key, and Project ID on this component in the inspector window.

4: Build and run! The player settings for Hololens, Android, and iOS are already set up, so you should not need to do anything else. The camera on your device will begin taking pictures which will be sent to your Custom Vision project for prediction. The resulting PredictionResult is used in DemoDriver.cs to determine whether or not you've found your treasure (whether or not it recognizes any of your trained classes in the image)!

If you have found something, you'll get an explosion of red cubes around where you are standing!


Poke around this project to see how you can make other AR applications using Vision!
