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

public class PhysicButton : MonoBehaviour
{
    public UnityEvent onPressed, onReleased;

    private bool _isPressed;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    #region Events
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            _rb.AddRelativeForce((-1) * transform.up * 5);
    }

    private void onTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPressed = true;
            onPressed.Invoke();
            Debug.Log("Button Pressed");
        }
    }

    void OnTriggerExit(Collider other)
    {
        _isPressed = false;
        onReleased.Invoke();
        Debug.Log("Button Released");
    }
    #endregion

}
