//==================================================================================================================
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
    private static PlayerController player;

    private FixedJoint attachJoint;
    private Rigidbody currentRigidBody;
    private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
    private WebXRInputManager inputManager;
    private Transform _transform;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private Animator controller_anim;

    [SerializeField, Range(1, 15)]
    private readonly float throwForce = 9f;
    private bool isGrabbing = false;
    private ISpecialInteractable specialInteractable;
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
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        float normalizedTime = inputManager.GetTriggerAxis();
        if (normalizedTime < 0.1f) { normalizedTime = inputManager.GetGripAxis(); }

        // Interaction
        if (inputManager.IsGripButtonDown()) { Interaction(true); }
        if (inputManager.IsGripButtonUp()) { Interaction(false); }

        if (inputManager.Is_A_ButtonPressed())
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
                Drop(currentRigidBody);
            }
        }
    }
    #endregion

    #region Functions and events

    //Collider events

    void OnTriggerEnter(Collider other)
    {
        if (!IsInteractable(other.gameObject)) { return; }

        contactRigidBodies.Add(other.attachedRigidbody);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsInteractable(other.gameObject)) { return; }

        contactRigidBodies.Remove(other.attachedRigidbody);
    }

    //Interaction Functions

    /// <summary>
    /// Take the nearest rigidbody and call the correct function (Drop, pick or press)
    /// </summary>
    /// <param name="isPicking"></param>
    private void Interaction(bool isPicking = true)
    {
        // Take object
        if (currentRigidBody != null) Drop(currentRigidBody);
        currentRigidBody = Utils.GetNearestObject(_transform, contactRigidBodies);

        if (!currentRigidBody) { return; }

        // Special Interactable
        CheckSpecialObject();

        // Operation
        if (isPicking)
            Pickup(currentRigidBody);
        else
            Drop(currentRigidBody);
    }

    #region Interaction Auxiliar Functions
    /// <summary>
    /// Pick the object and connect to hand.
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Pickup(Rigidbody currentRigidBody)
    {
        Debug.Log("You are grabbing an object!!");

        // Special Interactable
        specialInteractable?.Grab(true);

        if (specialInteractable == null || specialInteractable.CanMoveIt())
        {
            currentRigidBody.MovePosition(_transform.position);
            attachJoint.connectedBody = currentRigidBody;

            lastPosition = currentRigidBody.position;
            lastRotation = currentRigidBody.rotation;

            //Cancel Movement when it on hands
            currentRigidBody.velocity = Vector3.zero;
            currentRigidBody.angularVelocity = Vector3.zero;

            isGrabbing = true;
        }


    }

    /// <summary>
    /// Drop the object picked in hand
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Drop(Rigidbody currentRigidBody)
    {
        Debug.Log("You are dropping an object!!");

        attachJoint.connectedBody = null;
        currentRigidBody.velocity = ((currentRigidBody.position - lastPosition) / Time.deltaTime);

        var deltaRotation = currentRigidBody.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        angle *= Mathf.Deg2Rad;
        currentRigidBody.angularVelocity = axis * angle / Time.deltaTime;

        // Delete references
        specialInteractable?.Drop(true);
        specialInteractable = null;

        currentRigidBody = null;
        isGrabbing = false;

    }

    /// <summary>
    /// Apply an impulse force to currentRigidbody to throw it.
    /// </summary>
    /// <param name="currentRigidBody"></param>
    private void Throw(Rigidbody currentRigidBody)
    {
        specialInteractable?.Throw(true);
        if (currentRigidBody != null && isGrabbing && !IsInteractableNotMovable(currentRigidBody.gameObject))
        {
            Debug.Log("You are throwing an object!!");
            if (specialInteractable == null || specialInteractable.CanThrowIt())
            {
                currentRigidBody.AddForce(this.gameObject.transform.forward * throwForce, ForceMode.Impulse);
                attachJoint.connectedBody = null;
                currentRigidBody = null;
                isGrabbing = false;
            }
        }


    }

    private void CheckSpecialObject()
    {
        specialInteractable = currentRigidBody.GetComponent<SpecialInteractable>();
    }

    /// <summary>
    /// Check if the object passed as parameter has the "Interactable" tag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool IsInteractable(GameObject other)
    {
        return IsInteractableObject(other) || IsInteractableNotMovable(other);
    }

    /// <summary>
    /// Check if the object passed as parameter has the "Interactable" tag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool IsInteractableObject(GameObject other)
    {
        return other.CompareTag(Constants.INTERACTABLE_TAG);
    }

    /// <summary>
    /// Check if the object passed as parameter has the "InteractableNotMovible" tag (For example, a door)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool IsInteractableNotMovable(GameObject other)
    {
        return other.CompareTag(Constants.INTERACTABLE_NOT_MOVABLE_TAG);
    }

    /// <summary>
    /// Check if the special interactable object have the Break Interaction check and if exceed the maximum distance allowed.
    /// </summary>
    /// <returns></returns>
    private bool CheckForBreakInteraction()
    {
        if (specialInteractable != null)
            return specialInteractable.CheckNeedToBreak(_transform);
        else
            return false;
    }
    #endregion

    #endregion
}
