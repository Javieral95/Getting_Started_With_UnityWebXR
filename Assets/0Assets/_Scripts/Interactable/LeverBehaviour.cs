using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverBehaviour : MonoBehaviour
{
    public UnityEvent onActivate, onDisactivate;

    public ColliderTriggerListener OnCollider;
    public ColliderTriggerListener OffCollider;
    public Collider TriggerCollider;

    private string _TRIGGER_TAG;
    private bool _is_activate = false;

    // Start is called before the first frame update
    void Start()
    {
        _TRIGGER_TAG = TriggerCollider.gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if(OnCollider.IsColliderActivate && !_is_activate)
            ActivateLever();
        else if(OffCollider.IsColliderActivate && _is_activate)
            DisactivateLever();
    }

    private void ActivateLever()
    {
        Debug.Log("ACTIVATE");
        onActivate.Invoke();
        _is_activate = true;
    }
    
    private void DisactivateLever()
    {
        Debug.Log("DISACTIVATE");
        onDisactivate.Invoke();
        _is_activate = false;
    }
}
