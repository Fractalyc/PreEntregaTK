using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSquare : MonoBehaviour
{
    [SerializeField] private float edgePadding;
    [SerializeField] private float timeLimit;
    [SerializeField] private float resultDuration; 
    [SerializeField] private bool missLoses = false;   

    [SerializeField] private Sprite sadSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite verySadSprite;
    [SerializeField] private Sprite[] flowerSprites;
    [SerializeField] private int flowerCount;
    [SerializeField] private float flowerSpawnTime;
    [SerializeField] private float slideDuration;
    [SerializeField] private Vector2 inwardDistanceRange = new Vector2(0.5f, 3.5f);
    [SerializeField] private Vector2 flowerScaleRange = new Vector2(0.12f, 0.25f);
    [SerializeField] private int flowerSortingOrder; 

    private class Flower
    {
        public Transform transform;
        public float startX;
        public float targetX;
        public float y;
        public float spin;
        public float age;
    }

    private readonly List<Flower> flowers = new List<Flower>();
    private float timer;
    private Camera mainCamera;
    private Collider2D squareCollider;
    private SpriteRenderer mirrorRenderer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        squareCollider = GetComponent<Collider2D>();
        mirrorRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        timer = timeLimit;
        mirrorRenderer.sprite = sadSprite;
        MoveToRandomPosition();
    }

    private void Update()
    {
        if (gameOver)
        {
            AnimateFlowers();
            return;
        }

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

            if (squareCollider.OverlapPoint(mouseWorldPos))
            {
                EndRound(true);
            }
            else if (missLoses)
            {
                EndRound(false);
            }
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");

        if (won)
        {
            mirrorRenderer.sprite = happySprite;
            StartCoroutine(SpawnFlowers());
        }
        else
        {
            mirrorRenderer.sprite = verySadSprite;
        }

        StartCoroutine(ReturnToManager(won));
    }

    private IEnumerator ReturnToManager(bool won)
    {
        yield return new WaitForSeconds(resultDuration);
        GameManager.ReportResult(won);
    }


    private IEnumerator SpawnFlowers()
    {
        if (flowerSprites == null || flowerSprites.Length == 0) yield break;

        float delay = flowerSpawnTime / Mathf.Max(1, flowerCount);
        for (int i = 0; i < flowerCount; i++)
        {
            SpawnFlower();
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnFlower()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float side = Random.value < 0.5f ? -1f : 1f;
        float startX = side * (camWidth + 1.5f);
        float targetX = side * (camWidth - Random.Range(inwardDistanceRange.x, inwardDistanceRange.y));
        float y = Random.Range(-camHeight, camHeight);

        GameObject go = new GameObject("Flower");
        go.transform.SetParent(transform.parent);
        go.transform.position = new Vector3(startX, y, 0f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        go.transform.localScale = Vector3.one * Random.Range(flowerScaleRange.x, flowerScaleRange.y);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = flowerSprites[Random.Range(0, flowerSprites.Length)];
        sr.sortingOrder = flowerSortingOrder;

        flowers.Add(new Flower
        {
            transform = go.transform,
            startX = startX,
            targetX = targetX,
            y = y,
            spin = Random.Range(-120f, 120f),
            age = 0f
        });
    }

    private void AnimateFlowers()
    {
        foreach (Flower f in flowers)
        {
            f.age += Time.deltaTime;
            float t = Mathf.Clamp01(f.age / slideDuration);

            float eased = 1f - Mathf.Pow(1f - t, 3f);

            float x = Mathf.Lerp(f.startX, f.targetX, eased);
            f.transform.position = new Vector3(x, f.y, 0f);

            f.transform.Rotate(0f, 0f, f.spin * (1f - t) * Time.deltaTime);
        }
    }

    private void MoveToRandomPosition()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float x = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
        float y = Random.Range(-camHeight + edgePadding, camHeight - edgePadding);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
