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
    [Header("Physical Toggle settings"), Space(5)]
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

    // Status Functions
    public bool ChangeStatus()
    {
        if (this.CanInteract)
        {
            UpdateStatus(!Value);
            PlaySound();
            Value = !Value;
            return Value;
        }
        return false;
    }

    public bool ChangeStatus(bool newValue = false)
    {
        if (this.CanInteract)
        {
            UpdateStatus(newValue);
            Value = newValue;
            return newValue;
        }
        return false;
    }

    private void UpdateStatus(bool newValue)
    {
        //STACK OVERFLOW
        OnObject.SetActive(newValue);
        OffObject.SetActive(!newValue);
        if (newValue != Value)
            onChangeValue.Invoke(newValue);
    }

    // Disable
    public void ChangeActive(bool isActive=true)
    {
        this.CanInteract = isActive;
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
