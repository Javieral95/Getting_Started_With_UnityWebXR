/***********************************************************************
 ************** SPECIAL INTERACTABLE OBJECT: Video Card ****************
 ****   Allow the user to change the scale in a musical object      ****
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicTheoryTapeBehaviour : TapeBehaviour
{
    [Header("Notes to mark"), Tooltip("Must change the colour of these notes in a musical object. Value=1(Semitone) . Value=2(Tone)")]
    public int[] Jumps = new int[6]; //1: Semitone 2: Tone.
}

public enum Notes{
    C = 0, 
    Db = 1,
    D = 2,
    Eb = 3,
    E = 4,
    F = 5,
    Gb = 6,
    G = 7,
    Ab = 8,
    A = 9,
    Bb = 10,
    B = 11
}
