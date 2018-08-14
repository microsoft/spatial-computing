To use (samples coming soon):
0: Add VisionManager.cs, PredictionResult.cs, and Prediction.cs to your Assets folder

1: Attach VisionManager.cs to a GameObject in the scene.

2: Set your options in VisionManager in the Unity Inspector.

3: Get a reference to VisionManager with something like:
VisionManager visionManager = GetComponent<VisionManager>();

4: Get an image as a byte[] formatted as a png, jpg, or any other format accepted by CustomVision/ComputerVision

5: Send your image for prediction and get the result by calling:
PredictionResult result = await visionManager.SendImageAsync(imageData); //imageData is the byte[] here

6: Do whatever you like with the PredictionResult.

Now you have image classification in your app!