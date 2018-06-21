using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour {

    Image foregroundImage;
    Image backgroundImage;
    Text progressText;

    int downloadcount;

    const string defaultmessage = "file transfers left: ";

    public float Value
    {
        get
        {
            if (foregroundImage != null)
                return (foregroundImage.fillAmount * 100f);
            else
                return 0;
        }
        set
        {
            if (foregroundImage != null)
                foregroundImage.fillAmount = (value / downloadcount) / (downloadcount * 100f);
        }
    }

    void Start()
    {
        foreach (Image img in gameObject.GetComponentsInChildren<Image>())
        {
            switch (img.name)
            {
                case "ProgressBarForeground":
                    foregroundImage = img;
                    break;
                case "ProgressBarBackground":
                    backgroundImage = img;
                    break;
            }
        }
        //foregroundImage = GameObject.Find("ProgressBarForeground").GetComponent<Image>();
        //backgroundImage = GameObject.Find("ProgressBarBackground").GetComponent<Image>();
        progressText = gameObject.GetComponentInChildren<Text>();
        ResetProgress();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void AddDownload()
    {
        downloadcount++;
        if(downloadcount > 0)
        {
            backgroundImage.enabled = true;
            foregroundImage.enabled = true;
            progressText.enabled = true;
            progressText.text = defaultmessage + downloadcount.ToString();
        }
    }

    public void RemoveDownload()
    {
        downloadcount--;
        progressText.text = defaultmessage + downloadcount.ToString();
        if (downloadcount == 0)
        {
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        backgroundImage.enabled = false;
        foregroundImage.enabled = false;
        progressText.enabled = false;
        Value = 0;
    }
}
