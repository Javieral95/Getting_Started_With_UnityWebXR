using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillzoneBehaviour : MonoBehaviour
{
    private static GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
            Destroy(other.gameObject);
        else if (other.CompareTag("Player"))
            gameManager.RespawnPlayer();
    }
}
