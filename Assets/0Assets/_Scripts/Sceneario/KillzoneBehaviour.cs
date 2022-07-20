using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillzoneBehaviour : MonoBehaviour
{
    private static GameManager gameManager;
    private ISpecialInteractable tempSpecialInteractable;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.INTERACTABLE_TAG) && !ResetSpecialInteractable(other))
            Destroy(other.gameObject);
        else if (other.CompareTag(Constants.PLAYER_TAG))
            gameManager.RespawnPlayer();
    }

    private bool ResetSpecialInteractable(Collider other)
    {
        tempSpecialInteractable = other.GetComponent<SpecialInteractable>();
        if (tempSpecialInteractable != null)
        {
            tempSpecialInteractable.ResetPosition();
            return true;
        }
        return false;
    }
}
