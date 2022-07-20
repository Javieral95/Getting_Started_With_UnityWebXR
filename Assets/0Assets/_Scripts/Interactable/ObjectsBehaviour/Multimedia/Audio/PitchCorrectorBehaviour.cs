using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PitchCorrectorBehaviour : MonoBehaviour
{
    [Header("Objects")]
    public List<MusicalObjectBehaviour> MusicalObjects;
    public TextMeshProUGUI DisplayPitchText;
    [Tooltip("First increase button, second decrease button")]
    public TextMeshProUGUI[] ButtonTexts = new TextMeshProUGUI[2];
    [Header("Config Init pitch value"), SerializeField, Range(0.2f, 3), Tooltip("Init Pitch Value")]
    private float pitchValue;
    
    [Header("Config"), SerializeField, Range(0.2f,3)]
    private float minPitchValue = 0.2f;
    [SerializeField, Range(0.2f, 3)]
    private float maxPitchValue = 3;
    private Vector2 limits;

    [SerializeField, Range(0.2f, 1), Space(5)]
    private float pitchValueChange = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        InitPitchCorrector();
        ChangePitch(pitchValue);
    }

    public void IncreasePitch()
    {
        ChangePitch(pitchValue + pitchValueChange);
    }

    public void DecreasePitch()
    {
        ChangePitch(pitchValue - pitchValueChange);
    }

    private void ChangePitch(float value)
    {
        var newValue = ClampValue(value);

        foreach(var musical in MusicalObjects)        
            musical.ChangePitch(newValue);

        DisplayPitchText.text = $"Pitch: {newValue}";
        pitchValue = newValue;
    }

    private float ClampValue(float value)
    {
        return Mathf.Clamp(value, limits.x, limits.y);
    }
    private void InitPitchCorrector()
    {
        if (minPitchValue > maxPitchValue)
        {
            Debug.LogError("Min pìtch value is higher than max pitch value, swapping values . . . ");
            var tmp = minPitchValue;
            minPitchValue = maxPitchValue;
            maxPitchValue = tmp;
        }
        limits = new Vector2(minPitchValue, maxPitchValue);
        pitchValue = ClampValue(pitchValue);

        if(ButtonTexts.Length == 2)
        {
            ButtonTexts[0].text = $"+{pitchValueChange}";
            ButtonTexts[1].text = $"-{pitchValueChange}";
        }
    }
}

