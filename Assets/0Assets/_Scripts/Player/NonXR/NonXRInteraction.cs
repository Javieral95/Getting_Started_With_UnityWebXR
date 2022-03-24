//==================================================================================================================
//
// This scripts allow the player to grab objects and press buttons trigger without 3D glasses, using his mouse.
//
//==================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonXRInteraction : MonoBehaviour
{
    public Image CursorImage;

    private static GameManager gameManager;
    private Camera myCamera;

    //Non XR Interaction
    [SerializeField, Range(1, 15)]
    private float NonXR_throwForce = 9f;
    bool NonXR_isDragging;
    GameObject NonXR_selectedObject;
    Rigidbody NonXR_selectedObject_rb;
    Ray NonXR_ray;

    private Vector3 screenCenter = Vector3.zero;
    private float z_GrabObjectPosition = 1f;
    private Vector3 mousePos = Vector3.zero;

    private PhysicButton pressedButton;
    private GrabbableDoor grabbableDoor;
    private BreakInteraction breakInteraction;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        NonXR_isDragging = false;
        screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, z_GrabObjectPosition);

        Debug.Log("SCREEN CENTER: " + screenCenter);
    }

    // Update is called once per frame
    void Update()
    {
        CheckResizeWindow();
        NonXR_Interaction();
    }
    
    private void LateUpdate()
    {
        if (CheckForBreakInteraction())
        {
            if (NonXR_selectedObject_rb != null)
            { //Ñapa, revisar proximo dia
                CancelForces();
                breakInteraction?.ResetPosition();
                DropObject();
            }
        }
    }
    #endregion

    public void InitCamera(Camera controllerCamera)
    {
        if (myCamera == null)
            myCamera = controllerCamera;
    }

    private void NonXR_Interaction()
    {
        //TO-DO: Improve this method (detect object collisions and avoid errors)
        NonXR_ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Screen Center
        RaycastHit hit;

        //Grab object or press button
        if (Physics.Raycast(NonXR_ray.origin, NonXR_ray.direction, out hit, 5f) && Input.GetMouseButtonDown(0) && NonXR_isDragging == false)
        {
            if (hit.collider != null)
            {
                var tmpGameObject = hit.collider.gameObject;
                if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_TAG) || tmpGameObject.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG))
                {
                    //Grab
                    Debug.Log("You select: " + hit.collider.gameObject.name);
                    NonXR_selectedObject = hit.collider.gameObject;
                    NonXR_selectedObject_rb = NonXR_selectedObject.GetComponent<Rigidbody>();
                    NonXR_isDragging = true;

                    CancelForces();
                    CheckSpecialObject();
                }
                else if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_TRIGGER_TAG))
                    PressButton(tmpGameObject);

            }
            else
                ReleaseButton();

        }
       
        //Move Grabbed object
        MoveObject();

        //Drop Grabbed object
        if (Input.GetMouseButtonUp(0))
        {
            DropObject();
            //Button (trigger)
            ReleaseButton();
        }

        //Throw Grabbed object
        if (NonXR_isDragging && Input.GetMouseButtonDown(1) && NonXR_selectedObject_rb != null)
        {
            ThrowObject(NonXR_selectedObject_rb);
            DropObject();
        }
    }

    #region Auxiliar Methods

    private Vector3 GetMousePosition(GameObject target)
    {
        Vector3 screenPoint = Vector3.negativeInfinity;
        if (target != null)
            screenPoint = myCamera.WorldToScreenPoint(target.transform.position);

        float zPosition = (screenPoint.z != float.NegativeInfinity) ? screenPoint.z : 1f;
        zPosition = z_GrabObjectPosition;

        var tmpm = screenCenter;
        tmpm.z = zPosition;
        return myCamera.ScreenToWorldPoint(tmpm);
    }

    private void MoveObject()
    {
        if (NonXR_selectedObject == null) return;

        bool isNotMovable = NonXR_selectedObject.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG);

        if (NonXR_isDragging)
        {
            mousePos = GetMousePosition(NonXR_selectedObject);
            if (isNotMovable)
            {
                NonXR_selectedObject_rb.MovePosition(mousePos);
                //NonXR_selectedObject_rb.MoveRotation(Quaternion.Euler(new Vector3(0, 10, 0)));
            }
            else
                NonXR_selectedObject.transform.position = mousePos;
            
            //Cancel Movement when it on "hands"
            CancelForces();
        }
    }

    private void ThrowObject(Rigidbody object_rb)
    {
        if (grabbableDoor == null && !NonXR_selectedObject.gameObject.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG))
            object_rb.AddForce(myCamera.transform.forward * NonXR_throwForce, ForceMode.Impulse);
    }

    private void DropObject()
    {
        NonXR_isDragging = false;
        NonXR_selectedObject = null;
        NonXR_selectedObject_rb = null;

        grabbableDoor?.DropObject();
        grabbableDoor = null;
        breakInteraction = null;
    }

    private void CheckResizeWindow()
    {
        if (screenCenter.x != Screen.width / 2 || screenCenter.y != Screen.height / 2)
            screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }

    private void CancelForces()
    {
        NonXR_selectedObject_rb.velocity = Vector3.zero;
        NonXR_selectedObject_rb.angularVelocity = Vector3.zero;
    }

    private void CheckSpecialObject()
    {
        if (NonXR_selectedObject.gameObject.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG))
            grabbableDoor = NonXR_selectedObject.GetComponent<GrabbableDoor>();

        CheckIfHaveBreakProperty(NonXR_selectedObject_rb);
    }

    /// <summary>
    /// If the select object is a button, this functions call his Press event.
    /// </summary>
    /// <param name="button"></param>
    private void PressButton(GameObject button)
    {
        Debug.Log("You are pressing a button!!");
        pressedButton = button.GetComponent<PhysicButton>();
        pressedButton?.PressButton();
    }

    /// <summary>
    /// If the select object is a button, this functions call his Release event.
    /// </summary>
    private void ReleaseButton()
    {
        pressedButton?.ReleaseButton();
        pressedButton = null;
    }


    /// <summary>
    /// Check if the current Rigid body have BreakInteraction script
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void CheckIfHaveBreakProperty(Rigidbody currentRigidBody)
    {
        if (breakInteraction == null)
            breakInteraction = currentRigidBody.gameObject.GetComponent<BreakInteraction>();
    }

    /// <summary>
    /// Check if the object, with BreakInteractions script, exceed the maximum distance allowed.
    /// </summary>
    /// <returns></returns>
    private bool CheckForBreakInteraction()
    {
        if (breakInteraction != null)
            return breakInteraction.CheckNeedToBreak(mousePos);
        else
            return false;
    }
    #endregion
}
