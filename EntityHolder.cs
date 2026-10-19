using UnityEngine;
using UnityEngine.InputSystem;

public class EntityHolder : MonoBehaviour
{
    [SerializeField] private float holdDistance = 1f;
    [SerializeField] private float holdHeight = 1f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwUpAngle = 30f;
    [SerializeField] private Key dropKey = Key.Space;
    [SerializeField] private float holdSmoothing = 15f;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;

    private Transform cameraTransform;
    private GameObject heldEntity;
    private AudioSource audioSource;
    private Vector3 currentHoldPosition;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (heldEntity == null)
        {
            return;
        }

        Vector3 targetPosition = transform.position
                                 + transform.forward * holdDistance
                                 + transform.up * holdHeight;

        float t = 1f - Mathf.Exp(-holdSmoothing * Time.deltaTime);
        currentHoldPosition = Vector3.Lerp(currentHoldPosition, targetPosition, t);

        heldEntity.transform.position = currentHoldPosition;
        heldEntity.transform.rotation = transform.rotation;
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

        PickMe pickMe = entity.GetComponent<PickMe>();
        if (pickMe != null)
        {
            pickMe.ResetRange();
        }

        currentHoldPosition = transform.position
                              + transform.forward * holdDistance
                              + transform.up * holdHeight;

        heldEntity.transform.position = currentHoldPosition;
        heldEntity.transform.rotation = transform.rotation;

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

        PlaySound(pickupClip);
    }

    public void DropEntity()
    {
        if (!IsHoldingEntity()) return;

        GameObject entity = heldEntity;
        heldEntity = null;

        Collider[] colliders = entity.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        Rigidbody rb = entity.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            Quaternion throwRotation = Quaternion.AngleAxis(-throwUpAngle, cameraTransform.right);
            Vector3 throwDirection = throwRotation * cameraTransform.forward;

            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        Fly fly = entity.GetComponent<Fly>();
        if (fly != null)
        {
            fly.SetStatic(false);
        }

        PlaySound(dropClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
