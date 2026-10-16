using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CursorController cursorController;
    private WaterRiser waterRiser;

    private void Awake()
    {
        cursorController = GetComponent<CursorController>();
        waterRiser = GetComponent<WaterRiser>();
    }

    private void Start()
    {
        cursorController.CatchMouse();
        waterRiser.StartRising();
    }
}

