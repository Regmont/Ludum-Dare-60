using UnityEngine;
using UnityEngine.InputSystem;

public class EntityHolder : MonoBehaviour
{
    [SerializeField] private float holdDistance = 1f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Key dropKey = Key.Space;
    [SerializeField] private float holdSmoothing = 15f;

    private Transform holdPoint;
    private Transform cameraTransform;
    private GameObject heldEntity;

    private void Awake()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;

        GameObject holdPointObject = new GameObject("HoldPoint");
        holdPointObject.transform.SetParent(null);
        holdPointObject.transform.position = cameraTransform.position + cameraTransform.forward * holdDistance;
        holdPointObject.transform.rotation = cameraTransform.rotation;
        holdPoint = holdPointObject.transform;
    }

    private void LateUpdate()
    {
        if (holdPoint == null)
        {
            return;
        }

        Quaternion targetRotation = cameraTransform.rotation;
        Vector3 targetPosition = cameraTransform.position + targetRotation * Vector3.forward * holdDistance;

        float t = 1f - Mathf.Exp(-holdSmoothing * Time.deltaTime);

        holdPoint.rotation = Quaternion.Slerp(holdPoint.rotation, targetRotation, t);
        holdPoint.position = Vector3.Lerp(holdPoint.position, targetPosition, t);
    }

    private void Update()
    {
        if (Keyboard.current[dropKey].wasPressedThisFrame)
        {
            DropEntity();
        }
    }

    public bool IsHoldingEntity()
    {
        return heldEntity != null;
    }

    public void PickUpEntity(GameObject entity)
    {
        if (entity == null) return;
        if (IsHoldingEntity()) return;

        heldEntity = entity;
        heldEntity.transform.SetParent(holdPoint);
        heldEntity.transform.localPosition = Vector3.zero;
        heldEntity.transform.localRotation = Quaternion.identity;

        Rigidbody rb = heldEntity.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders = heldEntity.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Fly fly = heldEntity.GetComponent<Fly>();
        if (fly != null)
        {
            fly.SetStatic(true);
        }
    }

    public void DropEntity()
    {
        if (!IsHoldingEntity()) return;

        GameObject entity = heldEntity;
        heldEntity = null;

        entity.transform.SetParent(null);

        Collider[] colliders = entity.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        Rigidbody rb = entity.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
        }

        Fly fly = entity.GetComponent<Fly>();
        if (fly != null)
        {
            fly.SetStatic(false);
        }
    }

    public GameObject PlaceEntity()
    {
        if (!IsHoldingEntity()) return null;

        GameObject entity = heldEntity;
        heldEntity = null;

        entity.transform.SetParent(null);

        Fly fly = entity.GetComponent<Fly>();
        if (fly != null)
        {
            fly.SetStatic(true);
        }

        return entity;
    }
}
