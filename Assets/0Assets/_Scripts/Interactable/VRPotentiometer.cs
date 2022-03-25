using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VRPotentiometer : SpecialInteractable
{
    public TextMeshProUGUI DisplayScreenText;

    public bool RotateX = false;
    public bool RotateY = false;
    public bool RotateZ = false;

    public float MinGrades = 0;
    public float MaxGrades = 350;

    public float sensitivity;

    private Vector3 initEulerAngles; //Init Rotation
    private Vector2 limits;
    private Rigidbody _rb;
    private bool clicking;

    private bool changed;
    private float initRot;

    //XR set to true
    private bool isXRInteraction;
    [SerializeField]
    private Axis XRRotationAxis;
    private float rotationValueDivisor = 10;
    
    private Transform collisionObjectTransform;

    //Value
    private float val = .0f;

    new void Start()
    {
        base.Start();
        initEulerAngles = this.transform.localEulerAngles;
        limits = new Vector2(MinGrades, MaxGrades);
        _rb = this.gameObject.GetComponent<Rigidbody>();
    }

    new void Update()
    {
        base.Update();
        if (clicking)
        {
            if (isXRInteraction)
                val += sensitivity * 1000 * (GetXRRotationValue()) * Time.deltaTime;
            else
                val += sensitivity * 1000 * Input.GetAxis("Mouse X") * Time.deltaTime;
            val = Mathf.Clamp(val, limits.x, limits.y);
            transform.localEulerAngles = GetRotationEularAngles(val);
            if (val != initRot) { changed = true; }
            if (changed && (val == limits.x || val == limits.y)) { clicking = false; }

            DisplayScreenText.text = GetScreenValue();
        }
    }

    //Public Functions

    public float GetValue()
    {
        return val;
    }

    public string GetScreenValue()
    {
        string ret = String.Format("{0:0.00}", ((GetValue()/MaxGrades)*100));
        return $"{ret}%";
    }

    //Auxiliar Functions

    private Vector3 GetRotationEularAngles(float value)
    {
        Vector3 ret = initEulerAngles;

        if (RotateX)
            ret.x = val;
        if (RotateY)
            ret.y = val;
        if (RotateZ)
            ret.z = val;

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
        initRot = val;

        isXRInteraction = isXR;
    }

    public override void Throw(bool isXR = false)
    {
        throw new System.NotImplementedException();
    }

    private float GetXRRotationValue()
    {
        float potentiometerValue = 0;
        float handValue = 0;

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

        return (handValue - potentiometerValue)/rotationValueDivisor;
    }
    private void OnTriggerEnter(Collider other)
    {
        var tmp = other.gameObject;
        if (tmp.CompareTag("PlayerHands") && collisionObjectTransform == null)
            collisionObjectTransform = tmp.GetComponent<Transform>();
    }
    private void OnTriggerExit(Collider other)
    {
        if(!clicking)
            collisionObjectTransform = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if(collisionObjectTransform == null && other.gameObject.CompareTag("PlayerHands"))
            collisionObjectTransform = other.gameObject.GetComponent<Transform>();
    }
}

enum Axis// your custom enumeration
{
    X,
    Y,
    Z
};