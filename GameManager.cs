using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState { NotStarted, Playing, Ended }

    public static GameManager Instance { get; private set; }

    [SerializeField] private string startButtonName = "StartButton";
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private RawImage endImage;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private GameObject staminaBar;

    private CursorController cursorController;
    private WaterRiser waterRiser;

    private GameState state = GameState.NotStarted;
    private bool isPlayerAlive = true;

    public bool IsPlaying => state == GameState.Playing;
    public bool IsPlayerAlive => isPlayerAlive;

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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            QuitGame();
            return;
        }

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

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.gameObject.SetActive(false);
        }

        if (staminaBar != null)
        {
            staminaBar.SetActive(true);
        }
    }

    public void KillPlayer()
    {
        if (!isPlayerAlive)
        {
            return;
        }

        isPlayerAlive = false;
        EndGame();
    }

    public void EndGame()
    {
        if (state == GameState.Ended)
        {
            return;
        }

        state = GameState.Ended;
        cursorController.ReleaseMouse();
        timerText.gameObject.SetActive(false);

        if (staminaBar != null)
        {
            staminaBar.SetActive(false);
        }

        bool[] occupied = EntityPlacer.Instance.Occupied;
        string occupiedString = "";
        for (int i = 0; i < occupied.Length; i++)
        {
            occupiedString += occupied[i] ? "1" : "0";
        }

        occupiedString += isPlayerAlive ? "1" : "0";

        ShowEndImage(occupiedString);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private void ShowEndImage(string mask)
    {
        if (endImage == null)
        {
            return;
        }

        Texture2D texture = Resources.Load<Texture2D>("Sprites/" + mask);
        if (texture != null)
        {
            endImage.texture = texture;
        }
        else
        {
            Debug.LogWarning("Sprite not found: Sprites/" + mask);
        }

        endImage.gameObject.SetActive(true);
    }
}
