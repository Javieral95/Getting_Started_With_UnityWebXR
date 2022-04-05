using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class VRSlider : SpecialInteractable
{
    public FloatEvent onChangeValue;

    [Header("General options")]

    public TextMeshProUGUI DisplayScreenText;
    public float Sensitivity;

    [Header("Non XR Options")]
    [Tooltip("If is setting to true check input Mouse Y instead of input Mouse X")]
    public bool IsVerticalSlider;

    private Vector2 limits;
    private bool clicking;
    private bool changed;
    private float initPos;

    //XR set to true
    [Header("XR options")]
    [SerializeField, Tooltip("(Global) Detect the hand's movement in this axis (Select only one Option!)")]
    private Axis GlobalXRMoveAxis;
    [Tooltip("Activate if need to rotate the object and put the movement value in negative.")]
    public bool InvertAxis;
    private bool isXRInteraction;
    private Rigidbody _rb;
    private Transform collisionObjectTransform;
    private float moveValueDivisor = 1;

    public float value { get; private set; }

    //Unity Functions
    new void Start()
    {
        base.Start();
        //InitLimits();
        limits = new Vector2(transform.localPosition.z, -transform.localPosition.z);
        _rb = this.gameObject.GetComponent<Rigidbody>();
    }

    new void Update()
    {
        base.Update();
        if (clicking)
        {
            transform.localPosition += Vector3.forward * Sensitivity * GetMovementValue() * Time.deltaTime;
            transform.localPosition = GetNewPosition();

            if (transform.localPosition.x != initPos) { changed = true; }
            if (changed && (CheckLimits())) { clicking = false; }

            UpdateValue();

            if (DisplayScreenText != null)
                DisplayScreenText.text = $"{String.Format("{0:0.00}", value)}%";
        }
    }

    //Private Functions
    private float GetMovementValue()
    {
        if (isXRInteraction)
        {
            float sliderValue = 0;
            float handValue = 0;
            float ret = 0;

            switch (GlobalXRMoveAxis)
            {
                case (Axis.X):
                    sliderValue = this.transform.position.x;
                    handValue = collisionObjectTransform.position.x;
                    break;
                case (Axis.Y):
                    sliderValue = this.transform.position.y;
                    handValue = collisionObjectTransform.position.y;
                    break;
                case (Axis.Z):
                    sliderValue = this.transform.position.z;
                    handValue = collisionObjectTransform.position.z;
                    break;
                default:
                    return 0;
            }
            ret = (handValue - sliderValue) / moveValueDivisor;
            if (InvertAxis)
                return -ret;
            else
                return ret;
        }
        else
        {
            if (IsVerticalSlider)
                return Input.GetAxis("Mouse Y");
            else
                return Input.GetAxis("Mouse X");
        }
    }

    private void UpdateValue()
    {
        //value = (Math.Abs(transform.localPosition.z) / (-limits.y + limits.x)) * 100;

        var limit = Math.Abs(limits.x);
        value = ((limit + transform.localPosition.z) / (2 * limit)) * 100;

        //Call Event
        onChangeValue?.Invoke(value);
    }

    private bool CheckLimits()
    {
        return transform.localPosition.z == limits.x || transform.localPosition.z == limits.y;
    }

    private Vector3 GetNewPosition()
    {
        return new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, limits.x, limits.y));
    }
    //Special Interactable Object Functions

    public override void Drop(bool isXR = false)
    {
        clicking = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        collisionObjectTransform = null;
    }

    public override void Grab(bool isXR = false)
    {
        initPos = transform.localPosition.x;
        clicking = true;
        changed = false;

        isXRInteraction = isXR;
    }

    public override void Throw(bool isXR = false)
    {
        Debug.Log("You cannot throw a Slider >:(");
    }

    // XR
    private void OnTriggerEnter(Collider other)
    {
        var tmp = other.gameObject;
        if (tmp.CompareTag("PlayerHands") && collisionObjectTransform == null)
            collisionObjectTransform = tmp.GetComponent<Transform>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!clicking)
            collisionObjectTransform = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (collisionObjectTransform == null && other.gameObject.CompareTag("PlayerHands"))
            collisionObjectTransform = other.gameObject.GetComponent<Transform>();
    }
};