using UnityEngine;

public class GapMeasure : MonoBehaviour
{
    [SerializeField] Collider a, b;

    void Start()
    {
        float gapX = Mathf.Max(a.bounds.min.x - b.bounds.max.x,
                               b.bounds.min.x - a.bounds.max.x);
        float gapZ = Mathf.Max(a.bounds.min.z - b.bounds.max.z,
                               b.bounds.min.z - a.bounds.max.z);
        Debug.Log("Gap along X: " + gapX + " m, along Z: " + gapZ + " m");
    }
}