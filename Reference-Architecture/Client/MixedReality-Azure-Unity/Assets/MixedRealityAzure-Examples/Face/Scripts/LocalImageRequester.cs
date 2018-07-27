using System.Collections;
using UnityEngine;

// Helper class not part of SDK
public class LocalImageRequester : MonoBehaviour, IImageRequester
{
	private Texture2D texture;
	private Object[] images;

	public IEnumerator RequestImage(string uri, bool canInterupt = true)
	{
		if(images == null)
		{
			images = Resources.LoadAll("people");
		}

		int randomIndex = Random.Range(0, images.Length);
		texture = (Texture2D)images[randomIndex];

		yield return null;
	}

	public Texture2D GetImage()
	{
		return texture;
	}
}
