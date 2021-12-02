﻿//==================================================================================================================
//
// ControllerInteraction.cs
// Controls the process of grabbing things reading the input button from WebXRInputManager
// and firing the "grab" process when detecting a trigger.
//
//==================================================================================================================

using UnityEngine;
using System.Collections.Generic;
using WebXR;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(WebXRInputManager))]
public class ControllerInteraction : MonoBehaviour
{
    #region Properties
    private static GameManager gameManager;

    private FixedJoint attachJoint;
    private Rigidbody currentRigidBody;
    private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
    private WebXRInputManager inputManager;
    private Transform _transform;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private Animator controller_anim;

    [SerializeField, Range(1, 15)]
    private float throwForce = 9f;
    private bool isGrabbing = false;
    private GrabbableDoor grabbableDoor;
    private BreakInteraction breakInteraction;
    #endregion

    #region Unity Functions
    void Awake()
    {
        _transform = transform;
        attachJoint = GetComponent<FixedJoint>();
        controller_anim = GetComponent<Animator>();
        inputManager = GetComponent<WebXRInputManager>();
    }

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        float normalizedTime = inputManager.GetTriggerAxis();
        if (normalizedTime < 0.1f) { normalizedTime = inputManager.GetGripAxis(); }

        /*if(!inputManager.IsTriggerButtonInactive() || !inputManager.IsGripButtonInactive())
            Debug.Log("TriggerAxis: " + inputManager.GetTriggerAxis() + " - GripAxis: " + inputManager.GetGripAxis());*/

        if (inputManager.IsTriggerButtonDown() || inputManager.IsGripButtonDown()) { Interaction(true); }
        if (inputManager.IsTriggerButtonUp() || inputManager.IsGripButtonUp()) { Interaction(false); }

        if (inputManager.is_A_ButtonPressed())
            Throw(currentRigidBody);

        controller_anim.Play("Take", -1, normalizedTime);  // Use the controller button or axis position to manipulate the playback time for hand model.
    }


    void FixedUpdate()
    {
        if (!currentRigidBody) return;

        lastPosition = currentRigidBody.position;
        lastRotation = currentRigidBody.rotation;
    }

    private void LateUpdate()
    {
        if (CheckForBreakInteraction())
        {
            if (currentRigidBody != null)
            { //Ñapa, revisar proximo dia
                currentRigidBody.angularVelocity = Vector3.zero;
                currentRigidBody.velocity = Vector3.zero;
            }
            Drop(currentRigidBody);
        }
    }
    #endregion

    #region Functions and events

    //Collider events

    void OnTriggerEnter(Collider other)
    {
        if (!isInteractable(other.gameObject)) { return; }

        contactRigidBodies.Add(other.attachedRigidbody);
    }

    void OnTriggerExit(Collider other)
    {
        if (!isInteractable(other.gameObject)) { return; }

        contactRigidBodies.Remove(other.attachedRigidbody);
    }

    //Interaction Functions

    /// <summary>
    /// Take the nearest rigidbody and call the correct function (Drop, pick or press)
    /// </summary>
    /// <param name="isPicking"></param>
    private void Interaction(bool isPicking = true)
    {
        currentRigidBody = GetNearestRigidBody();
        if (!currentRigidBody) { return; }

        CheckSpecialObject();

        //Operation

        if (isPicking)
            Pickup(currentRigidBody);
        else
            Drop(currentRigidBody);
    }

    #region Interaction Auxiliar Functions
    /// <summary>
    /// Trigger when the user press a button
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void PressButton(Rigidbody currentRigidBody)
    {
        Debug.Log("You are pressing a button!!");
        TriggerInterface component = currentRigidBody.gameObject.GetComponent<TriggerInterface>();
        if (component != null)
            component.Press();
    }

    /// <summary>
    /// Pick the object and connect to hand.
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Pickup(Rigidbody currentRigidBody)
    {
        Debug.Log("You are grabbing an object!!");

        currentRigidBody.MovePosition(_transform.position);
        attachJoint.connectedBody = currentRigidBody;

        lastPosition = currentRigidBody.position;
        lastRotation = currentRigidBody.rotation;

        //Cancel Movement when it on hands
        currentRigidBody.velocity = Vector3.zero;
        currentRigidBody.angularVelocity = Vector3.zero;

        isGrabbing = true;
        grabbableDoor?.GrabObject();

    }

    /// <summary>
    /// Drop the object picked in hand
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Drop(Rigidbody currentRigidBody)
    {
        Debug.Log("You are dropping an object!!");

        attachJoint.connectedBody = null;
        if (grabbableDoor != null)
        {
            grabbableDoor?.DropObject();
            grabbableDoor = null;
        }
        else
        {
            currentRigidBody.velocity = (currentRigidBody.position - lastPosition) / Time.deltaTime;

            var deltaRotation = currentRigidBody.rotation * Quaternion.Inverse(lastRotation);
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            angle *= Mathf.Deg2Rad;
            currentRigidBody.angularVelocity = axis * angle / Time.deltaTime;

            breakInteraction?.ResetPosition();
            breakInteraction = null;

            currentRigidBody = null;
            isGrabbing = false;
        }
    }

    /// <summary>
    /// Apply an impulse force to currentRigidbody to throw it.
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Throw(Rigidbody currentRigidBody)
    {
        if (currentRigidBody != null && isGrabbing && !IsInteractableNotMovable(currentRigidBody.gameObject))
        {
            Debug.Log("You are throwing an object!!");
            currentRigidBody.AddForce(this.gameObject.transform.forward * throwForce, ForceMode.Impulse);

            attachJoint.connectedBody = null;
            currentRigidBody = null;
            isGrabbing = false;
        }
    }

    /// <summary>
    /// Of all the rigidbodies saved in contactRigidBodies list, take and return the nearest to player.
    /// </summary>
    /// <returns></returns>
    private Rigidbody GetNearestRigidBody()
    {
        Rigidbody nearestRigidBody = null;
        float minDistance = float.MaxValue;
        float distance;

        foreach (Rigidbody contactBody in contactRigidBodies)
        {
            distance = (contactBody.transform.position - _transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestRigidBody = contactBody;
            }
        }

        return nearestRigidBody;
    }

    private void CheckSpecialObject()
    {
        CheckIfHaveBreakProperty(currentRigidBody);
        grabbableDoor = currentRigidBody.GetComponent<GrabbableDoor>();   
    }

    /// <summary>
    /// Check if the object passed as parameter has the "Interactable" tag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool isInteractable(GameObject other)
    {
        return isInteractableObject(other) /*|| isInteractableTrigger(other)*/ || IsInteractableNotMovable(other);
    }

    /// <summary>
    /// Check if the object passed as parameter has the "Interactable" tag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool isInteractableObject(GameObject other)
    {
        return other.CompareTag(GameManager.INTERACTABLE_TAG);
    }
    /// <summary>
    /// Check if the object passed as parameter has the "InteractableTrigger" tag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool isInteractableTrigger(GameObject other)
    {
        return other.CompareTag(GameManager.INTERACTABLE_TRIGGER_TAG);
    }

    /// <summary>
    /// Check if the object passed as parameter has the "InteractableNotMovible" tag (For example, a door)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool IsInteractableNotMovable(GameObject other)
    {
        return other.CompareTag(GameManager.INTERACTABLE_NOT_MOVABLE_TAG);
    }

    /// <summary>
    /// Check if the current Rigid body have BreakInteraction script
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void CheckIfHaveBreakProperty(Rigidbody currentRigidBody)
    {
        if (breakInteraction == null)
            breakInteraction = currentRigidBody?.gameObject.GetComponent<BreakInteraction>();
    }

    /// <summary>
    /// Check if the object, with BreakInteractions script, exceed the maximum distance allowed.
    /// </summary>
    /// <returns></returns>
    private bool CheckForBreakInteraction()
    {
        if (breakInteraction != null)
            return breakInteraction.CheckNeedToBreak(_transform);
        else
            return false;
    }
    #endregion

    #endregion
}
