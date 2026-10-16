using UnityEngine;

public class WaterRiser : MonoBehaviour
{
    [SerializeField] private Transform water;
    [SerializeField] private float targetY = 5f;
    [SerializeField] private float riseTime = 30f;

    private float startY;
    private float elapsedTime;
    private bool isRising;

    public bool IsRising => isRising;
    public float RemainingTime => Mathf.Max(riseTime - elapsedTime, 0f);

    public void StartRising()
    {
        if (isRising)
        {
            return;
        }

        startY = water.position.y;
        elapsedTime = 0f;
        isRising = true;
    }

    private void Update()
    {
        if (!isRising)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / riseTime);
        float newY = Mathf.Lerp(startY, targetY, t);

        Vector3 pos = water.position;
        pos.y = newY;
        water.position = pos;

        if (t >= 1f)
        {
            isRising = false;
        }
    }
}
