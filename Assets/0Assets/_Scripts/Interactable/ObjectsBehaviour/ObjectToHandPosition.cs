using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectToHandPosition : SpecialInteractable
{
    [Header("Object to Hand Position settings")]
    public bool ThrowToResetPosition; 

    private Dictionary<int,Transform> hands;
    private ChangeInteractionColour _interactionColor;

    [SerializeField, Tooltip("Coordinates to added to object position when the user pick it")]
    private float y_offset = 0f;
    private RigidbodyConstraints _previewConstrains;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        allowPermanentClick = true;
        allowTranslation = true;
        _previewConstrains = _rb.constraints;
        _rb.constraints = RigidbodyConstraints.FreezeAll;

        hands = new Dictionary<int, Transform>(2);

        var haveInteraction = TryGetComponent(out _interactionColor);
        if (!haveInteraction)
            _interactionColor = GetComponentInChildren<ChangeInteractionColour>();
    }

    // Special Interactable Functions
    public override void Drop(bool isXR = false)
    {
        _rb.constraints = _previewConstrains;
    }

    public override void Grab(bool isXR = false)
    {
        if (isXR && hands.Count > 0)
        {
            List<Transform> list = hands.Values.ToList();
            Transform newPosition = Utils.GetNearestObject(_transform, list);

            _transform.rotation = newPosition.rotation;
            _transform.position = newPosition.position;
            _transform.localPosition = _transform.localPosition + y_offset * _transform.up;

            //Avoid always see interaction color
            _interactionColor.SetDefaultColor();
        }
    }

    public override void Throw(bool isXR = false)
    {
        if (ThrowToResetPosition)
        {
            ResetPosition();
            this.ForceBreak();            

            hands.Clear();
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // Trigger events
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.HANDS_TAG))
        {
            var handID = other.GetInstanceID();
            var handTransform = other.GetComponent<Transform>();
            hands.Add(handID, handTransform);
            _rb.constraints = _previewConstrains;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.HANDS_TAG))
        {
            var handID = other.GetInstanceID();
            hands.Remove(handID);
        }
    }
}
