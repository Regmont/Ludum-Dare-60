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

    private bool[] occupied;

    public bool[] Occupied => occupied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        occupied = new bool[slots.Length];
    }

    public bool TryPlace(Fly fly)
    {
        int index = FindSlotIndexFor(fly.gameObject.name);
        if (index < 0)
        {
            return false;
        }

        if (fly.transform.position.y <= waterRiser.TargetY)
        {
            return false;
        }

        fly.transform.position = slots[index].point.position;
        fly.transform.rotation = slots[index].point.rotation;
        fly.SetStatic(true);

        PickMe pickMe = fly.GetComponent<PickMe>();
        if (pickMe != null)
        {
            pickMe.SetPlaced(true);
        }

        occupied[index] = true;

        return true;
    }

    public int PlacedCount()
    {
        int count = 0;
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i]) count++;
        }
        return count;
    }

    public bool AllPlaced()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (!occupied[i]) return false;
        }
        return true;
    }

    private int FindSlotIndexFor(string entityName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].entityName == entityName)
            {
                return i;
            }
        }
        return -1;
    }
}
