/***********************************************************************
 ********************* SPECIAL INTERACTABLE CLASS  *********************
 ****** Implements Special Interactable Interface (Grab, Drop Throw ****
 *********** methods) and Break interaction property *******************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class SpecialInteractable : MonoBehaviour, ISpecialInteractable
{
    [Tooltip("Use this property to disable the interaction with this object")]
    public bool CanInteract = true;

    [Header("Break Interaction options")]
    [ConditionalHide("allowPermanentClick")]
    [Tooltip(
        "If the interaction with the object must break when the user it moves away of it, put this property to True. ")]
    public bool HaveBreakInteraction;

    [ConditionalHide("allowPermanentClick")]
    [Tooltip(
    "Only Breaks the interaction manually using 'ForceBreak()' function")]
    public bool ManuallyBreakInteraction;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [Tooltip("If Reference is null, the reference to calculate the distance will be the object's init position.")]
    public Transform Reference;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [Tooltip(
        "If the scripts detects than need to break the interaction, will break that. Otherwise, need to manually check in child scripts (calling 'CheckNeedToBreak()' function) to break it and calling ResetPosition function")]
    public bool AuthomaticUpdate;

    [ConditionalHide("HaveBreakInteraction", true, ConditionalSourceField2 = "allowPermanentClick")]
    [SerializeField, Range(0, 10)]
    private float maxDistance = 0f;

    [ConditionalHide("allowPermanentClick")]
    [SerializeField, Header("Other settings"),
     Tooltip("Set to True if the user will be able to take it and move with his hands"), Space(5)]
    protected bool allowTranslation;

    [SerializeField,
     Tooltip(
         "If is set to false with set Break Interaction to true with max distance = 0. Disactivate with elements like toggles.")]
    protected bool allowPermanentClick = true;

    [SerializeField,
 Tooltip(
     "If is set to true the user can apply force to throw this object."), ConditionalHide("allowPermanentClick")]
    protected bool canThrow;

    private bool _useObjectAsReference = false;
    private bool _needTobreak = false;
    private bool _isDestroyed;

    protected Transform _transform;
    protected Rigidbody _rb;
    protected Vector3 initPosition;
    protected Quaternion initRotation;

    private float _distance;

    // If have a sound
    protected AudioSource _audioSource;
    protected bool haveSound;

    // Unity Functions. Must call base.Start() and base.Awake() in other classes Start and Awake!!

    #region Unity Functions

    protected virtual void Start()
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

        _audioSource = this.GetComponent<AudioSource>();
        haveSound = _audioSource != null;
    }

    protected virtual void Update()
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

    public bool CanThrowIt()
    {
        return canThrow;
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
        if (!HaveBreakInteraction && !ManuallyBreakInteraction) return false;

        bool ret = false;

        try
        {
            if (!ManuallyBreakInteraction)
            {
                if (_useObjectAsReference)
                    ret = _needTobreak || CalculateDistance(playerReference.position, Reference.position) >= maxDistance;
                else
                    ret = _needTobreak || CalculateDistance(playerReference.position, initPosition) >= maxDistance;
            }
            else ret = _needTobreak;

            _needTobreak = false;
        }
        catch (MissingReferenceException e)
        {
            Debug.LogError(e.Message);
            Destroy(gameObject);
            return false;
        }

        return ret;
    }

    public bool CheckNeedToBreak(Vector3 playerReferencePos)
    {
        if (!HaveBreakInteraction && !ManuallyBreakInteraction) return false;

        bool ret = false;

        try
        {
            if (!ManuallyBreakInteraction)
            {
                if (_useObjectAsReference)
                    ret = _needTobreak || CalculateDistance(playerReferencePos, Reference.position) >= maxDistance;
                else
                    ret = _needTobreak || CalculateDistance(playerReferencePos, initPosition) >= maxDistance;
            }
            else ret = _needTobreak;


            _needTobreak = false;

        }
        catch (MissingReferenceException e)
        {
            Debug.LogError(e.Message);
            Destroy(gameObject);
            return false;
        }

        return ret;
    }

    /// <summary>
    /// Breaks the interaction in the next frame
    /// </summary>
    protected void ForceBreak()
    {
        _needTobreak = true;
    }

    public bool IsDestroyed()
    {
        return _isDestroyed;
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }

    protected void PlaySound()
    {
        if (haveSound)
            _audioSource.PlayOneShot(_audioSource.clip);
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
    bool CanThrowIt();
    void ResetPosition();
    bool CheckNeedToBreak();
    bool CheckNeedToBreak(Transform playerReference);
    bool CheckNeedToBreak(Vector3 playerReferencePos);
    bool IsDestroyed();
}

#endregion