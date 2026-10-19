using UnityEngine;
using UnityEngine.InputSystem;

public class PickMe : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Key pickupKey = Key.E;

    private EntityHolder entityHolder;
    private bool playerInRange;
    private bool isPlaced;

    public bool IsPlaced => isPlaced;

    public void SetPlaced(bool value)
    {
        isPlaced = value;
    }

    public void ResetRange()
    {
        playerInRange = false;
        entityHolder = null;
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (isPlaced)
        {
            return;
        }

        if (Keyboard.current[pickupKey].wasPressedThisFrame)
        {
            entityHolder.PickUpEntity(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        entityHolder = other.GetComponent<EntityHolder>();
        if (entityHolder == null)
        {
            return;
        }

        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (other.GetComponent<EntityHolder>() == entityHolder)
        {
            entityHolder = null;
            playerInRange = false;
        }
    }
}
