using UnityEngine;
using UnityEngine.InputSystem;

public class EntityHolder : MonoBehaviour
{
    [SerializeField] private float holdDistance = 1f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Key dropKey = Key.Space;

    private Transform holdPoint;
    private Transform cameraTransform;
    private GameObject heldEntity;

    private void Awake()
    {
        GameObject holdPointObject = new GameObject("HoldPoint");
        holdPointObject.transform.SetParent(transform);
        holdPointObject.transform.localPosition = new Vector3(0f, 0f, holdDistance);
        holdPointObject.transform.localRotation = Quaternion.identity;
        holdPoint = holdPointObject.transform;

        cameraTransform = GetComponentInChildren<Camera>().transform;
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
        if (entity == null)
        {
            return;
        }

        if (IsHoldingEntity())
        {
            return;
        }

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
    }

    public void DropEntity()
    {
        if (!IsHoldingEntity())
        {
            return;
        }

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
    }

    public GameObject PlaceEntity()
    {
        if (!IsHoldingEntity())
        {
            return null;
        }

        GameObject entity = heldEntity;
        heldEntity = null;

        entity.transform.SetParent(null);

        return entity;
    }
}
