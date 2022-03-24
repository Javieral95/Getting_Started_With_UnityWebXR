using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverBehaviour : SpecialInteractable
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
        base.Start();
        _TRIGGER_TAG = TriggerCollider.gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(OnCollider.IsColliderActivate && !_is_activate)
            ActivateLever();
        else if(OffCollider.IsColliderActivate && _is_activate)
            DisactivateLever();
    }

    private void ActivateLever()
    {
        onActivate.Invoke();
        _is_activate = true;
    }
    
    private void DisactivateLever()
    {
        onDisactivate.Invoke();
        _is_activate = false;
    }

    public override void Drop()
    {        
    }

    public override void Grab()
    {
    }

    public override void Throw()
    {
    }
}
