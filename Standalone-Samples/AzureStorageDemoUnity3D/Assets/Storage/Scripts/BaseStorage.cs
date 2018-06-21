using Microsoft.WindowsAzure.Storage;
using UnityEngine;
using UnityEngine.UI;

public class BaseStorage : MonoBehaviour
{
    // Note that due to a Unity limitation, you cannot use https, so make sure your endpoint connection string
    // only uses http. THIS MEANS YOUR CONNECTION WILL NOT BE ENCRYPTED. We are working on that.
	public string ConnectionString = string.Empty;

	protected CloudStorageAccount StorageAccount;
	private Text _myText;  // The Text field on the canvas used to output messages in this demo

	// Use this for initialization
	void Start ()
	{
		_myText = GameObject.Find("DebugText").GetComponent<Text>();
		StorageAccount = CloudStorageAccount.Parse(ConnectionString);
	}

    // Clears the Canvas output text
	public void ClearOutput()
	{
		_myText.text = string.Empty;
	}

    // Appends a string to a new line in the canvas output text
	public void WriteLine(string s)
	{
		if(_myText.text.Length > 20000)
			_myText.text = string.Empty + "-- TEXT OVERFLOW --";

		_myText.text += s + "\r\n";
	}
}
