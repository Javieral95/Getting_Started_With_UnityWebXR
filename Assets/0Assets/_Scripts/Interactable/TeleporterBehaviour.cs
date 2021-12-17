using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterBehaviour : MonoBehaviour
{
    public GameManager GameManager;
    public Scene SceneToTeleport;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            GameManager.LoadScene(SceneToTeleport);
    }
}
