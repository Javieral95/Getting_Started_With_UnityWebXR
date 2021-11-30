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
    [SerializeField, Tooltip("Enable/disable rotation and movement control using VR hardware. For use in Unity editor only (Authomatic set to true when dettect hardware).")]
    private bool isXREnabled = false;

    [SerializeField, Tooltip("Enable/disable rotation control using VR hardware's sticks (Non Up down, only LR). For use in Unity editor only.")]
    private bool canRotateWithSticks = true;

    [Header("WebXR objects")]
    public WebXRInputManager inputManagerLeftHand;
    public WebXRInputManager inputManagerRightHand;

    public Transform cameraMainTransform;
    public Transform cameraLeftTransform;
    private Camera myCamera;

    private WebXRDisplayCapabilities capabilities;

    [Header("Player settings"), Tooltip("Movement Speed")]
    public float speed = 5f;
    [SerializeField, Range(0.1f, 5)]
    private float height = 2f;

    [SerializeField, Range(6, 30)] private float gravity = 20.0f;
    [SerializeField, Range(0, 10)] private float jumpSpeed = 6.0f;

    [Header("NonXR settings"), Tooltip("Mouse sensitivity"), Range(1, 5)]
    public float mouseSensitivity = 1f;

    [SerializeField, Range(0.1f, 5)]
    private float nonXRHeight = 2f;

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
    private CharacterController controller { get; set; }
    private CollisionFlags flags;

    private Vector3 moveDirection = Vector3.zero;

    //Preview
    private bool XRMoveEnabledPrev = false;
    private Vector3 positionPrev;
    private Quaternion rotationPrev;

    #endregion

    #region Unity Functions
    void Awake()
    {
        _nonXR_interactor = this.GetComponent<NonXRInteraction>();

        this.positionPrev = this.transform.position;
        this.rotationPrev = this.transform.rotation;
    }
    private IEnumerator Start()
    {
#if UNITY_WEBGL
        isXREnabled = false;
#endif
        //Components
        myCamera = cameraMainTransform.GetComponent<Camera>();
        controller = GetComponent<CharacterController>();

        //Preview
        myTransform = transform;
        originalRotation = transform.localRotation;
        XRMoveEnabledPrev = isXREnabled;

        _nonXR_interactor.InitCamera(myCamera);

        yield return new WaitForSeconds(0.5f);

        capabilities = new WebXRDisplayCapabilities();
        ChangeXRStatus(isXREnabled);
    }

    void Update()
    {
        //Debug.Log("CAMERA ROTATION: " + cameraMainTransform.rotation);

        if (isXREnabled)
            MoveXR();
        else
            MoveNonXR();

        //Only in Unity editor:
        if (XRMoveEnabledPrev != isXREnabled)
        {
            Debug.Log("Update -> XRMoveEnabled: " + isXREnabled);
            ChangeXRStatus(isXREnabled);
            XRMoveEnabledPrev = isXREnabled;
        }
    }

    #endregion

    #region XR Events and functions

    //Events
    private void onXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
    {
        Debug.Log("onXRChange:: state: " + state.ToString());
        if (state == WebXRState.VR)
        {
            ChangeXRStatus(true);
        }
        else
        {
            ChangeXRStatus(false);
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

    }

    void OnDisable()
    {
        Debug.Log("Disable event!");
        WebXRManager.OnXRChange -= onXRChange;
        WebXRManager.OnXRCapabilitiesUpdate -= onXRCapabilitiesUpdate;
    }

    //Enable and disable
    private void EnableXRMove(bool _enable)
    {
        ChangeXRStatus(_enable);
        Debug.Log("EnableXRMove:: XRMoveEnabled: " + isXREnabled);
    }

    /// Enables rotation and translation control for desktop environments.
    /// For mobile environments, it enables rotation or translation according to
    /// the device capabilities.
    private void EnableAccordingToPlatform()
    {
        isXREnabled = capabilities.canPresentVR;
        //ChangeXRStatus(isXREnabled);
        Debug.Log("EnableAccordingToPlatform:: XRMoveEnabled: " + isXREnabled);
    }
    #endregion

    #region FUNCTIONS

    #region XR

    private void MoveXR()
    {
        //if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        // Traslation
        if (inputManagerLeftHand != null)
        {
            float moveX = inputManagerLeftHand.stick.x;
            float moveZ = inputManagerLeftHand.stick.y;

#if UNITY_EDITOR
            moveZ = moveZ * (-1);
#endif

            Quaternion cameraDirection = GetCameraRotation();
            //Debug.Log("THE CAMERA: " + cameraDirection);
            var dir = GetCameraRotation() * new Vector3(moveX, 0, moveZ);
            MoveCharacterController(dir);
        }

        // Rotation (No rotation on Y -> Up/Down)
        if (canRotateWithSticks && inputManagerRightHand != null)
            PlainXRRotation();

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
                angle = -angle;

            rotationX += angle;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            myTransform.localRotation = originalRotation * xQuaternion;
        }

        strickXPreb = Mathf.Abs(inputManagerRightHand.stick.x);
    }

    private void PlainXRRotation()
    {
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

    #region Change XR Status methods
    //Change XR Status
    private void ChangeXRStatus(bool setActive)
    {
        isXREnabled = setActive;
        ChangeShowCursor(!setActive);
        ChangeNotXRInteraction(!setActive);
        ChaneXRHandStatus(setActive);

        ResetPlayerTransform();
        ResetPlayerHeight();
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

    #region Auxiliar methods

    private Quaternion GetCameraRotation()
    {
        //TO-DO: IS NOT WORKING AFTER BUILD! In WebGL App the MainCamera Rotation is always the same.
        Quaternion rotation = cameraMainTransform.localRotation;

#if UNITY_WEBGL
        //After Export in WebGL cameraMainTransform will return the same value, but not cameraLeftTransform.
        rotation = cameraLeftTransform.localRotation;
#endif

        return rotation;
    }

    private void ResetPlayerTransform()
    {
        this.transform.rotation = rotationPrev;
        this.transform.position = positionPrev;
    }

    private void ResetPlayerHeight()
    {
        if (isXREnabled)
        {
            this.controller.height = height;
            this.controller.center = new Vector3(this.controller.center.x, height / 2, this.controller.center.z);
        }
        else
        {
            this.controller.height = nonXRHeight;
            this.controller.center = new Vector3(this.controller.center.x, ((-1) * (nonXRHeight / 3)), this.controller.center.z);
        }
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

    #endregion
}