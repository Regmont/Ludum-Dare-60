using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private float mouseSensitivity = 10f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    public void Update()
    {
        Rotate();
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
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
        cameraTransform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }
}
