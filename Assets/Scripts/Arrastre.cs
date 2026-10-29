using UnityEngine;
using UnityEngine.InputSystem;

public class Arrastre : MonoBehaviour
{
    [SerializeField] private Collider2D draggable;
    [SerializeField] private Collider2D dropZone;
    [SerializeField] private float timeLimit;
    [SerializeField] private bool randomizePositions = true;
    [SerializeField] private float edgePadding;
    [SerializeField] private float minDistanceBetween;

    private Camera mainCamera;
    private float timer;
    private bool gameOver = false;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        timer = timeLimit;

        if (randomizePositions)
        {
            RandomizePositions();
        }
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

        Vector2 mouseWorldPos = GetMouseWorldPos();

        if (!isDragging && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (draggable.OverlapPoint(mouseWorldPos))
            {
                isDragging = true;
                dragOffset = draggable.transform.position - (Vector3)mouseWorldPos;
            }
        }

        if (isDragging)
        {
            Vector3 newPos = (Vector3)mouseWorldPos + dragOffset;
            newPos.z = draggable.transform.position.z;
            draggable.transform.position = newPos;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                CheckDrop();
            }
        }
    }

    private void CheckDrop()
    {
        Vector2 center = draggable.transform.position;
        EndRound(dropZone.OverlapPoint(center));
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        isDragging = false;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }

    private Vector2 GetMouseWorldPos()
    {
        return mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private void RandomizePositions()
    {
        Vector2 a = GetRandomPointInView();
        Vector2 b = a;

        const int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            b = GetRandomPointInView();
            if (Vector2.Distance(a, b) >= minDistanceBetween) break;
        }

        draggable.transform.position = new Vector3(a.x, a.y, draggable.transform.position.z);
        dropZone.transform.position = new Vector3(b.x, b.y, dropZone.transform.position.z);
    }

    private Vector2 GetRandomPointInView()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float x = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
        float y = Random.Range(-camHeight + edgePadding, camHeight - edgePadding);

        return new Vector2(x, y);
    }
}
