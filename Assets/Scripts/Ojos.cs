using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ojos : MonoBehaviour
{
    [SerializeField] private GameObject eyePrefab;
    [SerializeField] private Sprite closedEyeSprite;
    [SerializeField] private int eyeCount;
    [SerializeField] private float timeLimit;
    [SerializeField] private float edgePadding;
    [SerializeField] private float minDistanceBetween;

    private class Eye
    {
        public Collider2D collider;
        public SpriteRenderer renderer;
        public bool closed;
    }

    private readonly List<Eye> eyes = new List<Eye>();
    private Camera mainCamera;
    private float timer;
    private int closedCount;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        timer = timeLimit;
        SpawnEyes();
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
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            foreach (Eye eye in eyes)
            {
                if (!eye.closed && eye.collider.OverlapPoint(mouseWorldPos))
                {
                    CloseEye(eye);
                    return;
                }
            }
        }
    }

    private void CloseEye(Eye eye)
    {
        eye.closed = true;
        closedCount++;

        if (eye.renderer != null && closedEyeSprite != null)
        {
            eye.renderer.sprite = closedEyeSprite;
        }
        else
        {
            eye.collider.gameObject.SetActive(false);
        }

        if (closedCount >= eyes.Count)
        {
            EndRound(true);
        }
    }

    private void SpawnEyes()
    {
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < eyeCount; i++)
        {
            Vector2 position = GetNonOverlappingPosition(usedPositions);
            usedPositions.Add(position);

            Vector3 pos = new Vector3(position.x, position.y, eyePrefab.transform.position.z);
            GameObject go = Instantiate(eyePrefab, pos, Quaternion.identity, transform);

            Collider2D col = go.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogError("El prefab del ojo necesita un Collider2D.");
                Destroy(go);
                continue;
            }

            eyes.Add(new Eye
            {
                collider = col,
                renderer = go.GetComponent<SpriteRenderer>(),
                closed = false
            });
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
                if (Vector2.Distance(candidate, used) < minDistanceBetween)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose) return candidate;
        }
        return new Vector2(
            Random.Range(-camWidth + edgePadding, camWidth - edgePadding),
            Random.Range(-camHeight + edgePadding, camHeight - edgePadding));
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }
}
