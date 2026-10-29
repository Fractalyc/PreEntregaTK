using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Esquivar : MonoBehaviour
{
    [SerializeField] private Collider2D player;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float timeLimit;
    [SerializeField] private float startDelay;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float obstacleSpeed;
    [SerializeField] private float aimSpread;
    [SerializeField] private float spawnMargin;

    private class Obstacle
    {
        public Transform transform;
        public Collider2D collider;
        public Vector2 velocity;
    }

    private readonly List<Obstacle> obstacles = new List<Obstacle>();
    private Camera mainCamera;
    private float timer;
    private float spawnTimer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        timer = timeLimit;
        spawnTimer = startDelay;
    }

    private void Update()
    {
        if (gameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndRound(true);
            return;
        }

        FollowCursor();
        UpdateSpawning();
        MoveObstacles();
        CheckCollisions();
    }

    private void FollowCursor()
    {
        if (Mouse.current == null) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        player.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, player.transform.position.z);
    }

    private void UpdateSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnObstacle()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Aparece fuera de pantalla por un borde al azar
        Vector2 spawnPos;
        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPos = new Vector2(-camWidth - spawnMargin, Random.Range(-camHeight, camHeight));
                break;
            case 1:
                spawnPos = new Vector2(camWidth + spawnMargin, Random.Range(-camHeight, camHeight));
                break;
            case 2:
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), camHeight + spawnMargin);
                break;
            default:
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), -camHeight - spawnMargin);
                break;
        }

        Vector2 target = (Vector2)player.transform.position + Random.insideUnitCircle * aimSpread;
        Vector2 direction = (target - spawnPos).normalized;

        GameObject go = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
        Collider2D col = go.GetComponent<Collider2D>();
        if (col == null)
        {
            Destroy(go);
            return;
        }

        obstacles.Add(new Obstacle
        {
            transform = go.transform,
            collider = col,
            velocity = direction * obstacleSpeed
        });
    }

    private void MoveObstacles()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            Obstacle o = obstacles[i];
            o.transform.position += (Vector3)(o.velocity * Time.deltaTime);

            Vector2 pos = o.transform.position;
            bool outside = Mathf.Abs(pos.x) > camWidth + spawnMargin || Mathf.Abs(pos.y) > camHeight + spawnMargin;
            bool movingAway = Vector2.Dot(pos, o.velocity) > 0f;

            if (outside && movingAway)
            {
                Destroy(o.transform.gameObject);
                obstacles.RemoveAt(i);
            }
        }
    }

    private void CheckCollisions()
    {
        Physics2D.SyncTransforms();

        foreach (Obstacle o in obstacles)
        {
            if (player.Distance(o.collider).isOverlapped)
            {
                EndRound(false);
                return;
            }
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }
}
