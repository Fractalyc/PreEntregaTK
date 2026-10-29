using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Rub : MonoBehaviour
{
    [SerializeField] private float requiredDistance;
    [SerializeField] private float timeLimit;
    [SerializeField] private SpriteRenderer dirtSprite;

    private Camera mainCamera;
    private Collider2D areaCollider;
    private float timer;
    private float rubbedDistance;
    private Vector2 lastMouseWorldPos;
    private bool wasInside = false;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        areaCollider = GetComponent<Collider2D>();

        if (dirtSprite == null)
        {
            dirtSprite = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        timer = timeLimit;
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

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        bool isInside = areaCollider.OverlapPoint(mouseWorldPos);

        if (isInside && wasInside)
        {
            rubbedDistance += Vector2.Distance(mouseWorldPos, lastMouseWorldPos);
            UpdateVisual();

            if (rubbedDistance >= requiredDistance)
            {
                EndRound(true);
                return;
            }
        }

        lastMouseWorldPos = mouseWorldPos;
        wasInside = isInside;
    }

    private void UpdateVisual()
    {
        if (dirtSprite == null) return;
        Color c = dirtSprite.color;
        c.a = 1f - Mathf.Clamp01(rubbedDistance / requiredDistance);
        dirtSprite.color = c;
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }
}
