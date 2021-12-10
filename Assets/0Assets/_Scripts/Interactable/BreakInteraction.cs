//==================================================================================================================
//
// Avoid moving objects further than the allowed distance
//
//==================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakInteraction : MonoBehaviour
{
    public bool UseObjectAsReference = false;
    [Tooltip("If the scripts detects than need to break the interaction, will break that. Otherwise, need to break calling ResetPosition function")]
    public bool AuthomaticUpdate = false;
    public Transform Reference;

    [SerializeField, Range(0, 10)]
    private float maxDistance = 1f;
    private bool _needTobreak = false;

    private Transform _transform;
    private Vector3 initPosition;

    private float _distance;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        initPosition = _transform.position;

        if (Reference == null) UseObjectAsReference = false; //Avoid Errors
    }

    // Update is called once per frame
    void Update()
    {
        if (UseObjectAsReference)
            _distance = CalculateDistance(_transform.position, Reference.position);
        else
            _distance = CalculateDistance(_transform.position, initPosition);

        if (_distance >= maxDistance && !_needTobreak)
            _needTobreak = true;

        else if (_needTobreak && AuthomaticUpdate)
            ResetPosition();
    }

    private float CalculateDistance(Vector3 object1, Vector3 object2)
    {
        return Vector3.Distance(object1, object2);
    }

    public void ResetPosition()
    {
        if (UseObjectAsReference)
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
        if (UseObjectAsReference)
            return _needTobreak || CalculateDistance(playerReference.position, Reference.position) >= maxDistance;
        else
            return _needTobreak || CalculateDistance(playerReference.position, initPosition) >= maxDistance;
    }
    
    public bool CheckNeedToBreak(Vector3 playerReferencePos)
    {
        if (UseObjectAsReference)
            return _needTobreak || CalculateDistance(playerReferencePos, Reference.position) >= maxDistance;
        else
            return _needTobreak || CalculateDistance(playerReferencePos, initPosition) >= maxDistance;
    }
}
