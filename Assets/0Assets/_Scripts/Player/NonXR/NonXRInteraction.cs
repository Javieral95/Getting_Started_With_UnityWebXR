//==================================================================================================================
//
// This scripts allow the player to grab objects and press buttons trigger without 3D glasses, using his mouse.
//
//==================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonXRInteraction : MonoBehaviour
{
    private static GameManager gameManager;
    private Camera myCamera;

    //Non XR Interaction
    [SerializeField, Range(1, 15)]
    private float NonXR_throwForce = 9f;
    bool NonXR_isDragging;
    GameObject NonXR_selectedObject;
    Rigidbody NonXR_selectedObject_rb;
    Ray NonXR_ray;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);

    private Vector3 mousePos = Vector3.zero;
    private float z_GrabObjectPosition = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        NonXR_isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        NonXR_Interaction();
    }

    public void InitCamera(Camera controllerCamera)
    {
        if(myCamera == null)
            myCamera = controllerCamera;
    }

    private void NonXR_Interaction()
    {
        //TO-DO: Improve this method (detect object collisions and avoid errors)
        NonXR_ray = myCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Grab object or press button
        if (Physics.Raycast(NonXR_ray.origin, NonXR_ray.direction, out hit, 5f) && Input.GetMouseButtonDown(0) && NonXR_isDragging == false)
        {
            if (hit.collider != null)
            {
                var tmpGameObject = hit.collider.gameObject;
                if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_TAG))
                {
                    //Grab
                    Debug.Log("You select: " + hit.collider.gameObject.name);
                    NonXR_selectedObject = hit.collider.gameObject;
                    NonXR_selectedObject_rb = NonXR_selectedObject.GetComponent<Rigidbody>();
                    NonXR_isDragging = true;
                }
                else if (tmpGameObject.CompareTag(GameManager.INTERACTABLE_TRIGGER_TAG))
                {
                    //Press
                    Debug.Log("You are pressing a button!!");
                    TriggerInterface component = tmpGameObject.GetComponent<TriggerInterface>();
                    if (component != null)
                        component.Press();

                }
            }
        }

        //Move Grabbed object
        if (NonXR_isDragging)
        {
            mousePos = GetMousePosition(NonXR_selectedObject);
            NonXR_selectedObject.transform.position = mousePos;
        }

        //Drop Grabbed object
        if (Input.GetMouseButtonUp(0))
            DropObject();

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

        return myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPosition));
    }

    private void ThrowObject(Rigidbody object_rb)
    {
        object_rb.AddForce(Camera.main.transform.forward * NonXR_throwForce, ForceMode.Impulse);
    }

    private void DropObject()
    {
        NonXR_isDragging = false;
        NonXR_selectedObject = null;
        NonXR_selectedObject_rb = null;
    }
    #endregion
}
