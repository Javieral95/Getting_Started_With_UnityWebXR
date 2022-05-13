/***********************************************************************
 ************** Play music and video using an object   *****************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ScreenVideoBehaviour : MonoBehaviour
{
    //Parameters
    public VideoPlayer VideoSource;

    public AudioSource AudioSource;
    [SerializeField] private bool HaveSound;

    private bool isPlaying;
    private string videoUrl;
    private string DEFAULT_VIDEO_FILE_NAME = "video1.mp4";

    private void Start()
    {
        if (AudioSource == null) HaveSound = false;
        if (VideoSource == null)
            Debug.LogWarning("You didn't select a Video Source!!!");
        else if (VideoSource.clip == null)
        {
            Debug.LogWarning("Didnt select a video, will config to use the default video");
            VideoSource.url = System.IO.Path.Combine(Application.streamingAssetsPath, DEFAULT_VIDEO_FILE_NAME);
        }
        else
            VideoSource.url = VideoSource.clip.originalPath;
    }

    //Public event
    public void PlayContent(bool isChecked)
    {
        PlayVideo(isChecked);
        if (HaveSound)
            PlayAudio(isChecked);

        isPlaying = isChecked;
    }

    //Auxiliar functions
    private void PlayVideo(bool isChecked)
    {
        if (isChecked)
            VideoSource.Play();
        else
            VideoSource.Stop();
    }

    private void PlayAudio(bool isChecked)
    {
        if (isChecked)
            AudioSource.Play();
        else
            AudioSource.Stop();
    }
}