using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialInteractable : MonoBehaviour, ISpecialInteractable
{
    [Tooltip("If Reference is null, the reference to calculate the distance will be the object's init position.")]
    public Transform Reference;

    [Tooltip("If the interaction with the object must break when the user it moves away of it, put this property to True. ")]
    public bool HaveBreakInteraction = true;
    [Tooltip("If the scripts detects than need to break the interaction, will break that. Otherwise, need to break calling ResetPosition function")]
    public bool AuthomaticUpdate = false;

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
        if (_useObjectAsReference)
            return _needTobreak || CalculateDistance(playerReference.position, Reference.position) >= maxDistance;
        else
            return _needTobreak || CalculateDistance(playerReference.position, initPosition) >= maxDistance;
    }

    public bool CheckNeedToBreak(Vector3 playerReferencePos)
    {
        if (_useObjectAsReference)
            return _needTobreak || CalculateDistance(playerReferencePos, Reference.position) >= maxDistance;
        else
            return _needTobreak || CalculateDistance(playerReferencePos, initPosition) >= maxDistance;
    }

    #endregion

    #region Abstract methods

    public abstract void Drop();

    public abstract void Grab();

    public abstract void Throw();

    #endregion
}


public interface ISpecialInteractable
{
    void Grab();
    void Drop();
    void Throw();


    void ResetPosition();
    bool CheckNeedToBreak();
    bool CheckNeedToBreak(Transform playerReference);
    bool CheckNeedToBreak(Vector3 playerReferencePos);
}
