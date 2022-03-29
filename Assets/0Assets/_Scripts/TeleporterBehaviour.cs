using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TeleporterBehaviour : MonoBehaviour
{
    public GameManager GameManager;
    public Scene SceneToTeleport;

    public int StartPointIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            GameManager.LoadScene(SceneToTeleport, StartPointIndex);
    }
}
