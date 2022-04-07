using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BallGeneratorTrigger : MonoBehaviour
{
    private static GameManager gameManager;

    [Header("Gameobjects")]
    public GameObject BallsSpawn;
    [Header("Trigger function options")]
    public GameObject PrefabToInstantiate;
    [SerializeField, Range(2, 10)]
    private float instantiateBallForce = 5f;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        anim = this.gameObject.GetComponent<Animator>();

    }

    public void PressButton()
    {
        Debug.Log("ON CLICK EVENT!!!!!");
        PressFunction();
    }

    public void PressFunction()
    {
        gameManager.InstantiateNewObject(PrefabToInstantiate, BallsSpawn.transform.position, BallsSpawn.transform.rotation, instantiateBallForce);
    }
}
