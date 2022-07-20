/***********************************************************************
 ***************** SPECIAL INTERACTABLE OBJECT: LEAVER *****************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRPhysicalLever : SpecialInteractable
{
    [Header("Physical Lever settings"), Space(5)]
    [Tooltip("When the TriggerCollider collision with OnCollider will call OnActivate event.")]
    public Collider OnCollider;
    [Tooltip("When the TriggerCollider collision with OffCollider will call OnDisactivate event.")]
    public Collider OffCollider;
    [Tooltip("The trigger which is checked for call events")]
    public Collider TriggerCollider;

    [Header("Lever events")]
    public UnityEvent onActivate, onDisactivate;

    // Private properties
    private bool _is_activate;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        CheckLeverStatus();
    }

    // Auxiliar Functions
    private void CheckLeverStatus()
    {
        if (!_is_activate && TriggerCollider.bounds.Intersects(OnCollider.bounds))
            ActivateLever();
        else if (_is_activate && TriggerCollider.bounds.Intersects(OffCollider.bounds))
            DisactivateLever();
    }

    // Events
    private void ActivateLever()
    {
        Debug.Log("ACTIVATE");
        PlaySound();
        onActivate.Invoke();
        _is_activate = true;
    }

    private void DisactivateLever()
    {
        Debug.Log("DISACTIVATE");
        PlaySound();
        onDisactivate.Invoke();
        _is_activate = false;
    }

    // Special Interactable functions
    public override void Drop(bool isXR = false)
    {
    }

    public override void Grab(bool isXR = false)
    {
    }

    public override void Throw(bool isXR = false)
    {
    }
}
