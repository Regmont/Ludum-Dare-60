using UnityEngine;

public class EntityPlacer : MonoBehaviour
{
    public static EntityPlacer Instance { get; private set; }

    [System.Serializable]
    public class PointSlot
    {
        public Transform point;
        public string entityName;
    }

    [SerializeField] private PointSlot[] slots;
    [SerializeField] private WaterRiser waterRiser;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool TryPlace(Fly fly)
    {
        Transform target = FindSlotFor(fly.gameObject.name);
        if (target == null)
        {
            return false;
        }

        if (fly.transform.position.y <= waterRiser.TargetY)
        {
            return false;
        }

        fly.transform.position = target.position;
        fly.transform.rotation = target.rotation;
        fly.SetStatic(true);

        PickMe pickMe = fly.GetComponent<PickMe>();
        if (pickMe != null)
        {
            pickMe.SetPlaced(true);
        }

        return true;
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
}
