using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStepSound : MonoBehaviour
{
    public List<AudioClip> StepsAudioClips;
    private AudioSource _audioSource;
    private PlayerController playerController;

    public bool isMoving;
    [SerializeField, Tooltip("Check if the Steps Audio Clips sound only contain a clip for a right step and a clip for left step")]
    private bool onlyRightLeftSounds;
    private int clipIndex;
    private int soundCount;

    private float _t;
    private float _speed = 5f;
    [SerializeField]
    private float _speedModifier = 1f;

    private void OnEnable()
    {
        _t = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController != null) _speed = playerController.GetSpeed();

        if (StepsAudioClips == null) StepsAudioClips = new List<AudioClip>();
        _audioSource = this.GetComponent<AudioSource>();

        soundCount = StepsAudioClips.Count;

        if (onlyRightLeftSounds && soundCount != 2)
            onlyRightLeftSounds = false;

        isMoving = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _t += Time.deltaTime * _speed * _speedModifier;

        //REVISAR
        if (soundCount > 0 && (_t >= 1f && isMoving && !_audioSource.isPlaying))
        {
            _t = _t % 1f;
            _audioSource.PlayOneShot(GetFootAudioClip());
        }

    }

    private AudioClip GetFootAudioClip()
    {
        if (onlyRightLeftSounds)
        {

            if (clipIndex == 0)
            {
                clipIndex = 1;
                return StepsAudioClips[1];
            }
            else
            {
                clipIndex = 0;
                return StepsAudioClips[1];
            }
        }
        else
        {
            clipIndex = Random.Range(0, StepsAudioClips.Count - 1);
            return StepsAudioClips[clipIndex];
        }
    }
}
