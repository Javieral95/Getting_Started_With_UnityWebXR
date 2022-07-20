/***********************************************************************
 ************** Play music and video using an object   *****************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class ScreenVideoBehaviour : MonoBehaviour
{
    //Parameters
    [Header("Video Player & Clip (Clip has preference)")]
    public VideoPlayer VideoPlayer;

    public VideoClip VideoClip;
    [SerializeField] private string videoUrl;

    [Header("Audio"), Tooltip("Select one if you want to listen the audio from a different object.")]
    public AudioSource ExternalAudioSource;
    public GameObject VideoPlayerLightObject;

    [Tooltip("Put in false if you want to mute the video")]
    public bool WillPlayAudio;

    [Tooltip("Put in true if you want to use another clip instead video track")]
    public bool WillUseExternalAudioClip;

    private bool _isPlaying;
    private bool _useAudioTrack;
    private bool _haveExternalAudioSource;

    [Header("Debug (replace video url)")]
    //Will be deleted :)
    public bool USE_TEST_VIDEO;

    private void Start()
    {
        if (ExternalAudioSource == null) WillUseExternalAudioClip = false;
        else _haveExternalAudioSource = true;

        ChangeLightObjectStatus(false);

        if (USE_TEST_VIDEO) videoUrl = Constants.TEST_VIDEO_URL;

        if (VideoPlayer == null)
            Debug.LogWarning("You didn't select a Video Source!!!");
        else
        {
            VideoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
            if (VideoClip == null)
            {
                Debug.LogWarning($"{(this.gameObject.name)} Didnt select a video, will use url");
                if (string.IsNullOrWhiteSpace(videoUrl))
                {
                    Debug.LogWarning($"{(this.gameObject.name)} Didnt have a valid url, use default video");
                    videoUrl = System.IO.Path.Combine(Application.streamingAssetsPath,
                        Constants.DEFAULT_VIDEO_FILE_NAME);
                }

                ChangeVideoClip(videoUrl);
            }
            else
                ChangeVideoClip(VideoClip);
        }

        ConfigAudioClip();
    }

    #region Config Auxiliar Functions
    public void ChangeVideoClip(VideoClip clip)
    {
        VideoClip = clip;
        VideoPlayer.source = VideoSource.VideoClip;
        ConfigAudioClip();
    }

    public void ChangeVideoClip(string url)
    {
        VideoClip = null;
        videoUrl = url;
        VideoPlayer.source = VideoSource.Url;
        VideoPlayer.url = videoUrl;
        ConfigAudioClip();
    }

    private void ConfigAudioClip()
    {
        if (!WillPlayAudio)
        {
            VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            return;
        }

        if (_haveExternalAudioSource)
        {
            ExternalAudioSource.playOnAwake = false;
            VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        }
        else
            VideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        if (!WillUseExternalAudioClip)
        {
            VideoPlayer.controlledAudioTrackCount = 1;
            //Assign the Audio from Video to AudioSource to be played
            VideoPlayer.EnableAudioTrack(0, true);
            VideoPlayer.SetTargetAudioSource(0, ExternalAudioSource);
        }
    }

    public void ChangeVolume(float value)
    {
        if (_haveExternalAudioSource)
        {
            value = value / 100;
            value = Mathf.Clamp(value, 0, 1);

            if (_haveExternalAudioSource)
                ExternalAudioSource.volume = value;
            else
                VideoPlayer.SetDirectAudioVolume(0, value);

#if UNITY_WEBGL
            AudioListener.volume = value;
#endif
        }
    }

    #endregion

    #region  Functions
    public void PlayContent(bool on)
    {
        if (string.IsNullOrEmpty(videoUrl) && VideoClip == null) return;

        if (VideoPlayer.isPlaying)
        {
            VideoPlayer.Stop();
            if (_haveExternalAudioSource) ExternalAudioSource.Stop();
            if (VideoPlayerLightObject != null) VideoPlayerLightObject.SetActive(false);
        }
        else
            StartCoroutine(PlayAfterPrepare());

        _isPlaying = on;
    }

    public void PauseOrResumeContent(bool on)
    {
        if (string.IsNullOrEmpty(videoUrl) && VideoClip == null) return;

        if (VideoPlayer.isPlaying)
        {
            VideoPlayer.Pause();
            if (_haveExternalAudioSource) ExternalAudioSource.Pause();
        }
        else
        {
            VideoPlayer.Play();
            if (_haveExternalAudioSource) ExternalAudioSource.Play();
        }

        _isPlaying = on;
    }

    IEnumerator PlayAfterPrepare()
    {
        VideoPlayer.Prepare();

        while (!VideoPlayer.isPrepared)
        {
            yield return new WaitForEndOfFrame();
        }

        ChangeLightObjectStatus(true);
        VideoPlayer.Play();
        if (_haveExternalAudioSource)
            ExternalAudioSource.Play();
    }

    //private static void ConfigureTexture(VideoPlayer vp)
    //{
    //    vp.texture.wrapMode = TextureWrapMode.Mirror;
    //}

    private void ChangeLightObjectStatus(bool isOn)
    {
        if (VideoPlayerLightObject != null) VideoPlayerLightObject.SetActive(isOn);
    }

    #endregion





}