using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.MR.Face;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FaceHeatMapTest : MonoBehaviour
{
	[SerializeField] private FaceHeatMap faceHeatMap;
	[SerializeField] private LocalImageRequester imageRequester;
	[SerializeField] private FaceManager faceManager;
	[SerializeField] RawImage imageOnScreen;
	[SerializeField] Button applyFilterButton;

	Coroutine imageRequestRoutine;
	Texture2D image;

	public void RequestNewImage()
	{
		if (imageRequestRoutine != null)
		{
			StopCoroutine(imageRequestRoutine);
		}

		imageRequestRoutine = StartCoroutine(SetupImage());
	}

	public async void ApplyFilterHackAsync()
	{
		if(image == null)
		{
			Debug.Log("Image not ready, please wait.");
			return;
		}

		Texture2D heatMap = await faceHeatMap.AnalyzeImage(image, FaceAttributeType.Age);
		imageOnScreen.texture = heatMap;
		imageOnScreen.SetNativeSize();
	}

	private IEnumerator SetupImage()
	{
		image = null;
		applyFilterButton.interactable = false;

		yield return StartCoroutine(imageRequester.RequestImage("https://source.unsplash.com/800x600/?people,faces")); ;

		image = imageRequester.GetImage();

		ShowImage();
	}

	private void ShowImage()
	{
		if (image == null)
		{
			Debug.Log("Image is null, please wait.");
			return;
		}

		imageOnScreen.texture = image;
		imageOnScreen.SetNativeSize();
		applyFilterButton.interactable = true;
	}
}
