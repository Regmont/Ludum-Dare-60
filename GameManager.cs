using System.Collections;
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
    [SerializeField] private RawImage startImage;

    [Header("End UI")]
    [SerializeField] private Image fadeScreen;
    [SerializeField] private RawImage endImage;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float appearanceDuration = 1f;

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

    private void Start()
    {
        if (fadeScreen != null)
        {
            fadeScreen.color = new Color(0, 0, 0, 0);
            fadeScreen.gameObject.SetActive(false);
        }
        if (endImage != null)
        {
            endImage.color = new Color(1, 1, 1, 0);
            endImage.gameObject.SetActive(false);
        }
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

        startImage.gameObject.SetActive(false);

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
        if (!isPlayerAlive) return;

        isPlayerAlive = false;
        EndGame();
    }

    public void EndGame()
    {
        if (state == GameState.Ended) return;

        state = GameState.Ended;
        cursorController.ReleaseMouse();

        if (timerText != null) timerText.gameObject.SetActive(false);
        if (staminaBar != null) staminaBar.SetActive(false);

        bool[] occupied = EntityPlacer.Instance.Occupied;
        string occupiedString = "";
        for (int i = 0; i < occupied.Length; i++)
        {
            occupiedString += occupied[i] ? "1" : "0";
        }
        occupiedString += isPlayerAlive ? "1" : "0";

        StartCoroutine(EndSequence(occupiedString));
    }

    private IEnumerator EndSequence(string mask)
    {
        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            float elapsed = 0f;
            Color color = fadeScreen.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeScreen.color = color;
                yield return null;
            }
            color.a = 1f;
            fadeScreen.color = color;
        }

        if (endImage != null)
        {
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
            float elapsed = 0f;
            Color color = endImage.color;
            color.a = 0f;
            endImage.color = color;

            while (elapsed < appearanceDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsed / appearanceDuration);
                endImage.color = color;
                yield return null;
            }
            color.a = 1f;
            endImage.color = color;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
