using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBreakInteraction : SpecialInteractable
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        this.HaveBreakInteraction = true; //Avoid Errors
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    // Special Interactable Functions
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
