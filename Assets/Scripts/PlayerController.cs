using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 1.8f;      // metres
    [SerializeField] float fallMultiplier = 2.2f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBuffer = 0.12f;
    [SerializeField] Transform body;               // the capsule mesh child
    [SerializeField] float squashDuration = 0.25f;

    Rigidbody rb;
    Transform cam;
    Vector3 move;
    bool isGrounded, wasGrounded, jumpQueued;
    float lastGroundedTime = -1f, lastPressedTime = -1f;
    float airTime;
    Vector3 contactPoint, contactNormal;           // for the gizmo

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        cam = Camera.main.transform;
    }

    void Update()
    {
        // read input here, not in FixedUpdate
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // move relative to the camera, flat on the ground
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();
        move = Vector3.ClampMagnitude(forward * v + right * h, 1f);

        // coyote time and jump buffer (lecture 05)
        if (isGrounded && !jumpQueued && rb.linearVelocity.y <= 0f)
            lastGroundedTime = Time.time;
        if (Input.GetKeyDown(KeyCode.Space))
            lastPressedTime = Time.time;

        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        bool wantJump = Time.time - lastPressedTime <= jumpBuffer;
        if (canJump && wantJump)
        {
            jumpQueued = true;
            lastGroundedTime = lastPressedTime = -1f;   // used up
        }

        // juice: only on a real landing, not every grounded frame
        if (!isGrounded)
            airTime += Time.deltaTime;
        if (isGrounded && !wasGrounded && airTime > 0.1f)
        {
            StopAllCoroutines();
            StartCoroutine(Squash());
        }
        if (isGrounded)
            airTime = 0f;
        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            move.x * speed, rb.linearVelocity.y, move.z * speed);

        if (jumpQueued)
        {
            // v = sqrt(2gh), and impulse divides by mass so multiply by it
            float g = -Physics.gravity.y;
            float v0 = Mathf.Sqrt(2f * g * jumpHeight);

            // zero any fall first so a late jump is full height
            Vector3 vel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);
            rb.AddForce(Vector3.up * rb.mass * v0, ForceMode.Impulse);
            jumpQueued = false;
        }

        if (rb.linearVelocity.y < 0f)   // falling: pull harder
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f),
                        ForceMode.Acceleration);
    }

    void OnCollisionStay(Collision c)
    {
        ContactPoint contact = c.GetContact(0);
        // a wall isn't ground, so the normal has to point up
        if (contact.normal.y > 0.5f)
        {
            isGrounded = true;
            contactPoint = contact.point;
            contactNormal = contact.normal;
        }
    }

    void OnCollisionExit(Collision c) => isGrounded = false;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(contactPoint, contactNormal);
    }

    IEnumerator Squash()
    {
        for (float elapsed = 0f; elapsed < squashDuration; elapsed += Time.deltaTime)
        {
            float p = Ease.OutBack(elapsed / squashDuration);
            float y = Mathf.LerpUnclamped(0.7f, 1f, p);
            float xz = 1f / Mathf.Sqrt(y);          // keep the volume
            body.localScale = new Vector3(xz, y, xz);
            yield return null;
        }
        body.localScale = Vector3.one;              // back to rest exactly
    }
}