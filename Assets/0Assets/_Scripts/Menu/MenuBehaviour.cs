using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    [Header("Game Objects")]
    [Tooltip("The Canvas with the settings menu")]
    public GameObject MenuCanvas;
    [Tooltip("The settings menu will be opened pressing the Y (B) button of the controller")]
    public WebXRInputManager HandController;
    [Tooltip("The scene where the user will teleport after press Start Menu")]
    public Scene FirstScene;

    private GameManager gameManager;
    private PlayerController Player;
    private bool isAppStarted;

    //Settings controls
    [Header("Menu Controls")]
    public Slider sensibilitySlider;
    public TextMeshProUGUI sliderTextValue;
    public Toggle allowStickRotationToggle;
    public Toggle useTickRotationToggle;
    public Toggle allowStickMoveToggle;
    public Toggle allowTeleportToggle;
    public Button startButton;

    //VR Interaction Raycast objects
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    #region Unity events
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StopApp();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        AllowMouse(true);
        InitSettingsControls();
        
        m_EventSystem = MenuCanvas.GetComponent<EventSystem>();
        m_Raycaster = MenuCanvas.GetComponent<GraphicRaycaster>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAppStarted && PressSettingsMenuButton())
            ChangeSettingsMenuStatus();

        XRMenuInteraction();
    }
    #endregion

    #region Settings events
    public void ChangeMouseSensibility()
    {
        Player.mouseSensitivity = sensibilitySlider.value;
        sliderTextValue.text = $"{Player.mouseSensitivity}";
    }
    public void ChangeRotateSticks()
    {
        var active = allowStickRotationToggle.isOn;
        Player.canRotateWithSticks = active;        
        useTickRotationToggle.enabled = active;
    }
    public void ChangeUseTickRotation()
    {
        Player.useTickRotation = useTickRotationToggle.isOn;
    }
    public void ChangeStickMovement()
    {
        Player.canMoveWithSticks = allowStickMoveToggle.isOn;
    }
    public void ChangeAllowTeleport()
    {
        Player.isTeleportEnabled = allowTeleportToggle.isOn;
    }
    #endregion

    #region Main Menu
    /// <summary>
    /// This game will teleport to main scene after click Start button on main menu.
    /// </summary>
    public void StartGame()
    {
        if (isAppStarted)
            ChangeSettingsMenuStatus();
        else
            StartApp();
    }

    private void StartApp()
    {
        gameManager.ResumeApp();
        gameManager.LoadScene(FirstScene, 0);
        startButton.GetComponentInChildren<Text>().text = "Resume";

        MenuCanvas.SetActive(false);
        AllowMouse(false);
        isAppStarted = true;
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
    private void ChangeSettingsMenuStatus()
    {
        MenuCanvas.SetActive(!MenuCanvas.activeSelf);
        AllowMouse(MenuCanvas.activeSelf);
        if (MenuCanvas.activeSelf)
            gameManager.StopApp();
        else
            gameManager.ResumeApp();

        Debug.Log("Cursor set visible to: " + MenuCanvas.activeSelf);

    }
    #endregion

    #region VR Interaction
    private void XRMenuInteraction()
    {
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = new Vector3(0.5f, 0.5f, 0); //Screen Center
        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);
        }
    }
    #endregion

    #region Auxiliar Functions
    private void InitSettingsControls()
    {
        sensibilitySlider.onValueChanged.AddListener(delegate { ChangeMouseSensibility(); });
        allowStickRotationToggle.onValueChanged.AddListener(delegate { ChangeRotateSticks(); });
        useTickRotationToggle.onValueChanged.AddListener(delegate { ChangeUseTickRotation(); });
        allowStickMoveToggle.onValueChanged.AddListener(delegate { ChangeStickMovement(); });
        allowTeleportToggle.onValueChanged.AddListener(delegate { ChangeAllowTeleport(); });
        startButton.onClick.AddListener(delegate { StartGame(); });

        sliderTextValue.text = $"{Player.mouseSensitivity}";
        sensibilitySlider.value = Player.mouseSensitivity;
        allowStickRotationToggle.isOn = Player.canRotateWithSticks;
        useTickRotationToggle.isOn = Player.useTickRotation;
        useTickRotationToggle.enabled = Player.canRotateWithSticks;
        allowStickMoveToggle.isOn = Player.canMoveWithSticks;        
        allowTeleportToggle.isOn = Player.isTeleportEnabled;
    }

    private void AllowMouse(bool allow = true)
    {
        Cursor.visible = allow;
        Cursor.lockState = (allow) ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
}
