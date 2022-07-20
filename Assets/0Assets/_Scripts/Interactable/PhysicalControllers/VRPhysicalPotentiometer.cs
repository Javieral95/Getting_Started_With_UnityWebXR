using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class VRPhysicalPotentiometer : SpecialInteractable
{
    [Header("Physical Potentiometer settings"), Space(5)]
    public FloatEvent onChangeValue;
    public float InitValue = 0f;

    [Header("General options")]
    public TextMeshProUGUI DisplayScreenText;

    public bool RotateX = false;
    public bool RotateY = false;
    public bool RotateZ = false;

    public float MinGrades = 0;
    public float MaxGrades = 350;



    [Header("Non XR options")]
    public float sensitivity;

    private Vector3 initEulerAngles; //Init Rotation
    private Vector2 limits;
    private bool clicking;

    private bool changed;
    private float initRot;

    //XR set to true
    [Header("XR options")]
    private bool isXRInteraction;
    [SerializeField, Tooltip("(Global) Detect the hand's movement in this axis (Select only one Option!)")]
    private Axis XRRotationAxis;
    [Tooltip("Activate if need to rotate the object and put the rotate value in negative.")]
    public bool InvertAxis;
    private float rotationValueDivisor = 10;

    private Transform collisionObjectTransform;

    //Value
    public float value { get; private set; }
    public float RealValue { get { return (value / MaxGrades) * 100; } }

    //Unity Functions
    new void Start()
    {
        base.Start();
        initEulerAngles = this.transform.localEulerAngles;
        limits = new Vector2(MinGrades, MaxGrades);

        if (InitValue != 0)
        {
            value = (InitValue / 100) * MaxGrades;
            onChangeValue.Invoke(RealValue);
            DisplayScreenText.text = GetScreenValue();
            transform.localEulerAngles = GetRotationEularAngles(value);
        }
        
    }

    new void Update()
    {
        base.Update();
        if (clicking)
        {
            if (isXRInteraction)
                value += sensitivity * 1000 * (GetXRRotationValue()) * Time.deltaTime;
            else
                value += sensitivity * 1000 * Input.GetAxis("Mouse X") * Time.deltaTime;
            value = Mathf.Clamp(value, limits.x, limits.y);
            transform.localEulerAngles = GetRotationEularAngles(value);
            if (value != initRot) { changed = true; }
            if (changed && (value == limits.x || value == limits.y)) { clicking = false; }

            if (DisplayScreenText != null)
                DisplayScreenText.text = GetScreenValue();

            //Call Event
            onChangeValue?.Invoke(RealValue);
        }
    }

    //Public Functions
    public string GetScreenValue()
    {
        string ret = String.Format("{0:0.00}", (RealValue));
        return $"{ret}%";
    }

    //Auxiliar Functions

    private Vector3 GetRotationEularAngles(float value)
    {
        Vector3 ret = initEulerAngles;

        if (RotateX)
            ret.x = value;
        if (RotateY)
            ret.y = value;
        if (RotateZ)
            ret.z = value;

        return ret;
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
        clicking = true;
        changed = false;
        initRot = value;

        isXRInteraction = isXR;
    }

    public override void Throw(bool isXR = false)
    {
        Debug.Log("You cannot throw a Potentiometer >:(");
    }

    // XR
    private float GetXRRotationValue()
    {
        float potentiometerValue = 0;
        float handValue = 0;
        float ret = 0;

        if (this.XRRotationAxis == Axis.X)
        {
            potentiometerValue = this.transform.position.x;
            handValue = collisionObjectTransform.position.x;
        }
        else if (this.XRRotationAxis == Axis.Y)
        {
            potentiometerValue = this.transform.position.y;
            handValue = collisionObjectTransform.position.y;
        }
        else if (this.XRRotationAxis == Axis.Z)
        {
            potentiometerValue = this.transform.position.z;
            handValue = collisionObjectTransform.position.z;
        }

        ret = (handValue - potentiometerValue) / rotationValueDivisor;
        if (InvertAxis)
            return -ret;
        else
            return ret;
    }
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
}

