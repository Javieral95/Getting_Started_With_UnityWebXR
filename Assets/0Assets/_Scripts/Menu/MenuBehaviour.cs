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
    public Canvas MenuCanvas;
    private RectTransform menuCanvasTransform;
    private Vector3 originalMenuCanvasScale;
    private Vector3 originalMenuCanvasPosition;
    [SerializeField, Range(0.1f, 5f)]
    private float menuDistanceInVR;

    [Tooltip("The settings menu will be opened pressing the Y (B) button of the controller")]
    public WebXRInputManager HandController;
    [Tooltip("The scene where the user will teleport after press Start Menu")]
    public Scene FirstScene;

    private GameManager gameManager;
    private PlayerController Player;
    private bool isAppStarted;
    private bool isMenuOpened;
    private bool isButtonPressed;

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

    private Transform CanvasVRPosition;
    private Camera mainCamera;
    private Camera leftCamera;

    #region Unity events
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StopApp();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        AllowMouse(true);
        InitSettingsControls();

        menuCanvasTransform = MenuCanvas.gameObject.GetComponent<RectTransform>();
        originalMenuCanvasScale = menuCanvasTransform.localScale;
        originalMenuCanvasPosition = menuCanvasTransform.position;

        //VR
        m_EventSystem = MenuCanvas.GetComponent<EventSystem>();
        m_Raycaster = MenuCanvas.GetComponent<GraphicRaycaster>();
        m_PointerEventData = new PointerEventData(m_EventSystem);

        //End
        isMenuOpened = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (PressSettingsMenuButton() && isAppStarted)
        {
            ChangeSettingsMenuStatus();
            if (Player.IsXREnabled)
                MoveCanvasToHand();
        }

        if (isMenuOpened)
        {
            CheckCanvasRenderMode();
            XRMenuInteraction();
        }
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

        MenuCanvas.gameObject.SetActive(false);
        AllowMouse(false);
        isAppStarted = true;
        isMenuOpened = false;
    }
    #endregion

    #region Settings Menu
    private bool PressSettingsMenuButton()
    {
        isButtonPressed = true;

        if (Player.IsXREnabled)
            return HandController.Is_B_ButtonPressed();
        else
            return Input.GetButtonDown("Fire3");

    }

    private void MoveCanvasToHand()
    {
        menuCanvasTransform.position = HandController.transform.position;
        menuCanvasTransform.transform.localPosition = menuCanvasTransform.transform.localPosition + new Vector3(0, 0, menuDistanceInVR);
        menuCanvasTransform.LookAt(Camera.main.transform);
        menuCanvasTransform.Rotate(menuCanvasTransform.up, 180);
    }

    private void ChangeSettingsMenuStatus()
    {
        //CheckCanvasRenderMode();
        var oldValue = MenuCanvas.gameObject.activeSelf;
        MenuCanvas.gameObject.SetActive(!oldValue);
        AllowMouse(!oldValue);
        if (oldValue)
        {
            gameManager.ResumeApp();
            isMenuOpened = false;
        }
        else
        {
            MoveCanvasToHand();
            gameManager.StopApp();
            isMenuOpened = true;
        }
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

    private void CheckCanvasRenderMode()
    {
        if (Player.IsXREnabled && MenuCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            /*
            MenuCanvas.worldCamera = mainCamera;
#if UNITY_WEBGL
            MenuCanvas.worldCamera = leftCamera;
#endif
*/
            MenuCanvas.worldCamera = Camera.main;
            MenuCanvas.renderMode = RenderMode.WorldSpace;
            menuCanvasTransform.localScale = originalMenuCanvasScale;
            menuCanvasTransform.position = originalMenuCanvasPosition;
        }
        else if (!Player.IsXREnabled && MenuCanvas.renderMode == RenderMode.WorldSpace)
            MenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void AllowMouse(bool allow = true)
    {
        Cursor.visible = allow;
        Cursor.lockState = (allow) ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
}
