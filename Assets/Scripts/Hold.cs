using UnityEngine;
using UnityEngine.InputSystem;
public class Hold : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    [SerializeField] private float timeLimit = 3f;

    private float elapsedSinceStart;
    private bool timerActive = false;
    private float timer;
    private bool gameOver = false;
    public bool IsHolding { get; private set; }

    private void Start()
    {
        timer = timeLimit;
    }

    private void Update()
    {
        if (gameOver) return;

        IsHolding = Mouse.current != null && Mouse.current.leftButton.isPressed;
        elapsedSinceStart += Time.deltaTime;

        if (!timerActive)
        {
            if (elapsedSinceStart < delay)
            {
                return;
            }
            timerActive = true;
        }

        if (!IsHolding)
        {
            EndRound(false);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndRound(true);
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log((won ? "¡GANASTE! " : "PERDISTE. "));
        GameManager.ReportResult(won);
    }
}