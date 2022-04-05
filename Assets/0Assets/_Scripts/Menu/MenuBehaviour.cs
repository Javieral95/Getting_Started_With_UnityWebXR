/***********************************************************************
 ******* MENU (For VR with Canvas and NonVR with Physic buttons) *******
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    [Header("Game Objects")]
    [Tooltip("The Canvas with the settings menu")]
    public Canvas MenuCanvas;

    [Header("VR Menu interaction")]
    [SerializeField, Range(0.1f, 5f)]
    private readonly float VRMenuDistance = 0.3f;

    [Tooltip("The settings menu will be opened pressing the Y (B) button of the controller")]
    public WebXRInputManager HandController;

    [Tooltip("The scene where the user will teleport after press Start Menu")]
    public Scene FirstScene;

    private GameManager gameManager;
    private PlayerController Player;
    private bool oldXRStatus;
    private Transform cameraMainTransform;
    private Transform cameraLeftTransform;

    public bool IsAppStarted { get; private set; }
    public bool IsMenuOpened { get; private set; }

    //Settings controls
    [Header("VR Menu Controls")]
    public GameObject VRMenuObject;
    public VRToggle allowStickRotationVRToggle;
    public VRToggle useTickRotationVRToggle;
    public VRToggle allowStickMoveVRToggle;
    public VRToggle allowTeleportVRToggle;

    [Header("NonVR Menu Controls")]
    public Slider sensibilitySlider;
    public TextMeshProUGUI sliderTextValue;
    public Toggle allowStickRotationToggle;
    public Toggle useTickRotationToggle;
    public Toggle allowStickMoveToggle;
    public Toggle allowTeleportToggle;
    public Button startButton;

    #region Unity events
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StopApp();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cameraMainTransform = Player.cameraMainTransform;
        cameraLeftTransform = Player.cameraLeftTransform;

        Thread.Sleep(100);
        UpdateControlsValues();
        VRMenuObject.SetActive(false);
        //End
        IsMenuOpened = true;
    }

    // Update is called once per frame
    void Update()
    {
        //TO-DO: Refactor this
        CheckXRStatus();

        //Update loop
        if (Player.IsXREnabled)
        {
            VRMenuBehaviour();

            //Start App in VR
            if (!IsAppStarted && Is_Start_VR_Button_Pressed())
                StartApp();
        }
        else if (PressSettingsMenuButton() && IsAppStarted)
            ChangeNonVRSettingsMenuStatus();
    }
    #endregion

    #region Settings events
    public void ChangeMouseSensibility(float value)
    {
        Player.mouseSensitivity = value;
        sliderTextValue.text = $"{Player.mouseSensitivity}";
    }
    public void ChangeRotateSticks(bool isChecked)
    {
        //NonVR
        Player.canRotateWithSticks = isChecked;
        useTickRotationToggle.enabled = isChecked;

        //VR
        //allowStickRotationVRToggle.ChangeStatus(isChecked);
        useTickRotationVRToggle.enabled = isChecked;
    }
    public void ChangeUseTickRotation(bool isChecked)
    {
        //NonVR
        Player.useTickRotation = isChecked;
        //VR
        //if (Player.IsXREnabled)
        //    useTickRotationVRToggle.ChangeStatus(isChecked);
        //else
        //    useTickRotationToggle.isOn = isChecked;
    }
    public void ChangeStickMovement(bool isChecked)
    {
        //NonVR
        Player.canMoveWithSticks = isChecked;
        //VR
        //allowStickMoveVRToggle.ChangeStatus(isChecked);
    }
    public void ChangeAllowTeleport(bool isChecked)
    {
        //NonVR
        Player.isTeleportEnabled = isChecked;
        //VR
        //allowTeleportVRToggle.ChangeStatus(isChecked);
    }
    #endregion

    #region Main Menu
    /// <summary>
    /// This function will teleport to main scene after click Start button on main menu (or close the menu when the user is playing).
    /// </summary>
    public void StartGame()
    {
        if (IsAppStarted)
            ChangeNonVRSettingsMenuStatus();
        else
            StartApp();
    }

    /// <summary>
    /// This function will close the game.
    /// </summary>
    public void StopGame()
    {
        Application.Quit();
    }

    private void StartApp()
    {
        gameManager.ResumeApp();
        gameManager.LoadScene(FirstScene, 0);
        startButton.GetComponentInChildren<Text>().text = "Resume";

        MenuCanvas.gameObject.SetActive(false);
        AllowMouse(false);
        IsAppStarted = true;
        IsMenuOpened = false;
    }
    #endregion

    #region Settings Menu
    private bool PressSettingsMenuButton()
    {
        if (Player.IsXREnabled)
            return HandController.Is_B_ButtonPressed();
        else
            return Input.GetButtonDown("Fire3");

    }

    private void ChangeNonVRSettingsMenuStatus()
    {
        //CheckCanvasRenderMode();
        var oldValue = MenuCanvas.gameObject.activeSelf;
        MenuCanvas.gameObject.SetActive(!oldValue);
        AllowMouse(!oldValue);
        if (oldValue)
        {
            gameManager.ResumeApp();
            IsMenuOpened = false;
        }
        else
        {
            gameManager.StopApp();
            IsMenuOpened = true;
        }
    }
    #endregion

    #region VR Interaction
    private void VRMenuBehaviour()
    {
        var openMenuButtonPressed = PressSettingsMenuButton();
        var closeMenu = Is_Close_VR_Menu_Button_Pressed();

        if (openMenuButtonPressed)
        {
            VRMenuObject.gameObject.SetActive(true);
            ChangeVRControlsStatus(true);
            IsMenuOpened = true;
        }
        else if (closeMenu)
        {
            VRMenuObject.gameObject.SetActive(false);
            ChangeVRControlsStatus(false);
            IsMenuOpened = false;
        }

        if (IsMenuOpened)
            MoveVRMenuToHand();
    }

    private void MoveVRMenuToHand()
    {
        //VRMenuDistance
        VRMenuObject.transform.position = (HandController.transform.position) + (VRMenuDistance * HandController.transform.right); ;
        VRMenuObject.transform.LookAt(cameraMainTransform);
#if UNITY_WEBGL
        VRMenuObject.transform.LookAt(cameraLeftTransform);
#endif

    }

    private bool Is_Start_VR_Button_Pressed()
    {
        return HandController.Is_A_ButtonPressed();
    }
    private bool Is_Close_VR_Menu_Button_Pressed()
    {
        return HandController.Is_B_ButtonUp();
    }
    #endregion

    #region Auxiliar Functions
    private void UpdateControlsValues()
    {
        //startButton.onClick.AddListener(delegate { StartGame(); });

        sliderTextValue.text = $"{Player.mouseSensitivity}";
        sensibilitySlider.value = Player.mouseSensitivity;
        allowStickRotationToggle.isOn = Player.canRotateWithSticks;
        useTickRotationToggle.isOn = Player.useTickRotation;
        useTickRotationToggle.enabled = Player.canRotateWithSticks;
        allowStickMoveToggle.isOn = Player.canMoveWithSticks;
        allowTeleportToggle.isOn = Player.isTeleportEnabled;

        //Change VR VALUES
        allowStickRotationVRToggle.ChangeStatus(Player.canRotateWithSticks);
        useTickRotationVRToggle.ChangeStatus(Player.useTickRotation);
        useTickRotationVRToggle.enabled = Player.canRotateWithSticks;
        allowStickMoveVRToggle.ChangeStatus(Player.canMoveWithSticks);
        allowTeleportVRToggle.ChangeStatus(Player.isTeleportEnabled);
    }

    private void ChangeVRControlsStatus(bool status)
    {
        //Need to do this, because the parent is disabled but player hands will allow to change toggles values.
        allowStickRotationVRToggle.enabled = status;
        useTickRotationVRToggle.enabled = status;
        allowStickMoveVRToggle.enabled = status;
        allowTeleportVRToggle.enabled = status;

    }

    private void AllowMouse(bool allow = true)
    {
        Cursor.visible = allow;
        Cursor.lockState = (allow) ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void CheckXRStatus()
    {
        if (oldXRStatus != Player.IsXREnabled)
        {
            MenuCanvas.gameObject.SetActive(!Player.IsXREnabled);
            AllowMouse(!Player.IsXREnabled);
            IsMenuOpened = !Player.IsXREnabled;

            if (Player.IsXREnabled)
                gameManager.ResumeApp();
            else
                gameManager.StopApp();

            oldXRStatus = Player.IsXREnabled;
            UpdateControlsValues();
        }
    }
    #endregion
}
