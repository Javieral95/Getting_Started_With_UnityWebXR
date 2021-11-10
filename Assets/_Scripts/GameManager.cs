//==================================================================================================================
//
// Brain of App, have constants and general functions.
//
//==================================================================================================================

using System.Collections;
using System.Collections.Generic;
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
    public GameObject Player;
    public GameObject StartPoint;

    public const string INTERACTABLE_TAG = "Interactable";
    public const string INTERACTABLE_TRIGGER_TAG = "InteractableTrigger";
    #endregion

    //If a script will be using the singleton in its awake method, make sure the manager is first to execute with the Script Execution Order project settings
    void Awake()
    {
        EnsureSingleton();
    }

    /// <summary>
    /// This functions is called when the user press the RED BUTTON. Instantiate and throw a new Interactable ball.
    /// </summary>
    public void InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation, float impulseForce)
    {
        GameObject newBall = Instantiate(objectToInstantiate, position, rotation);
        Rigidbody newBallrb = newBall.GetComponent<Rigidbody>();

        if(newBallrb!=null)
            newBallrb.AddForce(Vector3.back * impulseForce, ForceMode.Impulse);
    }
    public void InstantiateNewObject(GameObject objectToInstantiate, Vector3 position, Quaternion rotation)
    {
        Instantiate(objectToInstantiate, position, rotation);
    }

    /// <summary>
    /// This function should teleport the player to start point again.
    /// </summary>
    public void RespawnPlayer()
    {
        //TO-DO: Is not working!
        if (StartPoint != null)
        {
            Player.transform.position = StartPoint.transform.position;
            Player.GetComponent<CharacterController>().transform.position = StartPoint.transform.position;
        }
    }
}
