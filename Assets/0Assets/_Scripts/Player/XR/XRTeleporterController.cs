using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class XRTeleporterController : MonoBehaviour
{
    public float stickSensitivity = 200;
    public bool stickRotate = false;
    public VRTeleporter Teleporter;
    public WebXRInputManager InputManager;
    private bool isActive = false;
    private float stickvalue = 0;
    private PlayerController player;

    #region Singleton
    public static XRTeleporterController Instance;
    private void Awake()
    {
        Instance = this;
        Teleporter.transform.Find("Marker").Find("Arrow").gameObject.SetActive(stickRotate);
    }
    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }
    #endregion
    public bool IsTeleporterActive() { return isActive; }
    private void Update()
    {
        if (player.isTeleportEnabled)
        {
            if (InputManager.IsTriggerButtonDown())
            {
                isActive = true;
                stickvalue = 90;
                Teleporter.ToggleDisplay(true);
            }
            if (InputManager.IsTriggerButtonUp())
            {
                if (isActive)
                {
                    if (stickRotate) { Teleporter.Teleport(stickvalue - 90); }
                    else { Teleporter.Teleport(); }
                }
                isActive = false;
                Teleporter.ToggleDisplay(false);
            }
            if (isActive)
            {
                stickvalue -= InputManager.stick.x * stickSensitivity * Time.deltaTime;
                Teleporter.PositionMarker.transform.Find("Arrow").localEulerAngles = new Vector3(0, 0, stickvalue);
            }
        }
    }
}