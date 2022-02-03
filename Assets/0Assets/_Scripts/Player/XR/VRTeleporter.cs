﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTeleporter : MonoBehaviour
{

    public GameObject PositionMarker; // marker for display ground position

    public GameObject Player; // target transferred by teleport

    public LayerMask ExcludeLayers; // excluding for performance

    private CharacterController PlayerCharacterController;
    private Transform PlayerTransform;

    public float angle = 45f; // Arc take off angle

    public float strength = 10f; // Increasing this value will increase overall arc length


    int maxVertexcount = 100; // limitation of vertices for performance. 

    private float vertexDelta = 0.08f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.

    private LineRenderer arcRenderer;

    private Vector3 velocity; // Velocity of latest vertex

    private Vector3 groundPos; // detected ground position

    private Vector3 lastNormal; // detected surface normal

    private bool groundDetected = false;

    private List<Vector3> vertexList = new List<Vector3>(); // vertex on arc

    private bool displayActive = false; // don't update path when it's false.


    private void Start()
    {
        PlayerCharacterController = Player.GetComponent<CharacterController>();
        PlayerTransform = Player.GetComponent<Transform>();
    }

    // Teleport target transform to ground position
    public void Teleport()
    {
        if (groundDetected)
        {
            PlayerCharacterController.enabled = false;
            PlayerTransform.position = groundPos + lastNormal * 0.1f;
            PlayerCharacterController.enabled = true;
            Debug.Log("YEAH! TELEPORT");
        }
        else
        {
            Debug.Log("Ground wasn't detected");
        }
    }

    // Active Teleporter Arc Path
    public void ToggleDisplay(bool active)
    {
        arcRenderer.enabled = active;
        PositionMarker.SetActive(active);
        displayActive = active;
    }





    private void Awake()
    {
        arcRenderer = GetComponent<LineRenderer>();
        arcRenderer.enabled = false;
        PositionMarker.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (displayActive)
        {
            UpdatePath();
        }
    }


    private void UpdatePath()
    {
        groundDetected = false;

        vertexList.Clear(); // delete all previouse vertices


        velocity = Quaternion.AngleAxis(-angle, transform.right) * transform.forward * strength;

        RaycastHit hit;


        Vector3 pos = transform.position; // take off position

        vertexList.Add(pos);

        while (!groundDetected && vertexList.Count < maxVertexcount)
        {
            Vector3 newPos = pos + velocity * vertexDelta
                + 0.5f * Physics.gravity * vertexDelta * vertexDelta;

            velocity += Physics.gravity * vertexDelta;

            vertexList.Add(newPos); // add new calculated vertex

            // linecast between last vertex and current vertex
            if (Physics.Linecast(pos, newPos, out hit, ~ExcludeLayers))
            {
                groundDetected = true;
                groundPos = hit.point;
                lastNormal = hit.normal;
            }
            pos = newPos; // update current vertex as last vertex
        }


        PositionMarker.SetActive(groundDetected);

        if (groundDetected)
        {
            PositionMarker.transform.position = groundPos + lastNormal * 0.1f;
            PositionMarker.transform.LookAt(groundPos);
        }

        // Update Line Renderer

        arcRenderer.positionCount = vertexList.Count;
        arcRenderer.SetPositions(vertexList.ToArray());
    }


}