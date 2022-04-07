using System;
using System.Collections;
using UnityEngine;

public class IndustrialControllerBehaviour : MonoBehaviour
{
    private Rigidbody IndustrialController_rb;
    private Animator _animator;
    private string ANIMATOR_BOOL = "isActivate";
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void ActivateIndustrialController()
    {
        //this.gameObject.SetActive(true);
        _animator.SetBool(ANIMATOR_BOOL, true);
        isActive = true;
    }

    public void DisactivateIndustrialController()
    {
        _animator.SetBool(ANIMATOR_BOOL, false);
        //if (isActive)
        //    StartCoroutine(DisactivateGameObject(1));
    }
    
    private IEnumerator DisactivateGameObject(int seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
        isActive = false;
    }
}