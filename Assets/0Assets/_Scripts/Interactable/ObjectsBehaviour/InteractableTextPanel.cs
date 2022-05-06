/***********************************************************************
 ********** Use it to appear a custom text above the object ************
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableTextPanel : SpecialInteractable
{
    [Header("Text panel settings: ")]
    public Canvas PanelCanvas;
    private GameObject canvasObject; 
    private TextMeshProUGUI panelText;

    [TextArea, Tooltip("This text will show inside the panel... If it is not null, will override Text Mesh Pro value!!")]
    public string ObjectText;
    [Tooltip("Animator")]
    public Animator Animator;
    [SerializeField, Range(0, 10)]
    private int secondsBeforeDissapearPanel = 1;

    private string ANIMATOR_BOOL = "isActivate";
    private bool haveAnimation;
    private bool panelIsActive;

    // Unity Events
    new void Start()
    {
        base.Start();

        if (Animator == null) haveAnimation = false;

        //Change Text and disable canvas
        canvasObject = PanelCanvas.gameObject;
        panelText = canvasObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!string.IsNullOrWhiteSpace(ObjectText))
            panelText.text = ObjectText;
    }

    new void Update()
    {
        base.Update();
    }

    // Special Interactable Functions
    public override void Drop(bool isXR = false)
    {
    }

    public override void Grab(bool isXR = false)
    {
        ChangeTextPanelStatus();
    }

    public override void Throw(bool isXR = false)
    {
    }

    // Auxiliar Functions
    private void ChangeTextPanelStatus()
    {
        if (panelIsActive)
        {
            Animator.SetBool(ANIMATOR_BOOL, false);
            StartCoroutine(DisactiveTextPanel());
        }
        else
        {
            Animator.SetBool(ANIMATOR_BOOL, true);
            canvasObject.SetActive(true);
            panelIsActive = true;
        }        
    }

    private IEnumerator DisactiveTextPanel()
    {
        string animationName = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (animationName == "ObjectTextPanel@Disappear")
        {
            yield return new WaitForSeconds(secondsBeforeDissapearPanel);
        }
        panelIsActive = false;
        canvasObject.SetActive(panelIsActive);
        yield break;
    }
}
