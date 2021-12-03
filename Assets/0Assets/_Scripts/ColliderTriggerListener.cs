using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderTriggerListener : MonoBehaviour
{
    public Collider TriggerCollider;
    public bool IsColliderActivate { get; private set; }

    private string triggerColliderTag = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        if (TriggerCollider != null)
            triggerColliderTag = TriggerCollider.gameObject.tag;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerColliderTag) || triggerColliderTag == string.Empty)
            IsColliderActivate = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerColliderTag) || triggerColliderTag == string.Empty)
            IsColliderActivate = false;
    }
}