using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(AudioSource))]
public class CollisionSound : MonoBehaviour
{
    public AudioClip CollisionSoundClip;
    public LayerMask HitLayers; // excluding for performance

    private AudioClip previewClip;

    private Collider _collider;
    private AudioSource _audioSource;
    private Rigidbody _rb;

    [SerializeField, Range(0,1)]
    private float collisionVolume = 1f;

    private bool _isHit;
    private float _rb_speed;
    // Start is called before the first frame update
    void Start()
    {
        _collider = this.gameObject.GetComponent<Collider>();
        _audioSource = this.gameObject.GetComponent<AudioSource>();
        _rb = this.gameObject.GetComponent<Rigidbody>();

        previewClip = _audioSource.clip;

        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1;
        _audioSource.volume = collisionVolume;
    }


    private void OnCollisionEnter(Collision collision)
    {
        _isHit = HitLayers == (HitLayers | (1 << collision.gameObject.layer));


        if (_isHit)
        {
            _rb_speed = _rb.velocity.magnitude;
            if (_rb_speed > 0)
            {
                _audioSource.clip = CollisionSoundClip;
                _audioSource.Play();
            }
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
       // _audioSource.clip = previewClip;
    }    
}
