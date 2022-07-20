using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class MusicalObjectBehaviour : SpecialInteractable
{
    //== Parameters ==
    [Header("Musical object settings")]
    public AudioClip AudioClip;
    public string NoteText;
    private TextMeshProUGUI _noteText;

    [Header("Interaction Colour"), Tooltip("When the user play this sound, will change its color")]
    public Color InteractionColor;

    private Color initColor;
    private Material _mat;

    // Sound Parameters
    //private AudioSource _audioSource; Is in Parent (SpecialInteractable)
    [SerializeField, Tooltip("Use for music base, for example")]
    private bool isLoopSound;

    private bool isMarked;
    private Color markColor;

    // Unity Events
    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.loop = isLoopSound;
        _audioSource.playOnAwake = false;
    }
    new void Start()
    {
        base.Start();

        HaveBreakInteraction = false;

        _mat = this.GetComponent<Renderer>().material;
        initColor = _mat.color;

        _noteText = GetComponentInChildren<TextMeshProUGUI>();

        if (AudioClip == null) Debug.LogError($"{gameObject.name} You didnt select a Audio Clip!!!");
        else
            _audioSource.clip = AudioClip;

        if (!string.IsNullOrWhiteSpace(NoteText)) _noteText.text = NoteText;
    }

    //Auxiliar Functions
    private void Play()
    {
        PrintObject(InteractionColor);
        PlaySound();
        StartCoroutine(BackToDefaultColour());
    }

    public void ChangePitch(float value)
    {
        _audioSource.pitch = value;
    }

    public void MarkObject(Color color)
    {
        markColor = color;
        isMarked = true;
        PrintObject(color);
    }
    public void DesmarkObject()
    {
        isMarked = false;
        RestoreColor();
    }

    private void PrintObject(Color color)
    {
        _mat.color = color;
    }

    private void RestoreColor()
    {
        if (isMarked)
            _mat.color = markColor;
        else
            _mat.color = initColor;
    }

    /// <summary>
    /// Wait one frame and back to default color
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackToDefaultColour()
    {
        yield return 0;
        RestoreColor();
    }

    // Events
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.INTERACTABLE_TAG))
            Play();
    }

    public override void Drop(bool isXR = false)
    {
    }

    public override void Grab(bool isXR = false)
    {
        Play();
    }

    public override void Throw(bool isXR = false)
    {
    }
}
