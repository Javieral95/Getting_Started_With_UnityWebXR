/***********************************************************************
 ************** SPECIAL INTERACTABLE OBJECT: Video Card ****************
 ****   Allow the user to change the video player in a VideoPlayer  ****
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TapeBehaviour : SpecialInteractable
{
    private bool isInsidePlayer;
    private bool isGrabbed;

    [Header("Title"), Tooltip("Tape title text object (if have it)")]
    public TextMeshProUGUI TapeTitleTextObject;
    [Tooltip("If is empty, wont replace TapeTitleTextObject content")]
    public string TapeTitle;

    [Header("Description")]
    public GameObject DescriptionObject;
    private TextMeshProUGUI panelText;
    private Animator descriptionAnimator;

    [Tooltip("Will replace TextMesh Description content"), TextArea]
    public string DescriptionText;

    private bool isDescriptionActive;
    [SerializeField, Tooltip("Seconds of waiting before descriptions disappearance"), Range(1, 5)]
    private int descriptionSeconds = 2;

    private PlayerController Player;
    private Transform cameraMainTransform;
    private Transform cameraLeftTransform;


    private Transform _previousReference;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Title
        if (TapeTitleTextObject == null) Debug.LogError($"({gameObject.name}) Didnt select a title object!!");
        else        
            if (!string.IsNullOrWhiteSpace(TapeTitle)) TapeTitleTextObject.text = TapeTitle;        

        //Description
        if (DescriptionObject == null) Debug.LogError($"({gameObject.name}) Didnt select a description object!!");
        else
        {
            panelText = DescriptionObject.GetComponentInChildren<TextMeshProUGUI>();
            descriptionAnimator = DescriptionObject.GetComponent<Animator>();

            if (!string.IsNullOrWhiteSpace(DescriptionText)) panelText.text = DescriptionText;

            DescriptionObject.SetActive(false);
        }

        //Need to refactor this (Same in InteractableTextPanel)
        Player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).GetComponent<PlayerController>();
        cameraMainTransform = Player.cameraMainTransform;
        cameraLeftTransform = Player.cameraLeftTransform;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (!isGrabbed && isInsidePlayer)
            FreezePosition();
        if (isDescriptionActive)
            RotateDescription();
    }

    // Special Interactable functions

    public override void Drop(bool isXR = false)
    {
        isGrabbed = false;
    }

    public override void Grab(bool isXR = false)
    {
        isGrabbed = true;
        _rb.constraints = RigidbodyConstraints.None;
    }

    public override void Throw(bool isXR = false)
    {
        if (!isDescriptionActive && DescriptionObject != null)
        {
            isDescriptionActive = true;
            DescriptionObject.SetActive(true);
            descriptionAnimator.SetBool(Constants.IS_ACTIVATE, true);
            StartCoroutine(HideDescription());
        }
    }

    // Auxiliar Functions

    private void FreezePosition()
    {
        if (_previousReference == null) return;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _rb.constraints = RigidbodyConstraints.FreezeAll;

        this.transform.position = _previousReference.position;
        this.transform.rotation = _previousReference.rotation;

        isGrabbed = false;
    }

    public void FreezePosition(Transform videoPlayerTransform)
    {
        _previousReference = videoPlayerTransform;
        FreezePosition();
    }

    public void PutOnLector(Transform videoPlayerTransform)
    {
        _previousReference = videoPlayerTransform;
        this.ForceBreak();
        isInsidePlayer = true;
    }

    public void PutOffLector()
    {
        isInsidePlayer = false;
    }

    // Description Functions

    private IEnumerator HideDescription()
    {
        yield return new WaitForSeconds(descriptionSeconds);

        descriptionAnimator.SetBool(Constants.IS_ACTIVATE, false);

        yield return new WaitForEndOfFrame();

        isDescriptionActive = false;
        DescriptionObject.SetActive(false);
    }

    private void RotateDescription()
    {
#if UNITY_WEBGL
        DescriptionObject.transform.LookAt(cameraLeftTransform);
#endif
#if UNITY_EDITOR
        DescriptionObject.transform.LookAt(cameraMainTransform);
#endif
    }

    protected void SetTapeTitle()
    {

    }
}
