using System.Collections;
using TMPro;
using UnityEngine;

public class IndustrialControllerBehaviour : MonoBehaviour
{
    public GameObject IndustrialController;
    public TextMeshProUGUI ScreenText;

    private Animator _animator;
    private string ANIMATOR_BOOL = "isActivate";
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateIndustrialController()
    {
        this.gameObject.SetActive(true);
        _animator.SetBool(ANIMATOR_BOOL, true);
    }

    public void DisactivateIndustrialController()
    {
        _animator.SetBool(ANIMATOR_BOOL, false);
        StartCoroutine(DisactivateGameObject(1));
    }

    private IEnumerator DisactivateGameObject(int seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
    }
}
