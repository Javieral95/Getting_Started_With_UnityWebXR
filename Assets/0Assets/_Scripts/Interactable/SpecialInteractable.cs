using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialInteractable : MonoBehaviour, ISpecialInteractable
{
    [Header("Break Interaction options")]
    [SerializeField, Tooltip("Set to True if the user will be able to take it and move with his hands")]
    private bool allowTranslation;
    [Tooltip("If Reference is null, the reference to calculate the distance will be the object's init position.")]
    public Transform Reference;

    [Tooltip("If the interaction with the object must break when the user it moves away of it, put this property to True. ")]
    public bool HaveBreakInteraction;
    [Tooltip("If the scripts detects than need to break the interaction, will break that. Otherwise, need to break calling ResetPosition function")]
    public bool AuthomaticUpdate;

    [SerializeField, Range(0, 10)]
    private float maxDistance = 1f;
    private bool _useObjectAsReference = false;
    private bool _needTobreak = false;

    private Transform _transform;
    private Vector3 initPosition;

    private float _distance;

    // Unity Functions. Must call base.Start() and base.Awake() in other classes Start and Awake!!
    #region Unity Functions
    public virtual void Start()
    {
        _transform = GetComponent<Transform>();
        initPosition = _transform.position;

        if (Reference != null) _useObjectAsReference = true; //Avoid Errors
    }

    public virtual void Update()
    {
        if (HaveBreakInteraction)
        {
            if (_useObjectAsReference)
                _distance = CalculateDistance(_transform.position, Reference.position);
            else
                _distance = CalculateDistance(_transform.position, initPosition);

            if (_distance >= maxDistance && !_needTobreak)
                _needTobreak = true;

            else if (_needTobreak && AuthomaticUpdate)
                ResetPosition();
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
            _transform.position = Reference.position;
        else
            _transform.position = initPosition;

        _needTobreak = false;
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

    public abstract void Drop(bool isXR=false);

    public abstract void Grab(bool isXR = false);

    public abstract void Throw(bool isXR = false);

    #endregion
}


public interface ISpecialInteractable
{
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Grab(bool isXR=false);
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Drop(bool isXR=false);
    /// <summary>
    /// Use isXR if the object have an special behaviour when the user is playing using VR Headset
    /// </summary>
    /// <param name="isXR"></param>
    void Throw(bool isXR=false);


    bool CanMoveIt();
    void ResetPosition();
    bool CheckNeedToBreak();
    bool CheckNeedToBreak(Transform playerReference);
    bool CheckNeedToBreak(Vector3 playerReferencePos);
}
