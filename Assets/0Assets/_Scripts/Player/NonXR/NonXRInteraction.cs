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

    private PhysicButton pressedButton;
    private NotMovable objectNotMovable;

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
                    if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG))
                        objectNotMovable = NonXR_selectedObject.GetComponent<NotMovable>();
                }
                else if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_TRIGGER_TAG))
                {
                    //Press
                    Debug.Log("You are pressing a button!!");
                    pressedButton = tmpGameObject.GetComponent<PhysicButton>();
                    if (pressedButton != null)
                        pressedButton.PressButton();
                    
                }
            }
            else
            {
                if (pressedButton != null)
                {
                    pressedButton.ReleaseButton();
                    pressedButton = null;
                }
            }

        }

        //Move Grabbed object
        if (NonXR_isDragging)
            NonXR_selectedObject.transform.position = GetMousePosition(NonXR_selectedObject);

        //Drop Grabbed object
        if (Input.GetMouseButtonUp(0))
        {
            DropObject();
            //Button (trigger)
            if (pressedButton != null)
            {
                pressedButton.ReleaseButton();
                pressedButton = null;
            }
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

    private void ThrowObject(Rigidbody object_rb)
    {
        if (objectNotMovable == null)
            object_rb.AddForce(myCamera.transform.forward * NonXR_throwForce, ForceMode.Impulse);
    }

    private void DropObject()
    {
        NonXR_isDragging = false;
        NonXR_selectedObject = null;
        NonXR_selectedObject_rb = null;

        objectNotMovable?.DropObject();
        objectNotMovable = null;
    }

    private void CheckResizeWindow()
    {
        if (screenCenter.x != Screen.width / 2 || screenCenter.y != Screen.height / 2)
            screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }
    #endregion
}
