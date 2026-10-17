using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { NotStarted, Playing, Ended }

    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        cursorController.CatchMouse();
        state = GameState.Playing;
        waterRiser.StartRising();
    }

    public void EndGame()
    {
        state = GameState.Ended;
        cursorController.ReleaseMouse();
    }
}
