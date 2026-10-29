using UnityEngine;
using UnityEngine.InputSystem;
public class Mashing : MonoBehaviour
{
    [SerializeField] private int requiredClicks;
    [SerializeField] private float timeLimit;

    private int currentClicks;
    private float timer;
    private bool gameOver = false;

    private void Start()
    {
        timer = timeLimit;
        currentClicks = 0;
    }

    private void Update()
    {
        if (gameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndRound(false);
            return;
        }

        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentClicks++;

            if (currentClicks >= requiredClicks)
            {
                EndRound(true);
            }
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log((won ? "¡GANASTE! " : "PERDISTE. "));
        GameManager.ReportResult(won);
    }
}