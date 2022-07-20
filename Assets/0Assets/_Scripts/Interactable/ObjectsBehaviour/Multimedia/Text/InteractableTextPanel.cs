/***********************************************************************
 ********** Use it to appear a custom text above the object ************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InteractableTextPanel : SpecialInteractable
{
    protected GameManager gameManager;

    [Header("Text panel settings: ")]
    [Tooltip("Put it on true to use only Panels with texts, disactivate to use only a custom object")]
    public bool UseTextPanels;

    public GameObject PanelObject;
    private Transform previousPanelTransform;

    [TextArea, Tooltip("This texts will show inside the panels following the Panel Gameobject childs order.")]
    [ConditionalHide("UseTextPanels")]
    public string[] ObjectText;

    private TextMeshProUGUI[] panelsTexts;

    private Animator animator;
    [Header("Animation settings"), SerializeField, Range(0, 10)]
    private int secondsBeforeDissapearPanel = 1;

    private bool haveAnimation;
    protected bool isPanelActive;

    private PlayerController Player;
    private Transform cameraMainTransform;
    private Transform cameraLeftTransform;

    // Unity Events
    protected virtual void Start()
    {
        base.Start();
        gameManager = FindObjectOfType<GameManager>();

        if (PanelObject == null) Debug.LogError($"({this.gameObject.name}) ERROR: Need a panel object !!!!");
        else if (UseTextPanels && ObjectText.Count() == 0)
            Debug.LogWarning($"({this.gameObject.name}) Use text panels is set on true but dont write nothing in string list, using default Canvas objects texts . . .");
        else if (UseTextPanels && ObjectText.Count() > 0)
        {
            //Change Text
            panelsTexts = PanelObject.GetComponentsInChildren<TextMeshProUGUI>();
            InitPanelTexts();
        }

        animator = PanelObject.GetComponent<Animator>();
        previousPanelTransform = PanelObject.transform;

        haveAnimation = (animator != null);
        PlayAnimation(false);
        ChangeStatus(false);

        //Need to refactor this (Same in VideoTape)
        Player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).GetComponent<PlayerController>();
        cameraMainTransform = Player.cameraMainTransform;
        cameraLeftTransform = Player.cameraLeftTransform;
    }

    protected virtual void Update()
    {
        base.Update();

        if (isPanelActive)
        {
#if UNITY_WEBGL
            PanelObject.transform.LookAt(new Vector3(cameraLeftTransform.position.x, PanelObject.transform.position.y, cameraLeftTransform.position.z));
#else
            PanelObject.transform.LookAt(new Vector3(cameraMainTransform.position.x, PanelObject.transform.position.y, cameraMainTransform.position.z));
#endif


        }
    }

    // Special Interactable Functions
    public override void Drop(bool isXR = false)
    {
    }

    public override void Grab(bool isXR = false)
    {
        if (CanInteract)
            ChangeTextPanelStatus();
    }

    public override void Throw(bool isXR = false)
    {
    }

    // Auxiliar Functions
    public void ChangeTextPanelStatus()
    {
        if (isPanelActive)
        {
            gameManager.CloseTextPanel();
            gameManager.StopAudioSource();
            if (!PlayAnimation(false))
                ChangeStatus(false);
        }
        else
        {
            gameManager.OpenTextPanel(this);
            ChangeStatus(true);
            PlayAnimation(true);
            CanInteract = true;
        }
    }

    private void InitPanelTexts()
    {
        var minCount = Mathf.Min(panelsTexts.Count(), ObjectText.Count());

        for (var i = 0; i < minCount; i++)
            if (!string.IsNullOrWhiteSpace(ObjectText[i]))
                panelsTexts[i].text = ObjectText[i];
    }

    private void ChangeStatus(bool isOn)
    {
        isPanelActive = isOn;
        PanelObject.SetActive(isOn);
        if (haveSound) _audioSource.Stop();
    }
    private bool PlayAnimation(bool isOn)
    {
        if (haveAnimation)
        {
            animator.SetBool(Constants.IS_ACTIVATE, isOn);
            if (!isOn)
            {
                CanInteract = false;
                StartCoroutine(DisactiveTextPanel());
            }
            return true;
        }
        else
            return false;
    }

    private IEnumerator DisactiveTextPanel()
    {
        AnimatorClipInfo[] currentAnimation = animator.GetCurrentAnimatorClipInfo(0);
        if (currentAnimation.Length > 0)
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (animationName != "ObjectTextPanel@Idle")
            {
                yield return new WaitForSeconds(secondsBeforeDissapearPanel);
            }
        }
        isPanelActive = false;
        PanelObject.SetActive(isPanelActive);
        CanInteract = true;
        yield break;
    }
}
