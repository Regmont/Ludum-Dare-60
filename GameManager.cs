using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CursorController cursorController;

    private void Awake()
    {
        cursorController = GetComponent<CursorController>();
    }

    private void Start()
    {
        cursorController.CatchMouse();
    }
}
