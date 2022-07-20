using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AppearanceBehaviour : MonoBehaviour
{
    private Animator _animator;    
    private bool _isActivate;

    private AudioSource _audioSource;
    private bool _haveAudio;
    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _audioSource = this.GetComponent<AudioSource>();
        _haveAudio = _audioSource != null;

    }


    public void ChangeAnimationStatus(bool status)
    {
        _animator.SetBool(Constants.IS_ACTIVATE, status);
        if (_haveAudio)
            _audioSource.Play();
    }
}
