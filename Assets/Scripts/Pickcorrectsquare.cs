using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickcorrectsquare : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private int squareCount;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color wrongColor;
    [SerializeField] private float minDistanceBetweenSquares;
    [SerializeField] private float edgePadding;
    [SerializeField] private float timeLimit;

    private Camera mainCamera;
    private readonly List<GameObject> spawnedSquares = new List<GameObject>();
    private int correctIndex;
    private float timer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        timer = timeLimit;
        SpawnSquares();
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
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        for (int i = 0; i < spawnedSquares.Count; i++)
        {
            Collider2D col = spawnedSquares[i].GetComponent<Collider2D>();
            if (col.OverlapPoint(mouseWorldPos))
            {
                bool isCorrect = (i == correctIndex);
                EndRound(isCorrect);
                return;
            }
        }

        EndRound(false);
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log((won ? "¡GANASTE! " : "¡PERDISTE!"));
        GameManager.ReportResult(won);
    }

    private void SpawnSquares()
    {
        correctIndex = Random.Range(0, squareCount);

        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < squareCount; i++)
        {
            Vector2 position = GetNonOverlappingPosition(usedPositions);
            usedPositions.Add(position);

            GameObject square = Instantiate(squarePrefab, position, Quaternion.identity, transform);
            SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = (i == correctIndex) ? correctColor : wrongColor;
            }

            spawnedSquares.Add(square);
        }
    }

    private Vector2 GetNonOverlappingPosition(List<Vector2> usedPositions)
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        const int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float x = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
            float y = Random.Range(-camHeight + edgePadding, camHeight - edgePadding);
            Vector2 candidate = new Vector2(x, y);

            bool tooClose = false;
            foreach (Vector2 used in usedPositions)
            {
                if (Vector2.Distance(candidate, used) < minDistanceBetweenSquares)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose) return candidate;
        }
        float fallbackX = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
        float fallbackY = Random.Range(-camHeight + edgePadding, camHeight - edgePadding);
        return new Vector2(fallbackX, fallbackY);

    }
}
