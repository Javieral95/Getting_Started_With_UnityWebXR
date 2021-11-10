﻿//==================================================================================================================
//
// PlayerController.cs (based in WebXRMove2.cs)
// Move the camera in the environment using the sticks or mouse
// Read the sticks using the WebXRInputManager
// Look if we are using the Unity Editor or a browser (with/without 3d glases)
// If the user doesnt have 3D Glasses he will be able to translate with keyboard and rotate & grab objects with mouse
//
//==================================================================================================================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using WebXR;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Properties
    private static NonXRInteraction _nonXR_interactor;

    [Header("Player movement (Only Unity Editor)")]
    [SerializeField, Tooltip("Enable/disable rotation control like use VR hardware. For use in Unity editor only.")]
    private bool ForceXR_VR = false;

    [Header("WebXR objects")]
    public WebXRInputManager inputManagerLeftHand;
    public WebXRInputManager inputManagerRightHand;

    public Transform cameraMainTransform;
    public Transform cameraLeftTransform;
    private Camera myCamera;

    private WebXRDisplayCapabilities capabilities;

    [Header("Player settings"), Tooltip("Movement Speed")]
    public float speed = 5f;
    public bool canStrafe = true;

    [SerializeField, Range(6, 30)] private float gravity = 20.0f;
    [SerializeField, Range(0, 10)] private float jumpSpeed = 6.0f;

    [Header("NonXR settings"), Tooltip("Mouse sensitivity"), Range(1, 5)]
    public float mouseSensitivity = 1f;

    [Tooltip("Straffe Speed")]
    public float rotationAngle = 15f;

    [Header("Debug Texts")]
    public Text stickText;
    public Text cameraText;

    //Rotations
    private float minimumX = -360f;
    private float maximumX = 360f;

    private float minimumY = -90f;
    private float maximumY = 90f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private Quaternion originalRotation;

    //Others
    private bool inGround = false;
    private float strickXPreb = 0;

    private WebXRInputManager inputManager;
    private Transform myTransform;
    public CharacterController controller { get; private set; }
    private CollisionFlags flags;


    private bool XRMoveEnabledPrev = false;

    private Vector3 moveDirection = Vector3.zero;

    #endregion

    #region Unity Functions
    private void Awake()
    {
        _nonXR_interactor = this.GetComponent<NonXRInteraction>();
        _nonXR_interactor.InitCamera(myCamera);

    }

    private IEnumerator Start()
    {
        myTransform = transform;
        myCamera = cameraMainTransform.GetComponent<Camera>();

        originalRotation = transform.localRotation;
        controller = GetComponent<CharacterController>();

        XRMoveEnabledPrev = ForceXR_VR;

        yield return new WaitForSeconds(0.5f);


        capabilities = new WebXRDisplayCapabilities();
        //capabilities = WebXRManager.Instance.GetWebXRDisplayCapabilities();
        //Debug.Log("WebXRMove->Start:: vrCapabilities VR: " + capabilities.supportsImmersiveVR.ToString());
    }

    void Update()
    {
        // if (!XRDevice.isPresent && XRMoveEnabled) { EnableXRMove(false); }

        if (ForceXR_VR)
            MoveXR();
        else
            MoveNonXR();

        //Only in Unity editor:
        if (XRMoveEnabledPrev != ForceXR_VR)
        {
            Debug.Log("Update -> XRMoveEnabled: " + ForceXR_VR);
            EnableXRMove(ForceXR_VR);
            XRMoveEnabledPrev = ForceXR_VR;
        }
        //Debug.Log("Update -> Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " + inputManagerRightHand.stick);
    }

    #endregion

    #region XR Events and functions

    //Events
    private void onXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
    {
        Debug.Log("onXRChange:: state: " + state.ToString());
        if (state == WebXRState.VR)
        {
            EnableXRMove(true);
        }
        else
        {
            EnableXRMove(false);
        }
    }

    private void onXRCapabilitiesUpdate(WebXRDisplayCapabilities vrCapabilities)
    {
        Debug.Log("onXRCapabilitiesUpdate:: vrCapabilities VR: " + vrCapabilities.canPresentVR.ToString());
        capabilities = vrCapabilities;
        EnableAccordingToPlatform();
    }

    void OnEnable()
    {
        Debug.Log("Enable event!");
        WebXRManager.OnXRChange += onXRChange;
        WebXRManager.OnXRCapabilitiesUpdate += onXRCapabilitiesUpdate;

        ChangeXRStatus(true);
    }

    void OnDisable()
    {
        Debug.Log("Disable event!");
        WebXRManager.OnXRChange -= onXRChange;
        WebXRManager.OnXRCapabilitiesUpdate -= onXRCapabilitiesUpdate;

        ChangeXRStatus(false);
    }

    //Enable and disable
    private void EnableXRMove(bool _enable)
    {
        ForceXR_VR = _enable;
        ChangeXRStatus(_enable);
        Debug.Log("EnableXRMove:: XRMoveEnabled: " + ForceXR_VR);
    }

    /// Enables rotation and translation control for desktop environments.
    /// For mobile environments, it enables rotation or translation according to
    /// the device capabilities.
    private void EnableAccordingToPlatform()
    {
        ForceXR_VR = capabilities.canPresentVR;
        ChangeXRStatus(ForceXR_VR);
        Debug.Log("EnableAccordingToPlatform:: XRMoveEnabled: " + ForceXR_VR);
    }
    #endregion

    #region FUNCTIONS

    #region XR

    private void MoveXR()
    {
        //if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        // Traslation (Only translation forward, no sideways)
        if (inputManagerLeftHand != null)
        {
            //float moveX = canStrafe ? inputManagerLeftHand.stick.x : 0;
            //float moveZ = -inputManagerLeftHand.stick.y;
            float moveX = inputManagerLeftHand.stick.x;
            float moveZ = inputManagerLeftHand.stick.y * (-1);

            Quaternion cameraDirection = GetCameraRotation();

            var dir = GetCameraRotation() * new Vector3(moveX, 0, moveZ);
            MoveCharacterController(dir);
        }

        // Rotation (No rotation on Y -> Up/Down)
        if (inputManagerRightHand != null)
        {
            TickXRRotation();
        }

        // Write the values in the UI text in game screen
        if (stickText != null)
            stickText.text = "Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " +
                             inputManagerRightHand.stick;

        /*if (inputManagerLeftHand.IsStickEnabled() || inputManagerRightHand.IsStickEnabled())
            Debug.Log("MoveXR -> Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " + inputManagerRightHand.stick);*/
    }

    private void TickXRRotation()
    {
        //if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        if (strickXPreb < 0.1f && Mathf.Abs(inputManagerRightHand.stick.x) > 0.1f)
        {
            float angle = rotationAngle;
            if (inputManagerRightHand.stick.x < 0)
            {
                angle = -angle;
            }

            rotationX += angle;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            myTransform.localRotation = originalRotation * xQuaternion;
        }

        strickXPreb = Mathf.Abs(inputManagerRightHand.stick.x);
    }

    private void PlainXRRotation()
    {
        //if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        rotationX += inputManagerRightHand.stick.x * mouseSensitivity;
        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

        myTransform.localRotation = originalRotation * xQuaternion; // * yQuaternion;
    }

    #endregion

    #region Non XR

    private void MoveNonXR()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveCharacterController(dir);

        // Rotation
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX = ClampAngle(rotationX, minimumX, maximumX);

        rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion YQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
        transform.localRotation = originalRotation * xQuaternion * YQuaternion;
    }

    #endregion

    #region Auxiliar methods

    private Quaternion GetCameraRotation()
    {
        return cameraMainTransform.rotation;
    }

    void MoveCharacterController(Vector3 direction)
    {
        if (inGround)
        {
            // We are grounded, so recalculate movedirection directly from axes
            moveDirection = myTransform.TransformDirection(direction);
            moveDirection.y = 0; //Avoid to move in Y axis
            moveDirection *= speed;

            if (inputManagerLeftHand.is_B_ButtonPressed() || Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        flags = controller.Move(moveDirection * Time.deltaTime);
        inGround = (flags & CollisionFlags.CollidedBelow) != 0;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    #region Change XR Status methods
    //Change XR Status
    private void ChangeXRStatus(bool setActive)
    {
        ChangeShowCursor(!setActive);
        ChangeNotXRInteraction(!setActive);
        ChaneXRHandStatus(setActive);
    }

    private void ChangeShowCursor(bool setActive)
    {
        Debug.Log("Cursor set visible to: " + setActive);
        Cursor.visible = setActive;
        if (setActive)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    private void ChaneXRHandStatus(bool setActive)
    {
        inputManagerRightHand.gameObject.SetActive(setActive);
        inputManagerLeftHand.gameObject.SetActive(setActive);
    }

    private void ChangeNotXRInteraction(bool setActive)
    {
        _nonXR_interactor.enabled = setActive;
    }

    #endregion

    #endregion
}