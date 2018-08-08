To use (samples coming soon):
1: Make an instance of the Options class, and set your options and keys

2: Get an image into a byte[] in jpg/png format

3: Make an instance of the Detector class, passing in your options object to the constructor

4: Make a method that takes in a PredictionResult object, which will serve as a callback function when a result from the prediction is ready
	Something like: public void PredictionCallback(PredictionResult result){}

5: Call detector.SendImage(yourByte[], result => YourCallbackFunction(result))

6: When a result is ready, your callback function will be called, and you will have a PredictionResult that you can do with whatever you want!

Now you have image classification in your app!