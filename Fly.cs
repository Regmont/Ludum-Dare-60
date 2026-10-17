using UnityEngine;

public class Fly : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float hoverCheckDistance = 5f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private float minFallingTime = 0.1f;
    [SerializeField] private LayerMask groundMask = ~0;

    private enum State { Static, Falling, Rising, Hovering }

    private State state = State.Static;
    private Rigidbody rb;
    private Collider[] colliders;
    private float targetY;
    private float fallingTime;

    public bool IsStatic => state == State.Static;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void SetStatic(bool value)
    {
        if (value)
        {
            state = State.Static;
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
        else
        {
            state = State.Falling;
            fallingTime = 0f;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Falling:
                CheckLanding();
                break;

            case State.Rising:
                Rise();
                break;

            case State.Hovering:
                Rotate();
                Hover();
                break;
        }
    }

    private void CheckLanding()
    {
        if (rb == null)
        {
            return;
        }

        fallingTime += Time.deltaTime;

        if (fallingTime < minFallingTime)
        {
            return;
        }

        if (rb.linearVelocity.y <= 0.01f && IsGrounded())
        {
            BeginHoverSequence();
        }
    }

    private bool IsGrounded()
    {
        float radius = 0.2f;
        Vector3 origin = transform.position + Vector3.up * radius;
        return Physics.SphereCast(origin, radius, Vector3.down, out _, hoverCheckDistance, groundMask);
    }

    private void BeginHoverSequence()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;

        Vector3 origin = transform.position + Vector3.up * 0.1f;
        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, hoverCheckDistance, groundMask))
        {
            return;
        }

        targetY = hit.point.y + hoverHeight;

        rb.isKinematic = true;
        rb.useGravity = false;

        state = State.Rising;
    }

    private void Rise()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, riseSpeed * Time.deltaTime);
        transform.position = pos;

        Rotate();

        if (Mathf.Abs(transform.position.y - targetY) < 0.01f)
        {
            state = State.Hovering;
        }
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void Hover()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, 5f * Time.deltaTime);
        transform.position = pos;
    }
}
