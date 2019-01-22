using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;

public class WorldSpaceVideo : MonoBehaviour {

    // Used to control the appearance of the Play button, with pause toggle
    public Sprite playImage;
    public Sprite pauseImage;
    public Image playButtonImage;
    // Since we're loading videos by url, we're not storing actual VideoClip objects, just file names
    public string[] videoClips;  
    // Used to track video length and current position during playback
    public Text currentMinutes;
    public Text currentSeconds;
    public Text totalMinutes;
    public Text totalSeconds;
    public PlayHeadMover playheadMover;
    // Used to control playback, we get these from the current gameobject
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    // The index of the current video being played
    private int videoClipIndex;
    private bool isTotalTimeSet = false;

    private void Awake()
    {
        // Obtain video playback components from current gameobject
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    // Use this for initialization
    async void Start () {
        // Need to release the video texture to clear the screen on start, 
        // otherwise the last played frame from the last session will stick around
        videoPlayer.targetTexture.Release();
        // Set current video clip to the first one in the array, downloads it if needed
        await PrepareVideoFromFile(videoClips[0]);
    }
	
	// Update is called once per frame
	void Update () {
        // If a video is currently being played... 
		if (videoPlayer.isPlaying)
        {
            // This will only update the total time count for the video once. We need to call in update
            // loop since the total frame count isn;t set until the video actually plays.
            SetTotalTimeUI(false);
            // Update the current time index in the canvas + move the playhead based on progress
            SetCurrentTimeUI();
            playheadMover.MovePlayHead(CalculatePlayedFraction());
        }
	}

    /// <summary>
    /// Prepares a video file for playback by loading it in the video player + initializing audio.
    /// If the file, does not exist, it is downloaded from Azure blob storage first, using default override settings.
    /// All files are downloaded to the TempCache app folder, and played from there.
    /// </summary>
    /// <param name="videofile">The file name (no path) of the video to be played.</param>
    public async Task PrepareVideoFromFile(string videofile)
    {
        string localvideofile = await AzureBlobStorageClient.instance.DownloadStorageBlockBlobSegmentedOperationAsync(videofile);

        if (File.Exists(localvideofile))
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                playButtonImage.sprite = playImage;
            }
            videoPlayer.url = localvideofile;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);

            SetTotalTimeUI(true);

            // Preparation is not needed for videos loaded from url
            //videoPlayer.Prepare();
            ////Wait until video is prepared
            //while (!videoPlayer.isPrepared)
            //{
            //    Debug.Log("Preparing Video");
            //    yield return null;
            //}
            //Debug.Log("Done Preparing Video");
        }
    }

    /// <summary>
    /// Increment/reset the video clip index and prepare it for playback.
    /// </summary>
    public async void SetNextClip()
    {
        videoClipIndex++;

        if (videoClipIndex >= videoClips.Length)
        {
            videoClipIndex = videoClipIndex % videoClips.Length;
        }
        await PrepareVideoFromFile(videoClips[videoClipIndex]);
        
        //videoPlayer.Play();
        //playButtonRenderer.material = pauseButtonMaterial;
    }

    /// <summary>
    /// Toggle between Play & Pause on current video queued based on playback state.
    /// </summary>
    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            playButtonImage.sprite = playImage;
        } else
        {
            videoPlayer.Play();
            playButtonImage.sprite = pauseImage;
        }
    }

    /// <summary>
    /// Updates the player canvas to reflect the current time index being played in the video.
    /// </summary>
    void SetCurrentTimeUI()
    {
        string minutes = Mathf.Floor((int) videoPlayer.time / 60).ToString("00");
        string seconds = ((int)videoPlayer.time % 60).ToString("00");

        currentMinutes.text = minutes;
        currentSeconds.text = seconds;
    }
    void SetTotalTimeUI(bool reset)
    {
        string minutes = "00";
        string seconds = "00";

        if (reset)
        {
            isTotalTimeSet = false;
        }
        else if (!isTotalTimeSet && (videoPlayer.frameCount > 0))
        {
            float videolength = videoPlayer.frameCount / videoPlayer.frameRate;
            minutes = Mathf.Floor((int)videolength / 60).ToString("00");
            seconds = ((int)videolength % 60).ToString("00");
            isTotalTimeSet = true;
        } else
        {
            return;
        }

        totalMinutes.text = minutes;
        totalSeconds.text = seconds;
    }

    double CalculatePlayedFraction()
    {
        double fraction = (double)videoPlayer.frame / (double)videoPlayer.frameCount;
        return fraction;
    }
}
