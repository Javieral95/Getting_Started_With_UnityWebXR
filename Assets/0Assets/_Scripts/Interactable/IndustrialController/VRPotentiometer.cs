using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPotentiometer : MonoBehaviour
{
    public float sensitivity;

    private Vector2 limits;
    private bool clicking;
    private bool changed;
    private float initRot;

    private float val = .0f;

    private void Start()
    {
        limits = new Vector2(0, 350);
    }

    private void Update()
    {
        if (clicking)
        {
            val += sensitivity * 1000 * Input.GetAxis("Mouse X") * Time.deltaTime;
            val = Mathf.Clamp(val, limits.x, limits.y);
            transform.localEulerAngles = new Vector3(val, -90, -90);
            if (val != initRot) { changed = true; }
            if (changed && (val == limits.x || val == limits.y)) { clicking = false; }
        }
    }

    private void OnMouseDown()
    {
        clicking = true;
        changed = false;
        initRot = val;
    }

    private void OnMouseUp() { clicking = false; }
}