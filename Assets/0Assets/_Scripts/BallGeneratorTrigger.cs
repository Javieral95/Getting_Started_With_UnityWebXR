using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BallGeneratorTrigger : MonoBehaviour, TriggerInterface
{
    private static GameManager gameManager;

    [Header("Gameobjects")]
    public GameObject BallsSpawn;
    [Header("Trigger function options")]
    public GameObject PrefabToInstantiate;
    [SerializeField, Range(2, 10)]
    private float instantiateBallForce = 5f;

    private Animator anim;
    private string BUTTON_ANIMATION_TRIGGER = "Press_Trig";

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        anim = this.gameObject.GetComponent<Animator>();
    }

    public void Press()
    {
        PressFunction();
        PlayPressAnimation();
        PlayPressSound();
    }

    public void PressFunction()
    {
        gameManager.InstantiateNewObject(PrefabToInstantiate, BallsSpawn.transform.position, BallsSpawn.transform.rotation, instantiateBallForce);
    }

    public void PlayPressAnimation()
    {
        anim.SetTrigger(BUTTON_ANIMATION_TRIGGER);
    }

    public void PlayPressSound()
    {
        //Not sound yet
    }
}
