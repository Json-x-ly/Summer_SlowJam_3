using UnityEngine;
using System.Collections;

public class AnchorToVisualSpace : MonoBehaviour
{
    private float y;
    void Awake()
    {
        y = transform.position.y;
    }
    void Update()
    {
        transform.position = LogicVisualSpace.LogicToVisual(new Vector3(transform.position.x, y, transform.position.z));
    }
}
