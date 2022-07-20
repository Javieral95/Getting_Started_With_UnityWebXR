using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InfoScreenBehaviour : MonoBehaviour
{
    [Header("Main Object settings")]
    public Animator ScreenAnimator;
    [Header("ScreenObjectSettings")]
    public TextMeshProUGUI PageCounterText;
    public GameObject ScreenObjects;
    private Animator[] ScreenObjectsAnimators;

    private Animator currentScreenObject;
    private Animator nextScreenObject;
    private int pageIndex;
    private int pageCount;

    private bool canPress;
    // Start is called before the first frame update
    void Start()
    {
        InitScreenObjects();
        canPress = true;
        UpdatePageCount(0);
    }

    // Aux Functions
    public void TurnPage()
    {
        if (canPress && ScreenAnimator.GetBool(Constants.IS_ACTIVATE) && UpdatePageCount(+1))
        {
            ChangePage();
            currentScreenObject.SetTrigger(Constants.IS_LEFT_ANIMATION);
            nextScreenObject.SetTrigger(Constants.IS_RIGHT_ANIMATION);
        }
    }

    public void BackPage()
    {
        if (canPress && ScreenAnimator.GetBool(Constants.IS_ACTIVATE) && UpdatePageCount(-1))
        {
            ChangePage();
            currentScreenObject.SetTrigger(Constants.IS_RIGHT_ANIMATION);
            nextScreenObject.SetTrigger(Constants.IS_LEFT_ANIMATION);
        }
    }

    private void ChangePage()
    {
        nextScreenObject.gameObject.SetActive(true);
        StartCoroutine(DisactivateScreenObject());

        currentScreenObject.SetBool(Constants.IS_ACTIVATE, false);
        nextScreenObject.SetBool(Constants.IS_ACTIVATE, true);
    }

    private void InitScreenObjects()
    {
        ScreenObjectsAnimators = ScreenObjects.GetComponentsInChildren<Animator>();
        ScreenObjectsAnimators.Select(x => x.gameObject).ToList().ForEach(x => x.SetActive(false));
        ScreenObjectsAnimators[0].gameObject.SetActive(true);

        pageIndex = 0;
        pageCount = ScreenObjectsAnimators.Count();
    }
    private bool UpdatePageCount(int value)
    {
        currentScreenObject = ScreenObjectsAnimators[pageIndex];

        var previousValue = pageIndex;
        pageIndex = Mathf.Clamp(pageIndex + value, 0, pageCount - 1);
        PageCounterText.text = $"{pageIndex + 1}/{pageCount}";

        nextScreenObject = ScreenObjectsAnimators[pageIndex];

        return pageIndex != previousValue;
    }

    private IEnumerator DisactivateScreenObject()
    {
        canPress = false;
        yield return new WaitForSeconds(1); //1 Frame
        currentScreenObject.gameObject.SetActive(false);
        canPress = true;
    }
}
