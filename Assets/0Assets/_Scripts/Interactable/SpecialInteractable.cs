﻿/***********************************************************************
 ********************* SPECIAL INTERACTABLE CLASS  *********************
 ****** Implements Special Interactable Interface (Grab, Drop Throw ****
 *********** methods) and Break interaction property *******************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SpecialInteractable : MonoBehaviour, ISpecialInteractable
{
    [Header("Break Interaction options")]
    [ConditionalHide("allowPermanentClick")]
    [Tooltip("If the interaction with the object must break when the user it moves away of it, put this property to True. ")]
    public bool HaveBreakInteraction;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [Tooltip("If Reference is null, the reference to calculate the distance will be the object's init position.")]
    public Transform Reference;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [Tooltip("If the scripts detects than need to break the interaction, will break that. Otherwise, need to manually check in child scripts (calling 'CheckNeedToBreak()' function) to break it and calling ResetPosition function")]
    public bool AuthomaticUpdate;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [SerializeField,  Range(0, 10)]
    private float maxDistance = 0f;

    [ConditionalHide("allowPermanentClick")]
    [SerializeField, Header("Other settings"), Tooltip("Set to True if the user will be able to take it and move with his hands"), Space(5)]
    private bool allowTranslation;
    [SerializeField, Tooltip("If is set to false with set Break Interaction to true with max distance = 0. Disactivate with elements like toggles.")]
    private bool allowPermanentClick = true;

    private bool _useObjectAsReference = false;
    private bool _needTobreak = false;

    private Transform _transform;
    protected Rigidbody _rb;
    private Vector3 initPosition;
    private Quaternion initRotation;

    private float _distance;

    // Unity Functions. Must call base.Start() and base.Awake() in other classes Start and Awake!!
    #region Unity Functions
    public virtual void Start()
    {
        _transform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();
        initPosition = _transform.position;
        initRotation = _transform.rotation;

        if (Reference != null) _useObjectAsReference = true; //Avoid Errors

        //If Only allow one click
        if (!allowPermanentClick)
        {
            HaveBreakInteraction = true;
            maxDistance = 0;
            allowTranslation = false;
        }
    }

    public virtual void Update()
    {
        if (HaveBreakInteraction)
        {
            //Calculate distance
            if (_useObjectAsReference)
                _distance = CalculateDistance(_transform.position, Reference.position);
            else
                _distance = CalculateDistance(_transform.position, initPosition);

            //Update status or reset position
            if (_distance >= maxDistance && !_needTobreak)
            {
                if (AuthomaticUpdate)
                    ResetPosition();
                else
                    _needTobreak = true;
            }
        }
    }
    #endregion

    #region Functions
    private float CalculateDistance(Vector3 object1, Vector3 object2)
    {
        return Vector3.Distance(object1, object2);
    }

    //Public
    public bool CanMoveIt()
    {
        return allowTranslation;
    }

    public void ResetPosition()
    {
        if (_useObjectAsReference)
        {
            _transform.position = Reference.position;
            _transform.rotation = Reference.rotation;
        }
        else
        {
            _transform.position = initPosition;
            _transform.rotation = initRotation;
        }

        _needTobreak = false;
        //Cancel forces
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public bool CheckNeedToBreak()
    {
        return _needTobreak;
    }

    public bool CheckNeedToBreak(Transform playerReference)
    {
        if (!HaveBreakInteraction) return false;

        bool ret = false;

        if (_useObjectAsReference)
            ret = _needTobreak || CalculateDistance(playerReference.position, Reference.position) >= maxDistance;
        else
            ret = _needTobreak || CalculateDistance(playerReference.position, initPosition) >= maxDistance;

        _needTobreak = false;
        return ret;
    }

    public bool CheckNeedToBreak(Vector3 playerReferencePos)
    {
        if (!HaveBreakInteraction) return false;

        bool ret = false;

        if (_useObjectAsReference)
            ret = _needTobreak || CalculateDistance(playerReferencePos, Reference.position) >= maxDistance;
        else
            ret = _needTobreak || CalculateDistance(playerReferencePos, initPosition) >= maxDistance;

        _needTobreak = false;
        return ret;
    }

    #endregion

    #region Abstract methods

    public abstract void Drop(bool isXR = false);

    public abstract void Grab(bool isXR = false);

    public abstract void Throw(bool isXR = false);

    #endregion
}

#region INTERFACE
public interface ISpecialInteractable
{
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Grab(bool isXR = false);
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Drop(bool isXR = false);
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Throw(bool isXR = false);


    bool CanMoveIt();
    void ResetPosition();
    bool CheckNeedToBreak();
    bool CheckNeedToBreak(Transform playerReference);
    bool CheckNeedToBreak(Vector3 playerReferencePos);
}
#endregion
