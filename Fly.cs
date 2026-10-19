using UnityEngine;

public class Fly : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float hoverCheckDistance = 5f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private LayerMask groundMask;

    private enum State { Static, Falling, Rising, Hovering }

    private State state = State.Static;
    private Rigidbody rb;
    private float targetY;

    public bool IsStatic => state == State.Static;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
            case State.Rising:
                Rise();
                break;

            case State.Hovering:
                Rotate();
                Hover();
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Falling) return;

        if (((1 << collision.gameObject.layer) & groundMask) == 0) return;

        BeginHoverSequence(collision.GetContact(0).point);
    }

    private void BeginHoverSequence(Vector3 contactPoint)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;

        targetY = contactPoint.y + hoverHeight;

        rb.isKinematic = true;
        rb.useGravity = false;

        state = State.Rising;

        if (EntityPlacer.Instance != null && EntityPlacer.Instance.TryPlace(this))
        {
            return;
        }
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
