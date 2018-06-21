using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToMovieTheaterScene()
    {
        SceneManager.LoadScene("AzureBlobStorageVideoTest", LoadSceneMode.Single);
    }

    public void GoToBasicStorageDemoScene()
    {
        SceneManager.LoadScene("AzureBlobStorageTest", LoadSceneMode.Single);
    }
}
