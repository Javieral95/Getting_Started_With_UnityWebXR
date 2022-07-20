using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectGenerator : MonoBehaviour
{
    private static GameManager GameManager;

    [Header("Generator Settings")]
    public Transform SpawnPosition;
    public GameObject PrefabToInstantiate;
    [SerializeField, Range(1, 30)]
    private float instantiateForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager == null)
            GameManager = FindObjectOfType<GameManager>();
    }

    public void GenerateObject(bool applyForce = true)
    {
        if (applyForce)
            GameManager.InstantiateNewObject(PrefabToInstantiate, SpawnPosition.transform, instantiateForce);
        else
            GameManager.InstantiateNewObject(PrefabToInstantiate, SpawnPosition.transform);
    }
}
