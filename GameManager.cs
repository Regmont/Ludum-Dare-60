using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState { NotStarted, Playing, Ended }

    public static GameManager Instance { get; private set; }

    [SerializeField] private string startButtonName = "StartButton";
    [SerializeField] private TMP_Text timerText;

    private CursorController cursorController;
    private WaterRiser waterRiser;

    private GameState state = GameState.NotStarted;

    public bool IsPlaying => state == GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cursorController = GetComponent<CursorController>();
        waterRiser = GetComponent<WaterRiser>();
    }

    private void Update()
    {
        if (state != GameState.Playing)
        {
            return;
        }

        timerText.text = Mathf.CeilToInt(waterRiser.RemainingTime).ToString();
    }

    public void StartGame()
    {
        cursorController.CatchMouse();
        state = GameState.Playing;
        waterRiser.StartRising();

        GameObject startButton = GameObject.Find(startButtonName);
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        timerText.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        state = GameState.Ended;
        cursorController.ReleaseMouse();
        timerText.gameObject.SetActive(false);
    }
}
