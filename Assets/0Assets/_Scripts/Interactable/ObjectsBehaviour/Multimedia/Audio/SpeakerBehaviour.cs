/***********************************************************************
 ****** Play music and apply a force to speakers (like vibration) ******
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerBehaviour : MonoBehaviour
{
    public AudioSource AudioSource;
    private bool _haveAudio = true;

    public List<Rigidbody> Speakers;
    [SerializeField, Range(0.1f, 10)] private float speakersVibrationForce = 0.1f;

    [SerializeField, Range(0.5f, 3)] private float vibrationSecondsDelay = 0.5f;
    [SerializeField, Range(0.2f, 3)] private float vibrationSecondsIncrease = 1f;

    private void Start()
    {
        if (AudioSource == null)
        {
            Debug.LogWarning($"Warning! Didnt select an audio source for {this.gameObject.name}");
            _haveAudio = false;
        }
    }

    public void PlayMusic(bool on)
    {
        if (on)
            StartVibration();
        else
            StopVibration();
        PlayAudio(on);
    }

    private void PlayAudio(bool on)
    {
        if (!_haveAudio) return;
        if (on)
            AudioSource.Play();
        else
            AudioSource.Stop();
    }

    private void StartVibration()
    {
        float seconds = vibrationSecondsDelay;
        foreach (Rigidbody speaker in Speakers)
        {
            StartCoroutine(VibrateSpeaking(speaker, seconds));
            seconds = seconds + vibrationSecondsIncrease;
        }
    }

    private void StopVibration()
    {
        StopAllCoroutines();
    }

    private IEnumerator VibrateSpeaking(Rigidbody speaker, float seconds = 0.5f)
    {
        while (true)
        {
            speaker.AddRelativeForce(speakersVibrationForce * transform.forward, ForceMode.Impulse);
            yield return new WaitForSeconds(.1f);
        }
    }
}