/***********************************************************************
 ******* MENU (For VR with Canvas and NonVR with Physic buttons) *******
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    [Header("Game Objects")]
    [Tooltip("The Canvas with the settings menu")]
    public Canvas MenuCanvas;
    [Tooltip("The object which contains the VR Menu")]
    public GameObject VRMenuObject;
    [Tooltip("The settings menu will be opened pressing the Y (B) button of the controller in XR mode")]
    public WebXRInputManager HandController;

    [Header("Settings")]
    [Tooltip("The scene where the user will teleport after press Start Menu")]
    public Scene FirstScene;

    [SerializeField, Range(0.1f, 5f), Tooltip("Distance between VR Menu and hand")]
    private readonly float VRMenuDistance = 0.3f;

    //Settings controls
    [Header("VR Menu Controls")]
    public VRPhysicalToggle allowStickRotationVRToggle;
    public VRPhysicalToggle useTickRotationVRToggle;
    public VRPhysicalToggle allowStickMoveVRToggle;
    public VRPhysicalToggle allowTeleportVRToggle;

    [Header("NonVR Menu Controls")]
    public Slider sensibilitySlider;
    public TextMeshProUGUI sliderTextValue;
    public Toggle allowStickRotationToggle;
    public Toggle useTickRotationToggle;
    public Toggle allowStickMoveToggle;
    public Toggle allowTeleportToggle;
    public Button startButton;

    // Properties
    private GameManager gameManager;
    private PlayerController Player;
    private bool oldXRStatus;
    private Transform cameraMainTransform;
    private Transform cameraLeftTransform;

    [Header("Other Settings")]
    public string ResumeButtonText = "Resume";

    public bool IsAppStarted { get; private set; }
    public bool IsMenuOpened { get; private set; }


    #region Unity events
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StopApp();
        Player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).GetComponent<PlayerController>();
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
        Player.canRotateWithSticks = isChecked;
        useTickRotationToggle.enabled = isChecked; //NonXR
        useTickRotationVRToggle.enabled = isChecked; //XR
    }
    public void ChangeUseTickRotation(bool isChecked)
    {
        Player.useTickRotation = isChecked;
    }
    public void ChangeStickMovement(bool isChecked)
    {
        Player.canMoveWithSticks = isChecked;
    }
    public void ChangeAllowTeleport(bool isChecked)
    {
        Player.isTeleportEnabled = isChecked;
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
        startButton.GetComponentInChildren<Text>().text = ResumeButtonText;

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
        var closeMenuButtonPressed = Is_Close_VR_Menu_Button_Pressed();

        if (openMenuButtonPressed)
        {
            VRMenuObject.gameObject.SetActive(true);
            ChangeVRControlsStatus(true);
            IsMenuOpened = true;
        }
        else if (closeMenuButtonPressed)
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
#if UNITY_WEBGL
        VRMenuObject.transform.LookAt(cameraLeftTransform);
#else
        VRMenuObject.transform.LookAt(cameraMainTransform);
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
        //Need to do this because the parent is disabled but player hands will allow to change toggles values.
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