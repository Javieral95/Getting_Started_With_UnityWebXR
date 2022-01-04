//==================================================================================================================
//
// Brain of App, have constants and general functions.
//
//==================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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
    private GameObject Player;

    private CharacterController PlayerCharacterController;
    
    public GameObject[] StartPointsList;
    private Transform StartPoint;

    public const string INTERACTABLE_TAG = "Interactable";
    public const string INTERACTABLE_TRIGGER_TAG = "InteractableTrigger";
    public const string INTERACTABLE_NOT_MOVABLE_TAG = "InteractableNotMovable";
    #endregion

    private bool IsFirstIteration = true;
    private int StartPointIndex = 0;

    //If a script will be using the singleton in its awake method, make sure the manager is first to execute with the Script Execution Order project settings
    void Awake()
    {
        EnsureSingleton();
    }

    private void Start()
    {
        StartPointIndex = SceneArguments.StartPointIndex;
        IsFirstIteration = true;
        
        Player = GameObject.FindGameObjectWithTag("Player");
        
        StartPoint = StartPointsList[StartPointIndex].transform;
        
        PlayerCharacterController = Player.GetComponent<CharacterController>();
        MovePlayerToStartPoint();
    }
    public void FixedUpdate()
    {
        //If you want to teleport the Player (with CharacterController) you need to change his transform inside
        //FixedUpdate or disable the character controller, change the transform and enable it again.
        //if (IsFirstIteration)
        //    MovePlayerToStartPoint();

    }

    /// <summary>
    /// This functions is called when the user press the RED BUTTON. Instantiate and throw a new Interactable ball.
    /// </summary>
    public void InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation, float impulseForce)
    {
        GameObject newBall = Instantiate(objectToInstantiate, position, rotation);
        Rigidbody newBallrb = newBall.GetComponent<Rigidbody>();

        if (newBallrb != null)
            newBallrb.AddForce(Vector3.back * impulseForce, ForceMode.Impulse);
    }
    public void InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation)
    {
        Instantiate(objectToInstantiate, position, rotation);
    }

    private void MovePlayerToStartPoint(int startPointIndex=0)
    {
        ChangeCharacterControllerStatus(false);
        Player.transform.position = StartPoint.position;
        Player.transform.rotation = StartPoint.rotation;
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
    public void LoadScene(Scene sceneBuildIndex, int startPointIndex=0)
    {
        SceneArguments.StartPointIndex = startPointIndex;
        SceneManager.LoadScene(sceneBuildIndex.handle, LoadSceneMode.Single);
    }
}

public static class SceneArguments {
    public static int StartPointIndex { get; set; }
}