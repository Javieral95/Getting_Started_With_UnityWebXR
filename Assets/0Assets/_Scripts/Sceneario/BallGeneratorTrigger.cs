using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallGeneratorTrigger : MonoBehaviour
{
    private static GameManager gameManager;

    [Header("Gameobjects")]
    public GameObject BallsSpawn;
    [Header("Trigger function options")]
    public GameObject PrefabToInstantiate;
    [SerializeField, Range(2, 10)]
    private float instantiateBallForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

    }

    public void InstanceBall()
    {
        gameManager.InstantiateNewObject(PrefabToInstantiate, BallsSpawn.transform, instantiateBallForce);
    }
}
