using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 10, -6);
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float lookAhead = 0.35f;     // seconds
    Rigidbody targetBody;
    Vector3 velocity;                             // camera SmoothDamp memory
    Vector3 ahead, aheadVelocity;                 // smoothed look-ahead

    void Awake()
    {
        if (target == null)
            target = FindAnyObjectByType<PlayerController>().transform;
        targetBody = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        // the look-ahead is smoothed too, so a wall hit can't make it jump
        Vector3 v = targetBody.linearVelocity;
        Vector3 wantedAhead = new Vector3(v.x, 0f, v.z) * lookAhead;
        ahead = Vector3.SmoothDamp(ahead, wantedAhead,
                                   ref aheadVelocity, smoothTime);

        Vector3 aim = target.position + ahead;
        transform.position = Vector3.SmoothDamp(
            transform.position, aim + offset, ref velocity, smoothTime);
        transform.LookAt(aim);
    }
}