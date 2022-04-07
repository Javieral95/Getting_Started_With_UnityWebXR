using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerBehaviour : MonoBehaviour
{
    public AudioSource AudioSource;

    public List<Rigidbody> Speakers;
    [SerializeField, Range(0.1f, 5)] private float speakersVibrationForce = 0.1f;
    
    [SerializeField, Range(0.5f, 3)] private float vibrationSecondsDelay = 0.5f;
    [SerializeField, Range(0.2f, 3)] private float vibrationSecondsIncrease = 1f;
    
    public void PlayMusic()
    {
        AudioSource.Play();
        StartVibration();
    }

    public void StopMusic()
    {
        AudioSource.Stop();
        StopVibration();
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
            speaker.AddRelativeForce(speakersVibrationForce * Vector3.forward, ForceMode.Impulse);
            yield return new WaitForSeconds(.1f);
        }
    }
}