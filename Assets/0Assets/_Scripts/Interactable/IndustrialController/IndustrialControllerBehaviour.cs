using System;
using System.Collections;
using TMPro; //UI texts
using UnityEngine.UI; //Contiene UI
using UnityEngine;

public class IndustrialControllerBehaviour : MonoBehaviour
{
    public GameObject IndustrialController;
    public TextMeshProUGUI ScreenText;

    private Rigidbody IndustrialController_rb;
    private Animator _animator;
    private string ANIMATOR_BOOL = "isActivate";
    private bool isActive;
    private float value;

    private float MIN_ROTATION = 0f;
    private float MAX_ROTATION = 180f;
    private Quaternion _lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        value = 0f;
        _animator = GetComponent<Animator>();
        _lastRotation = this.transform.rotation;
        IndustrialController_rb = IndustrialController.GetComponent<Rigidbody>();
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ScreenText.text = GetScreenValue();
        if (value <= MIN_ROTATION || value >= MAX_ROTATION)
            ResetRotation();
        else
            _lastRotation = this.transform.rotation;
    }

    public void ActivateIndustrialController()
    {
        this.gameObject.SetActive(true);
        _animator.SetBool(ANIMATOR_BOOL, true);
        isActive = true;
    }

    public void DisactivateIndustrialController()
    {
        _animator.SetBool(ANIMATOR_BOOL, false);
        if (isActive)
            StartCoroutine(DisactivateGameObject(1));
    }
    
    private IEnumerator DisactivateGameObject(int seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
        isActive = false;
    }

    private string GetScreenValue()
    {
        string value = String.Format("{0:0.00}", GetValue());
        return $"{value}%";
    }

    private float GetValue()
    {
        var rotation = IndustrialController.transform.localRotation.eulerAngles;
        float tmpvalue = Math.Abs(rotation.y - rotation.x);
        value = tmpvalue;
        tmpvalue = Mathf.Clamp(value, MIN_ROTATION, MAX_ROTATION);

        return (tmpvalue / MAX_ROTATION) * 100;
    }

    private void ResetRotation()
    {
        IndustrialController_rb.angularVelocity = Vector3.zero;
        IndustrialController_rb.velocity = Vector3.zero;
        this.transform.rotation = _lastRotation;
    }
}