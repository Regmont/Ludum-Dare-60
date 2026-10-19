using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 4f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Collision")]
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionSmoothSpeed = 10f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentDistance;

    private void Start()
    {
        currentDistance = distance;
    }

    public void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        Rotate();
        Zoom();
        Follow();
    }

    public void SetMouseSensitivity(float mouseSensitivity)
    {
        this.mouseSensitivity = mouseSensitivity;
    }

    private void Rotate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, minPitch, maxPitch);
    }

    private void Zoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void Follow()
    {
        target.rotation = Quaternion.Euler(0f, xRotation, 0f);

        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0f);

        Vector3 pivotPosition = target.position + Vector3.up * height;
        Vector3 offsetDirection = rotation * Vector3.back;

        RaycastHit hit;
        if (Physics.SphereCast(pivotPosition, cameraRadius, offsetDirection, out hit, distance, collisionMask))
        {
            currentDistance = Mathf.Lerp(currentDistance, Mathf.Clamp(hit.distance, minDistance, distance), Time.deltaTime * collisionSmoothSpeed);
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * collisionSmoothSpeed);
        }

        Vector3 targetPosition = pivotPosition + offsetDirection * currentDistance;

        transform.position = targetPosition;
        transform.rotation = rotation;
    }
}
