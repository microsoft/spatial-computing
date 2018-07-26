using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MR.Face
{
	public class FaceHeatMap : MonoBehaviour
	{
		[SerializeField] FaceManager faceManager;

		[Tooltip("Enable for higher clarity, needs to re-analyze the image")]
		[SerializeField] bool highContrast;
		[SerializeField] [Range(3.0f, 0.2f)] float highContrastLevel;

		// Filters. Definately not the cleanest solution to this 
		// problem, but alas it's difficult to have dictionaries 
		// in Unity editor. filtersDictionary is basically all the
		// stand-alone filters keyed on the FaceAttribute.
		[Tooltip("Gender specific filter. Applies a color box based on the gender")]
		[SerializeField] GenderFilter genderFilter;
		[Tooltip("Age specific filter. Applies a color box based on the age you have specified")]
		[SerializeField] AgeFilter ageFilter;
		Dictionary<FaceAttributeType, IFilter> filtersDictionary;
		Texture2D lastImage;

		private void Start()
		{
			filtersDictionary = new Dictionary<FaceAttributeType, IFilter>();
			filtersDictionary.Add(FaceAttributeType.Gender, genderFilter);
			filtersDictionary.Add(FaceAttributeType.Age, ageFilter);
		}

		public async Task<Texture2D> AnalyzeImage(Texture2D image, FaceAttributeType faceAttributeType)
		{
			lastImage = image;

			Texture2D heatMapTexture = new Texture2D(image.width, image.height);

			Color[] c = image.GetPixels();
			heatMapTexture.SetPixels(c);
			if (highContrast)
			{
				for (int i = 0; i < heatMapTexture.width; ++i)
				{
					for (int j = 0; j < heatMapTexture.height; ++j)
					{
						Color color = heatMapTexture.GetPixel(i, j) * heatMapTexture.GetPixel(i, j).grayscale * highContrastLevel;
						heatMapTexture.SetPixel(i, j, color);
					}
				}
			}

			byte[] bytes = image.EncodeToPNG();
			using (Stream stream = new MemoryStream(bytes))
			{
				IList<FaceAttributeType> faceAttributes =
				new FaceAttributeType[]
				{
					faceAttributeType
				};

				IList<DetectedFace> faces = await faceManager.Detect(stream, faceAttributes);

				if (filtersDictionary.ContainsKey(faceAttributeType))
				{
					filtersDictionary[faceAttributeType].Apply(faces, image, heatMapTexture);
				}
				else
				{
					Debug.Log("Error, face attribute filter not found: " + faceAttributeType.ToString());
				}
			}

			return heatMapTexture;
		}

		interface IFilter
		{
			void Apply(IList<DetectedFace> faces, Texture2D originalImage, Texture2D image);
		}

		[System.Serializable]
		struct GenderFilter : IFilter
		{
			[Tooltip("Box color for male faces")]
			public Color male;
			[Tooltip("Box color for female faces")]
			public Color female;
			[Tooltip("Box color for genderless faces")]
			public Color genderless;

			public void Apply(IList<DetectedFace> faces, Texture2D originalImage, Texture2D image)
			{
				Debug.Log("FaceCount: " + faces.Count);
				foreach (var face in faces)
				{
					FaceRectangle faceRectangle = face.FaceRectangle;

					Color boxColor = male;
					if (face.FaceAttributes.Gender == Gender.Female)
					{
						boxColor = female;
					}
					else if (face.FaceAttributes.Gender == Gender.Genderless)
					{
						boxColor = genderless;
					}

					int w = faceRectangle.Width;
					int h = faceRectangle.Height;
					for (int i = 0; i < w; ++i)
					{
						for (int j = 0; j < h; ++j)
						{
							var posX = faceRectangle.Left + i;
							var posY = image.height - (faceRectangle.Top + j);
							image.SetPixel(posX, posY, boxColor * originalImage.GetPixel(posX, posY));
						}
					}

					image.Apply();
				}
			}
		}

		[System.Serializable]
		struct AgeFilter : IFilter
		{
			[Tooltip("A list of ages. Specify the age breaks here, for example, 10, 20, 30, means a) people under 10, b) people under 20 but older than 10 and c) people under 30 but older than 20.")]
			[SerializeField] List<int> ages;
			[Tooltip("This list needs to be the same size as the ages list. Associate a color to an age space.")]
			[SerializeField] List<Color> colors;

			public void Apply(IList<DetectedFace> faces, Texture2D originalImage, Texture2D image)
			{
				Debug.Log("FaceCount: " + faces.Count);
				foreach (var face in faces)
				{
					FaceRectangle faceRectangle = face.FaceRectangle;

					int w = faceRectangle.Width;
					int h = faceRectangle.Height;

					Color boxColor = colors[0];
					for (int k = 0; k < colors.Count; ++k)
					{
						boxColor = colors[k];

						if (ages[k] >= face.FaceAttributes.Age)
						{
							break;
						}
					}

					for (int i = 0; i < w; ++i)
					{
						for (int j = 0; j < h; ++j)
						{
							var posX = faceRectangle.Left + i;
							var posY = image.height - (faceRectangle.Top + j);
							image.SetPixel(posX, posY, boxColor * originalImage.GetPixel(posX, posY));
						}
					}

					image.Apply();
				}
			}
		}
	}
}
