//==================================================================================================================
//
// WebXRInputManager.cs
// A wrapper to read all input from original code: WebXRController.cs
// So, all the project will detect the input looking at this class instead of doing it from WebXRController
//
//==================================================================================================================

using UnityEngine;
using System.Collections.Generic;
using WebXR;


public enum XRButtonStatus { ButtonDown, ButtonUp, isDown, None }

[RequireComponent(typeof(WebXRController))]
public class WebXRInputManager : MonoBehaviour
{
    //public float stickAxis = 0;
    public Vector2 stick = Vector2.zero;
    private float[] strickFloat;
    private XRButtonStatus triggerStatus = XRButtonStatus.None;
    private XRButtonStatus gripStatus = XRButtonStatus.None;

    //New:
    private XRButtonStatus aStatus = XRButtonStatus.None;
    private XRButtonStatus bStatus = XRButtonStatus.None;

    private float triggerAxis = 0;
    private float gripAxis = 0;

    [Space (10)]
    public bool showDebug = false;

    private WebXRController controller;
    private string controllerHand;

    public WebXRController GetController() { return controller; }
    public bool IsControllerLeft() { return controller.hand == WebXRControllerHand.LEFT; }
    public bool IsControllerRight() { return controller.hand == WebXRControllerHand.RIGHT; }
    public void Pulse(float intensity, float duration) { controller.Pulse(intensity, duration); }

    public bool IsStickEnabled() { return stick.x != 0 || stick.y != 0; }

    public bool IsTriggerButtonInactive() { return triggerStatus == XRButtonStatus.None; }
    public bool IsTriggerButtonDown() { return triggerStatus == XRButtonStatus.ButtonDown ; }
    public bool IsTriggerDown() { return triggerStatus == XRButtonStatus.isDown; }
    public bool IsTriggerButtonUp() { return triggerStatus == XRButtonStatus.ButtonUp; }
    public float GetTriggerAxis() { return triggerAxis; }

    public bool IsGripButtonInactive() { return gripStatus == XRButtonStatus.None; }
    public bool IsGripButtonDown() { return gripStatus == XRButtonStatus.ButtonDown; }
    public bool IsGripDown() { return gripStatus == XRButtonStatus.isDown; }
    public bool IsGripButtonUp() { return gripStatus == XRButtonStatus.ButtonUp; }
    public float GetGripAxis() { return gripAxis; }

    public bool Is_A_ButtonPressed() {  return aStatus == XRButtonStatus.ButtonDown;  }
    public bool Is_B_ButtonPressed() { return bStatus == XRButtonStatus.ButtonDown; ; }

    public bool Is_A_ButtonUp() { return aStatus == XRButtonStatus.ButtonUp; }
    public bool Is_B_ButtonUp() { return bStatus == XRButtonStatus.ButtonUp; ; }

    private Vector3 fixupAngle = new Vector3(-30f, 0f, 0f); /**/
    void Awake()
    {
        controller = GetComponent<WebXRController>();
        controllerHand = controller.hand.ToString();
    }

    void Update()
    {
        /*transform.position -= Vector3.up * 1.0f;
        transform.rotation *= Quaternion.Euler(fixupAngle);*/

        ReadXRInput();


        if (IsTriggerButtonDown() && showDebug) Debug.Log("Controller " + controllerHand + " Trigger ButtonDown");
        if (IsGripButtonDown() && showDebug) Debug.Log("Controller " + controllerHand + " Grip ButtonDown");

        /*if (controller.GetButton("Trigger")) { if(showDebug) Debug.Log("Controller " + controllerHand + " Trigger is Down"); }
        if (controller.GetButton("Grip")) { if(showDebug) Debug.Log("Controller " + controllerHand + " Grip is Down"); } */

        if (IsTriggerButtonUp() && showDebug) Debug.Log("Controller " + controllerHand + " Trigger ButtonUp");
        if (IsGripButtonUp() && showDebug) Debug.Log("Controller " + controllerHand + " Grip ButtonUp");

        if (Is_A_ButtonPressed() && showDebug) Debug.Log("Controller " + controllerHand + " A (or X) Pressed");
        if (Is_B_ButtonPressed() && showDebug) Debug.Log("Controller " + controllerHand + " B (or Y) Pressed");

        /*stickAxis = controller.GetAxis("Stick");
        if(stickAxis != 0) Debug.Log("Controller " + controllerHand + " Strick Axis: "+ stickAxis.ToString());*/

        if (IsStickEnabled()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Strick Axis: " + stick.ToString()); }
    }

    private void ReadXRInput()
    {
        //Stick

        //controller.TryUpdateButtons();

        /*stick.x = controller.GetAxis("StickX");
        stick.y = controller.GetAxis("StickY");*/
        stick = controller.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);
        #if UNITY_EDITOR
        stick.y *= -1;
        #endif 

        
        //Trigger & Grip
        GetXRButtonStatus(WebXRController.ButtonTypes.Trigger, ref triggerStatus);
        GetXRButtonStatus(WebXRController.ButtonTypes.Grip, ref gripStatus);

        triggerAxis = controller.GetAxis(WebXRController.AxisTypes.Trigger);
        gripAxis = controller.GetAxis(WebXRController.AxisTypes.Grip);

        //New: A & B buttons (or X & Y)
        GetXRButtonStatus(WebXRController.ButtonTypes.ButtonA, ref aStatus);
        GetXRButtonStatus(WebXRController.ButtonTypes.ButtonB, ref bStatus);

    }

    private void GetXRButtonStatus(WebXRController.ButtonTypes buttonType, ref XRButtonStatus buttonStatus)
    {
        if (controller.GetButtonDown(buttonType))
        {
            buttonStatus = XRButtonStatus.ButtonDown;
        }
        else if (controller.GetButtonUp(buttonType))
        {
            buttonStatus = XRButtonStatus.ButtonUp;
        }
        else if (controller.GetButton(buttonType))
        {
            buttonStatus = XRButtonStatus.isDown;
        }
        else
        {
            buttonStatus = XRButtonStatus.None;
        }
    }
}
