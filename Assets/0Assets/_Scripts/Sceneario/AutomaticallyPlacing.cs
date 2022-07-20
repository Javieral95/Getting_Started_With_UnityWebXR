using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using System.Linq;

public class AutomaticallyPlacing : MonoBehaviour
{
    private GameManager GameManager;
    [Tooltip("Only select one!!")]
    public Axis AxisToInstantiate;
    public Vector3 InitPosition;
    public float ObjectSeparation = 0.3f;

    [Header("Object Panel Settings"), Tooltip("If ManuallySelectObjects is true you must select the objects to instantiate, if is false the script will place automatically the childrens of this GameObject")]
    public bool ManuallySelectObjects;
    [ConditionalHide("ManuallySelectObjects")]
    [Tooltip(
"Objects to instantiate")]
    public List<GameObject> InitObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();

        if (ManuallySelectObjects)
        {
            foreach (var temp in InitObjects)
                GameManager.InstantiateNewObject(temp, InitPosition, temp.transform.rotation);
        }
        else
        {
            foreach (Transform child in this.transform)            
                InitObjects.Add(child.gameObject);            
        }

        PlaceObjects();

    }

    private void PlaceObjects()
    {
        Vector3 tempPosition = InitPosition;
        float limits = (ObjectSeparation * InitObjects.Count) / 2f;

        if (AxisToInstantiate == Axis.X) tempPosition.x += limits;
        if (AxisToInstantiate == Axis.Y) tempPosition.y += limits;
        if (AxisToInstantiate == Axis.Z) tempPosition.z += limits;

        foreach(var tempObject in InitObjects)
        {
            tempObject.transform.localPosition = tempPosition;

            if (AxisToInstantiate == Axis.X) tempPosition.x -= ObjectSeparation;
            if (AxisToInstantiate == Axis.Y) tempPosition.y -= ObjectSeparation;
            if (AxisToInstantiate == Axis.Z) tempPosition.z -= ObjectSeparation;
        }
    }
}
