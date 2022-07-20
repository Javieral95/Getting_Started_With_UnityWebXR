using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicTheoryLector : TapeLectorBehaviour
{
    [Header("Music Thery Lector settings"), Tooltip("Must be one musical object per one of each 12 notes in order! [C-Db-D-Eb-E-F-Gb-G-Ab-A-Bb-B]")]
    public MusicalObjectBehaviour[] MusicalObjects = new MusicalObjectBehaviour[12];
    public Color ColorToPrintMusicalObjects = Color.cyan;

    [Header("Tone Settings")]
    public TextMeshProUGUI ToneText;
    public Notes InitNote = Notes.C;

    private int _notesCount;
    private int _musicalObjectCounts;
    private int _noteNumber;
    private MusicTheoryTapeBehaviour _theoryTape;

    // Start is called before the first frame update
    void Start()
    {
        if (MusicalObjects == null || MusicalObjects.Count() != 12) Debug.LogError($"{this.gameObject.name} Didnt select musical objects or bad count (Must be one musical object per one of each 12 notes in order! [C-Db-D-Eb-E-F-Gb-G-Ab-A-Bb-B]) !");
        _notesCount = Enum.GetNames(typeof(Notes)).Length;
        ChangeTone(0); //Init
    }

    public void ChangeTone(int value)
    {
        var tmp = _noteNumber + value;
        if (tmp >= (_notesCount - 1)) tmp = 0;
        else if (tmp < 0) tmp = _notesCount - 1;

        _noteNumber = tmp;
        InitNote = (Notes)_noteNumber;

        if (_tapeInside)
        {
            DisableMusicalObjectsPrint();
            PrintMusicalObjects();
        }

        ChangeToneText();
    }

    private void PrintMusicalObjects()
    {
        var interator = _noteNumber;
        _musicalObjectCounts = MusicalObjects.Count();

        MusicalObjects[_noteNumber].MarkObject(ColorToPrintMusicalObjects);

        foreach (int jump in _theoryTape.Jumps)
        {
            interator += jump;

            if (interator >= _musicalObjectCounts)
                interator -= _musicalObjectCounts;

            MusicalObjects[interator].MarkObject(ColorToPrintMusicalObjects);
        }
    }
    private void DisableMusicalObjectsPrint()
    {
        foreach (var musicalObject in MusicalObjects)
            musicalObject.DesmarkObject();
    }

    private void ChangeToneText()
    {
        if (ToneText != null)
            ToneText.text = $"Tone: {InitNote}";
    }

    // Events
    private void OnTriggerEnter(Collider other)
    {
        if (CheckEnteredTape(other))
        {
            _theoryTape = (MusicTheoryTapeBehaviour)tape;
            PrintMusicalObjects();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckExitTape(other))        
            DisableMusicalObjectsPrint();        
    }
}
