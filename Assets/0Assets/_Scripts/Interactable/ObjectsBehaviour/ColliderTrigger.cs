using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    [Header("Collider events")]
    public GameObjectEvent EnterEvent;
    public GameObjectEvent ExitEvent;
    public GameObjectEvent InEvent;
    public UnityEvent OutEvent;

    [Header("Collider Trigger Settings"), Tooltip("Leave as blank string to wont use an specific task")]
    public string SpecificTag;

    private bool useSpecificTag;
    private bool haveOutEvent;

    // Start is called before the first frame update
    void Start()
    {
        useSpecificTag = !(string.IsNullOrEmpty(SpecificTag));
        haveOutEvent = (OutEvent != null);

        //if (useSpecificTag && !UnityEditorInternal.InternalEditorUtility.tags.ToList().Contains(SpecificTag))
        //    Debug.LogError($"({this.gameObject.name}) The Specific tag pass as parameter ({SpecificTag}) doesnt exist!!!!");

    }

    private void Update()
    {
        if (haveOutEvent)
            OutEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((!useSpecificTag || other.CompareTag(SpecificTag)) && EnterEvent != null)
            EnterEvent.Invoke(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if ((!useSpecificTag || other.CompareTag(SpecificTag)) && ExitEvent != null)
            ExitEvent.Invoke(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((!useSpecificTag || other.CompareTag(SpecificTag)) && InEvent != null)
            InEvent.Invoke(other.gameObject);
    }
}
