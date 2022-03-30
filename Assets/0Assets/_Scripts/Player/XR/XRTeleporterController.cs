using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRTeleporterController : MonoBehaviour
{
    public float stickSensitivity = 200;
    public VRTeleporter Teleporter;
    public WebXRInputManager InputManager;

    private bool isActive = false;
    private float stickvalue = 0;

    #region Singleton
    public static XRTeleporterController Instance;
    private void Awake() { Instance = this; }
    #endregion

    public bool IsTeleporterActive() { return isActive; }

    private void Update()
    {
        if (InputManager.IsTriggerButtonDown())
        {
            isActive = true;
            stickvalue = 90;
            Teleporter.ToggleDisplay(true);
        }

        if (InputManager.IsTriggerButtonUp())
        {
            if (isActive) { Teleporter.Teleport(stickvalue - 90); }
            isActive = false;
            Teleporter.ToggleDisplay(false);
        }

        if (isActive)
        {
            stickvalue -= InputManager.stick.x * stickSensitivity * Time.deltaTime;
            Teleporter.PositionMarker.transform.Find("Arrow").localEulerAngles = new Vector3(0,0, stickvalue);
        }
    }
}