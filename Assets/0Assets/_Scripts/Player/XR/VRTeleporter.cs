using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VRTeleporter : MonoBehaviour
{
    public GameObject PositionMarker; // marker for display ground position
    public GameObject Player; // target transferred by teleport
    public LayerMask ExcludeLayers; // excluding for performance
    public LayerMask HitLayers; // layers to not teleport to
    public float angle = 45f; // Arc take off angle
    public float strength = 10f; // Increasing this value will increase overall arc length
    private Color goodColor;
    private Transform PlayerTransform;
    private CharacterController PlayerCharacterController;
    private int maxVertexcount = 100; // limitation of vertices for performance. 
    private float vertexDelta = 0.06f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.
    private LineRenderer arcRenderer;
    private Vector3 velocity; // Velocity of latest vertex
    private Vector3 groundPos; // detected ground position
    private Vector3 lastNormal; // detected surface normal
    private bool goodGround = false;
    private bool groundDetected = false;
    private List<Vector3> vertexList = new List<Vector3>(); // vertex on arc
    private bool displayActive = false; // don't update path when it's false.
    private void Awake()
    {
        arcRenderer = GetComponent<LineRenderer>();
        arcRenderer.enabled = false;
        PositionMarker.SetActive(false);
    }
    private void Start()
    {
        goodColor = arcRenderer.startColor;
        PlayerCharacterController = Player.GetComponent<CharacterController>();
        PlayerTransform = Player.GetComponent<Transform>();
    }
    // Teleport target transform to ground position
    public void Teleport()
    {
        if (groundDetected && goodGround)
        {
            PlayerCharacterController.enabled = false;
            PlayerTransform.position = groundPos + lastNormal * 0.1f;
            PlayerCharacterController.enabled = true;
        }
    }
    public void Teleport(float angle)
    {
        if (groundDetected && goodGround)
        {
            PlayerCharacterController.enabled = false;
            PlayerTransform.position = groundPos + lastNormal * 0.1f;
            PlayerTransform.eulerAngles = new Vector3(PlayerTransform.eulerAngles.x, -angle, PlayerTransform.eulerAngles.z);
            PlayerCharacterController.enabled = true;
        }
    }
    // Active Teleporter Arc Path
    public void ToggleDisplay(bool active)
    {
        arcRenderer.enabled = active;
        PositionMarker.SetActive(active);
        displayActive = active;
    }
    private void FixedUpdate()
    {
        if (displayActive) { UpdatePath(); }
    }
    private void UpdatePath()
    {
        groundDetected = false;
        goodGround = false;
        vertexList.Clear(); // delete all previouse vertices
        velocity = Quaternion.AngleAxis(-angle, transform.right) * transform.forward * strength;
        RaycastHit hit;
        Vector3 pos = transform.position; // take off position
        vertexList.Add(pos);
        while (!groundDetected && vertexList.Count < maxVertexcount)
        {
            Vector3 newPos = pos + velocity * vertexDelta + 0.5f * Physics.gravity * vertexDelta * vertexDelta;
            velocity += Physics.gravity * vertexDelta;
            vertexList.Add(newPos); // add new calculated vertex
            // linecast between last vertex and current vertex
            if (Physics.Linecast(pos, newPos, out hit, ~ExcludeLayers))
            {
                groundDetected = true;
                groundPos = hit.point;
                lastNormal = hit.normal;
                if (HitLayers == (HitLayers | (1 << hit.collider.gameObject.layer)))
                {
                    arcRenderer.startColor = Color.red;
                    arcRenderer.endColor = Color.red;
                }
                else
                {
                    goodGround = true;
                    arcRenderer.startColor = goodColor;
                    arcRenderer.endColor = goodColor;
                }
            }
            pos = newPos; // update current vertex as last vertex
        }
        PositionMarker.SetActive(goodGround);
        if (groundDetected)
        {
            PositionMarker.transform.position = groundPos;
            PositionMarker.transform.LookAt(groundPos - lastNormal);
        }
        // Update Line Renderer
        arcRenderer.positionCount = vertexList.Count;
        arcRenderer.SetPositions(vertexList.ToArray());
    }
}