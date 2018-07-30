using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


// Helper class not part of SDK
public class OnlineImageRequester : MonoBehaviour, IImageRequester
{
	private Texture2D texture;

	public IEnumerator RequestImage(string uri, bool canInterupt = true)
	{
		using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
		{
			yield return request.SendWebRequest();

			if (request.isNetworkError || request.isHttpError)
			{
				Debug.Log(request.error);
			}
			else
			{
				texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
			}
		}
	}

	public Texture2D GetImage()
	{
		return texture;
	}
}
