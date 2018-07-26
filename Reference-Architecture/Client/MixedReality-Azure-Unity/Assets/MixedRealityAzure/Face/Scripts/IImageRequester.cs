using System.Collections;
using UnityEngine;



// Helper class not part of SDK
public interface IImageRequester
{
	IEnumerator RequestImage(string uri, bool canInterupt = true);
	Texture2D GetImage();
}
