using UnityEngine;

public class DrawnPlayer : MonoBehaviour
{
    [SerializeField] private Transform water;
    [SerializeField] private float drownTime = 0.5f;

    private float drownTimer;
    private bool isTimerRunning;

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (!isTimerRunning)
        {
            if (water.position.y >= transform.position.y)
            {
                isTimerRunning = true;
                drownTimer = drownTime;
            }
            return;
        }

        drownTimer -= Time.deltaTime;

        if (drownTimer <= 0f)
        {
            isTimerRunning = false;
            GameManager.Instance.EndGame();
        }
    }
}
