/***********************************************************************
 ***************** SPECIAL INTERACTABLE OBJECT: DOOR *******************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPhysicalGrabbableDoor : SpecialInteractable
{
    [Header("Physical Grabbable Door settings"), Space(5), Tooltip("If it set to true this object will Follow always the reference object (using physics), otherwhise only follow when the user don't grab it.")]
    public bool FollowAlways = true;
    public Transform Target;

    private bool isGrabbed = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
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
