//==================================================================================================================
//
// Brain of App, have constants and general functions.
//
//==================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton

    //Use a property so other code can't assign instance.
    static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (!_instance)
                //Writing it this way names the new gameObject and adds the gameManager component to it, finally getting the added component and assigning it to
                //_instance.
                _instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();

            return _instance;
        }
    }
    //This ensures that no other GameObjects have a GameManager either attached from the editor or instantiated by
    //other code. Since it's always called on awake, no two GameManagers will ever be active at runtime.
    void EnsureSingleton()
    {
        //In case the instance was placed using the editor, rather than created by accessing the static member.
        if (!_instance)
            _instance = this;

        var gameManagerInstances = GameObject.FindObjectsOfType<GameManager>();

        if (gameManagerInstances.Length != 1)
        {
            Debug.LogError("Only one instance of the GameManager manager should ever exist. Removing extraneous.");

            foreach (var otherInstance in gameManagerInstances)
            {
                if (otherInstance != instance)
                {

                    //In case some moron added two GameManager instances to the same GameObject, somehow.
                    if (otherInstance.gameObject == instance.gameObject)
                        Destroy(otherInstance);
                    else
                        Destroy(otherInstance.gameObject);

                }
            }
        }

    }

    #endregion

    #region Properties
    [Header("Gameobjects")]
    private PlayerController Player;

    private CharacterController PlayerCharacterController;
    private Transform cameraMainTransform;
    private Transform cameraLeftTransform;

    public GameObject[] StartPointsList;
    private Transform StartPoint;

    public bool IsGameStopped { get; private set; }

    public const string INTERACTABLE_TAG = "Interactable";
    public const string INTERACTABLE_NOT_MOVABLE_TAG = "InteractableNotMovable";
    #endregion

    private int StartPointIndex = 0;
    private InteractableTextPanel openedTextPanel;
    private AudioSource playingAudioSource;

    //If a script will be using the singleton in its awake method, make sure the manager is first to execute with the Script Execution Order project settings
    void Awake()
    {
        EnsureSingleton();
    }

    private void Start()
    {
        StartPointIndex = SceneArguments.StartPointIndex;

        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cameraMainTransform = Player.cameraMainTransform;
        cameraLeftTransform = Player.cameraLeftTransform;

        StartPoint = StartPointsList[StartPointIndex].transform;

        PlayerCharacterController = Player.GetComponent<CharacterController>();
        openedTextPanel = null;

#if UNITY_WEBGL
        AudioListener.volume = 1;
#endif
        MovePlayerToStartPoint();
    }
    public void FixedUpdate()
    {
        //If you want to teleport the Player (with CharacterController) you need to change his transform inside
        //FixedUpdate or disable the character controller, change the transform and enable it again.
        //if (IsFirstIteration)
        //    MovePlayerToStartPoint();

    }

    public void StopApp()
    {
        IsGameStopped = true;
        Time.timeScale = 0;
    }

    public void ResumeApp()
    {
        IsGameStopped = false;
        Time.timeScale = 1;
    }

    #region Instantiate Functions
    /// <summary>
    /// This functions is called to instantiate and throw a new object instance.
    /// </summary>
    public GameObject InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation, float impulseForce)
    {
        GameObject newObject = Instantiate(objectToInstantiate, position, rotation);
        Rigidbody newObject_rb = newObject.GetComponent<Rigidbody>();

        if (newObject_rb != null)        
            newObject_rb.AddForce(rotation.eulerAngles * impulseForce, ForceMode.Impulse);

        return newObject;
    }
    public GameObject InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation)
    {
        return Instantiate(objectToInstantiate, position, rotation);
    }
    public GameObject InstantiateNewObject(GameObject objectToInstantiate, Transform transform)
    {
        return Instantiate(objectToInstantiate, transform.position, transform.rotation);
    }
    public GameObject InstantiateNewObject(GameObject objectToInstantiate, Transform transform, float impulseForce)
    {
        GameObject newObject = Instantiate(objectToInstantiate, transform.position, transform.rotation);
        Rigidbody newObject_rb = newObject.GetComponent<Rigidbody>();

        if (newObject_rb != null)
            newObject_rb.AddForce(transform.forward * impulseForce, ForceMode.Impulse);

        return newObject;
    }
    #endregion

    private void MovePlayerToStartPoint(int startPointIndex = 0)
    {
        ChangeCharacterControllerStatus(false);

        //Player.enabled = false;
        Player.gameObject.transform.position = StartPoint.position;

        Player.gameObject.transform.rotation = StartPoint.rotation;
        Player.cameraMainTransform.rotation = StartPoint.rotation;
        if(Player.IsXREnabled)
            Player.cameraLeftTransform.rotation = StartPoint.rotation;

        Player.SetOriginalRotation(StartPoint);
        //Player.enabled = true;
        ChangeCharacterControllerStatus(true);
    }

    private void ChangeCharacterControllerStatus(bool isEnabled)
    {
        //Need to disable and enable CharacterController or the teleport wont work
        PlayerCharacterController.enabled = isEnabled;
    }

    /// <summary>
    /// This function should teleport the player to start point again (Reset the scene).
    /// </summary>
    public void RespawnPlayer()
    {
        MovePlayerToStartPoint();
    }

    /// <summary>
    /// This function load a new scene using its name
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// This function load a new scene using its build index
    /// </summary>
    /// <param name="sceneBuildIndex"></param>
    public void LoadScene(Scene sceneBuildIndex, int startPointIndex = 0)
    {
        SceneArguments.StartPointIndex = startPointIndex;
        SceneManager.LoadScene(sceneBuildIndex.handle, LoadSceneMode.Single);
    }

    /// <summary>
    /// Ensure to have only one Interactable Text Panel open
    /// </summary>
    /// <param name="panel"></param>
    public void OpenTextPanel(InteractableTextPanel panel)
    {
        if (openedTextPanel != null) openedTextPanel.ChangeTextPanelStatus();
        openedTextPanel = panel;
    }
    public void CloseTextPanel()
    {
        openedTextPanel = null;
    }

    /// <summary>
    /// Ensure to have only one AudioSource playing
    /// </summary>
    /// <param name="audioSource"></param>
    public void PlayAudioSource(AudioSource audioSource)
    {
        if (playingAudioSource != null) playingAudioSource.Stop();
        playingAudioSource = audioSource;
        playingAudioSource.Play();
    }
    public void StopAudioSource()
    {
        playingAudioSource?.Stop();
        playingAudioSource = null;
    }
}


/// <summary>
/// Arguments o pass as parameter when the user change the scene
/// </summary>
public static class SceneArguments
{
    public static int StartPointIndex { get; set; }
}