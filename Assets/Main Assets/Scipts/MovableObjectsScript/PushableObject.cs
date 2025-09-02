using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    public float pushSpeed = 2f;
    public float checkRadius = 0.45f;
    public float checkDistance = 0.1f;
    public LayerMask obstacleMask;

    private Rigidbody rb;
    private Vector3 pushDirection = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Push(Vector3 direction)
    {
        pushDirection = new Vector3(direction.x, 0, direction.z).normalized;
    }

    void FixedUpdate()
    {
        if (pushDirection == Vector3.zero) return;

        Vector3 move = pushDirection * pushSpeed * Time.fixedDeltaTime;
        Vector3 start = transform.position + Vector3.up * 0.5f;
        Vector3 end = start + Vector3.up * 0.5f;

        // Capsule check prevents clipping
        if (!Physics.CapsuleCast(start, end, checkRadius, pushDirection, out RaycastHit hit, checkDistance + move.magnitude, obstacleMask))
        {
            rb.MovePosition(rb.position + move);
        }

        pushDirection = Vector3.zero;
    }
}
