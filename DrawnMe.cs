using UnityEngine;

public class DrawnMe : MonoBehaviour
{
    [SerializeField] private string waterTag = "Water";
    [SerializeField] private float drownDepth = 3f;
    [SerializeField] private float drownTime = 1.5f;

    private Transform water;
    private Rigidbody rb;
    private Collider[] colliders;
    private bool isDrowning;
    private float elapsedTime;
    private float startY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        GameObject waterObject = GameObject.FindGameObjectWithTag(waterTag);
        if (waterObject != null)
        {
            water = waterObject.transform;
        }
        else
        {
            Debug.LogError("Water with tag '" + waterTag + "' not found");
            enabled = false;
        }
    }

    private void Update()
    {
        if (isDrowning)
        {
            UpdateDrowning();
            return;
        }

        if (water.position.y >= transform.position.y)
        {
            Drown();
        }
    }

    public void Drown()
    {
        if (isDrowning)
        {
            return;
        }

        isDrowning = true;
        elapsedTime = 0f;
        startY = transform.position.y;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    private void UpdateDrowning()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / drownTime);
        float newY = Mathf.Lerp(startY, startY - drownDepth, t);

        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;

        if (t >= 1f)
        {
            isDrowning = false;
            gameObject.SetActive(false);
        }
    }
}
