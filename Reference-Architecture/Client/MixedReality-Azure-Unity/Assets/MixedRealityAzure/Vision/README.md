<h1>Computer Vision and Custom Vision for XR</h1>
<h3>Add image classification and object recognition to your AR application!</h3>

<h2>How To Use</h2>
1: Add the Unity Package "MR-Vision-x.x.x" to your Unity project.
  </br> You'll find that in this Github Repository under MixedReality-Azure-Unity/UnityPackages

2: Add the VisionObject prefab to your scene.
   </br>That's under Assets/MixedRealityAzure/Vision after you've added the package.

3: In the inspector for your scene's VisionObject, set your project information for Custom Vision, Computer Vision, or both!

4: Get a reference to the VisionManager script component with something like:
    </br>VisionManager visionManager = GetComponent<VisionManager>();

5: Get an image as a byte[] formatted as a .png, .jpg, .bmp, or any other format accepted by CustomVision/ComputerVision.
    </br> There are examples of how to do this using Hololens, using ARKit on iOS, or using ARCore for Android, in the Standalone Sample ""

6: Send your image for prediction and get the result by calling:
    </br>PredictionResult result = await visionManager.SendImageAsync(imageData); //imageData is the byte[] in this example

7: Do whatever you like with the PredictionResult.
    </br>It contains a list of Predictions for the image, where each Prediction has a name, confidence, and bounding box information if available.

Now you have image classification in your app!