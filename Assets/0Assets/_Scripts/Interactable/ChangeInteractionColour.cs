/***********************************************************************
 ************ Change object colour when the user select it *************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ChangeInteractionColour : MonoBehaviour
{    
    public Color Color;

    private Color initColor;
    private Material _mat;    

    // Unity Events
    private void Start()
    {
        _mat = this.GetComponent<Renderer>().material;
        initColor = _mat.color;
    }

    // Trigger Events
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHands"))
            SetSelectedColor();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHands"))
            SetDefaultColor();
    }

    private void OnDisable()
    {
        SetDefaultColor();
    }

    // Auxiliar Functions
    private void SetDefaultColor()
    {
        _mat.color = initColor;
    }
    private void SetSelectedColor()
    {
        _mat.color = Color;
    }
}
