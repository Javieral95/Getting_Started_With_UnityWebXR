using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotMovable : MonoBehaviour
{
    public bool FollowAlways = true; //TO-DO: Change position after drop object (or follow always if dont grab it)
    public Transform Target;

    private bool isGrabbed = false;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(FollowAlways || !isGrabbed)
            _rb.MovePosition(Target.transform.position);
    }

    public void GrabObject()
    {
        isGrabbed = true;
    }

    public void DropObject()
    { //Need to Do After fixed update
        this.transform.position = Target.position;
        this.transform.rotation = Target.rotation;
        isGrabbed = false;
    }

}
