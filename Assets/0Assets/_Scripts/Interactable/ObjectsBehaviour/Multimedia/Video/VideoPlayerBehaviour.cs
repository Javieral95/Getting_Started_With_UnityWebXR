using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VideoPlayerBehaviour : TapeLectorBehaviour
{
    [Header("Video Player Settings"), Tooltip("The screen where the videos will play in")]
    public ScreenVideoBehaviour ScreenVideo;
    [Tooltip("Toggle where you play the video... if you want it")]
    public VRPhysicalToggle PlayButton;

    private VideoTapeBehaviour _videoTape;
    private bool _canPlayVideo;
    private bool _playButtonIsActive;

    // Start is called before the first frame update
    void Start()
    {
        if (ScreenVideo == null) Debug.LogError($"{this.gameObject.name} Didnt select a Screen Video object!");
        if (PlayButton == null) Debug.LogWarning($"{this.gameObject.name} Didnt select a Play button object!");
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCanPlay();
    }

    private void CheckIfCanPlay()
    {
        if (!_playButtonIsActive && _canPlayVideo && _tapeInside)
        {
            PlayButton?.ChangeActive(true);
            _playButtonIsActive = true;
        }
        else if (_playButtonIsActive && (!_canPlayVideo || !_tapeInside))
        {
            PlayButton?.ChangeActive(false);
            _playButtonIsActive = false;
        }
    }

    public void ChangeCanPlayStatus(bool isAble)
    {
        _canPlayVideo = isAble;
    }

    // Events
    private void OnTriggerEnter(Collider other)
    {
        if (CheckEnteredTape(other))
        {
            _videoTape = (VideoTapeBehaviour)tape;
            ScreenVideo.ChangeVideoClip(_videoTape.Url);
            if(PlayButton == null)
                ScreenVideo.PlayContent(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckExitTape(other))
        {
            if (PlayButton == null)
                ScreenVideo.PlayContent(false);
            else
            {
                PlayButton.ChangeStatus(false);
                PlayButton.ChangeActive(false);
            }
        }
    }

    public void StopPlaying()
    {
        PlayButton.ChangeStatus(false);
        PlayButton.ChangeActive(false);
    }
}
