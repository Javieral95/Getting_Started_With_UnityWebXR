/***********************************************************************
 ************* SPECIAL INTERACTABLE OBJECT: TOGGLE (On/Off) ************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRPhysicalToggle : SpecialInteractable
{
    public BoolEvent onChangeValue;

    public GameObject OnObject;
    public GameObject OffObject;

    [SerializeField]
    private readonly bool initValue;

    public bool Value { get; private set; }
    // Start is called before the first frame update
    private void Awake()
    {
        Value = initValue;
        UpdateStatus(Value);
        //For avoid continuous clicking (maxDistance must be default value: 0)
        HaveBreakInteraction = true;
    }
    new void Start()
    {
        base.Start();
    }

    public bool ChangeStatus()
    {
        UpdateStatus(!Value);
        Value = !Value;
        return Value;
    }

    public bool ChangeStatus(bool newValue = false)
    {
        UpdateStatus(newValue);
        Value = newValue;
        return newValue;
    }

    private void UpdateStatus(bool newValue)
    {
        //STACK OVERFLOW
        OnObject.SetActive(newValue);
        OffObject.SetActive(!newValue);
        if (newValue != Value)
            onChangeValue.Invoke(newValue);
    }

    // Spetial Interactable Functions
    public override void Drop(bool isXR = false)
    {
    }

    public override void Grab(bool isXR = false)
    {
        ChangeStatus();
    }

    public override void Throw(bool isXR = false)
    {
        Debug.Log("Man, you cannot drop a Toggle >:(");
    }
}
