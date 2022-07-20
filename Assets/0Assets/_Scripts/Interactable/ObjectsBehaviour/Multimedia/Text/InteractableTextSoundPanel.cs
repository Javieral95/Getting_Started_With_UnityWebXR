/***********************************************************************
 ******* Use it as an InteractableTextPanel but can play audio *********
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractableTextSoundPanel : InteractableTextPanel
{
    public bool isPlaying { get; private set; }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _audioSource = this.GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    new void Update()
    {
        base.Update();
        isPlaying = _audioSource.isPlaying;
    }

    /// <summary>
    /// Override the AudioSource clip and play it
    /// </summary>
    /// <param name="newClip"></param>
    public void Play(AudioClip newClip)
    {
        if (isPanelActive)
        {
            if (isPlaying && _audioSource.clip?.name == newClip.name)
                Stop();
            else
            {
                _audioSource.clip = newClip;
                Play();

            }
        }
    }

    /// <summary>
    /// Play the audio source clip
    /// </summary>
    public void Play()
    {
        if (isPanelActive)
            gameManager.PlayAudioSource(_audioSource);
    }

    public void Stop()
    {
        gameManager.StopAudioSource();
        _audioSource.clip = null;
    }

}
