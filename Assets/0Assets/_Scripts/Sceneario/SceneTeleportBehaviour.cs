using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneTeleportBehaviour : MonoBehaviour
{
    private static GameManager gameManager;
    [Header("Options")]
    [Tooltip("Index of the new scene")]
    public Scene SceneToTeleport;
    [Tooltip("Index of the starpoint when the user will appear in the new Scene (Startpoints are saved into scene's GameManager)")]
    public int StartPointIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            gameManager.LoadScene(SceneToTeleport, StartPointIndex);
    }
}
