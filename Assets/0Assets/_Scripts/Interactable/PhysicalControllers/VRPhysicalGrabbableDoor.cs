/***********************************************************************
 ***************** SPECIAL INTERACTABLE OBJECT: DOOR *******************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPhysicalGrabbableDoor : SpecialInteractable
{
    public bool FollowAlways = true; //TO-DO: Change position after drop object (or follow always if dont grab it)
    public Transform Target;

    private bool isGrabbed = false;
    private Rigidbody _rb;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();
    }

    new void Update()
    {
        base.Update();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(FollowAlways || !isGrabbed)
            _rb.MovePosition(Target.transform.position);
    }

    public override void Grab(bool isXR = false)
    {
        isGrabbed = true;
    }

    public override void Drop(bool isXR = false)
    { //Need to Do After fixed update
        //this.transform.position = Target.position;
        //this.transform.rotation = Target.rotation;
        base.ResetPosition();
        isGrabbed = false;
    }

    public override void Throw(bool isXR = false)
    {
        Debug.LogError("Cannot throw a door!!!!");
    }
}
