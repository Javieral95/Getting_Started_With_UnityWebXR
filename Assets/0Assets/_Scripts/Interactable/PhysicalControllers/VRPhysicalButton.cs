//==================================================================================================================
//
// PhysicButton.cs
// Implements a physic buttons which you cant press with your hand.
//
//==================================================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRPhysicalButton : SpecialInteractable
{
    public UnityEvent onPressed, onReleased;    

    private bool _isPressed = false;
    private Rigidbody _rb;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isPressed)
            AddForceToButton();
    }

    #region Events
    //XR Interaction
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            PressButton();
    }

    void OnTriggerExit(Collider other)
    {
        ReleaseButton();
    }
    #endregion

    #region Functions
    //Need to separate Events and function because Non XR interaction don`t work with colliders.

    private void AddForceToButton()
    {
        _rb.AddForce((-1) * transform.up * 5);
    }

    public void PressButton()
    {
        if (!_isPressed)
        {
            _isPressed = true;
            onPressed.Invoke();
            Debug.Log("Button Pressed");
        }
    }

    public void ReleaseButton()
    {
        _isPressed = false;
        onReleased.Invoke();
        Debug.Log("Button Released");
    }


    public override void Grab(bool isXR = false)
    {
        PressButton();
    }
    public override void Drop(bool isXR = false)
    {
        ReleaseButton();
    }
    public override void Throw(bool isXR = false)
    {
        ReleaseButton();
    }
    #endregion

}
