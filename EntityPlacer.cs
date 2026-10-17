using UnityEngine;
using UnityEngine.InputSystem;

public class EntityPlacer : MonoBehaviour
{
    [System.Serializable]
    public class PointSlot
    {
        public Transform point;
        public string entityName;
    }

    [SerializeField] private PointSlot[] slots;
    [SerializeField] private Key placeKey = Key.F;
    [SerializeField] private string playerTag = "Player";

    private EntityHolder entityHolder;
    private bool playerInRange;

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (!entityHolder.IsHoldingEntity())
        {
            return;
        }

        if (Keyboard.current[placeKey].wasPressedThisFrame)
        {
            PlaceEntity();
        }
    }

    private void PlaceEntity()
    {
        GameObject placed = entityHolder.PlaceEntity();
        if (placed == null)
        {
            return;
        }

        Transform target = FindSlotFor(placed.name);
        if (target == null)
        {
            return;
        }

        placed.transform.position = target.position;
        placed.transform.rotation = target.rotation;

        Fly fly = placed.GetComponent<Fly>();
        if (fly != null)
        {
            fly.SetStatic(true);
        }

        PickMe pickMe = placed.GetComponent<PickMe>();
        if (pickMe != null)
        {
            pickMe.SetPlaced(true);
        }
    }

    private Transform FindSlotFor(string entityName)
    {
        foreach (PointSlot slot in slots)
        {
            if (slot.entityName == entityName)
            {
                return slot.point;
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        EntityHolder holder = other.GetComponent<EntityHolder>();
        if (holder == null)
        {
            return;
        }

        entityHolder = holder;
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
