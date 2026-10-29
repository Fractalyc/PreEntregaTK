using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MbappeSpecial : MonoBehaviour
{
    [SerializeField] private float timeLimit = 3f;
    [SerializeField] private float resultDuration = 2f;

    [SerializeField] private SpriteRenderer protagonist;
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite bowSprite;
    [SerializeField] private Sprite sadSprite;

    [SerializeField] private GameObject crowd;
    [SerializeField] private float crowdBobAmount = 0.1f;
    [SerializeField] private float crowdBobSpeed = 12f;

    [SerializeField] private Sprite[] flowerSprites;
    [SerializeField] private int flowerCount = 25;
    [SerializeField] private float flowerSpawnTime = 1f;
    [SerializeField] private Vector2 flowerFallSpeedRange = new Vector2(3f, 6f);
    [SerializeField] private Vector2 flowerScaleRange = new Vector2(0.12f, 0.25f);
    [SerializeField] private int flowerSortingOrder = 20;


    private class Flower
    {
        public Transform transform;
        public float speed;
        public float spin;
    }

    private readonly List<Flower> flowers = new List<Flower>();
    private Camera mainCamera;
    private Vector3 crowdStartPos;
    private float timer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        timer = timeLimit;

        if (protagonist != null) protagonist.sprite = neutralSprite;

        if (crowd != null)
        {
            crowdStartPos = crowd.transform.position;
            crowd.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver)
        {
            AnimateResult();
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");

        if (won)
        {
            if (protagonist != null) protagonist.sprite = bowSprite;
            StartCoroutine(SpawnFlowers());
        }
        else
        {
            if (protagonist != null) protagonist.sprite = sadSprite;
            if (crowd != null) crowd.SetActive(true);
        }

        StartCoroutine(ReturnToManager(won));
    }

    private IEnumerator ReturnToManager(bool won)
    {

        yield return new WaitForSeconds(resultDuration);
        GameManager.ReportResult(won);
    }



    private void AnimateResult()
    {
        MoveFlowers();

        if (crowd != null && crowd.activeSelf)
        {
            float offset = Mathf.Sin(Time.time * crowdBobSpeed) * crowdBobAmount;
            crowd.transform.position = crowdStartPos + Vector3.up * offset;
        }
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

        GameObject go = new GameObject("Flower");
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(Random.Range(-camWidth, camWidth), camHeight + 1f, 0f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        go.transform.localScale = Vector3.one * Random.Range(flowerScaleRange.x, flowerScaleRange.y);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = flowerSprites[Random.Range(0, flowerSprites.Length)];
        sr.sortingOrder = flowerSortingOrder;

        flowers.Add(new Flower
        {
            transform = go.transform,
            speed = Random.Range(flowerFallSpeedRange.x, flowerFallSpeedRange.y),
            spin = Random.Range(-90f, 90f)
        });
    }

    private void MoveFlowers()
    {
        float camHeight = mainCamera.orthographicSize;

        for (int i = flowers.Count - 1; i >= 0; i--)
        {
            Flower f = flowers[i];
            f.transform.position += Vector3.down * f.speed * Time.deltaTime;
            f.transform.Rotate(0f, 0f, f.spin * Time.deltaTime);

            if (f.transform.position.y < -camHeight - 1f)
            {
                Destroy(f.transform.gameObject);
                flowers.RemoveAt(i);
            }
        }
    }
}
