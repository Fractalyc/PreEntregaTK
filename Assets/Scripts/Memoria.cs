using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Memoria : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] squares;
    [SerializeField] private int sequenceLength;

    [Header("Colores")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;

    [Header("Tiempos")]
    [SerializeField] private float startDelay;
    [SerializeField] private float highlightDuration;
    [SerializeField] private float gapBetweenHighlights;
    [SerializeField] private float inputTimeLimit;

    private const float clickFlashDuration = 0.15f;

    private Camera mainCamera;
    private Collider2D[] colliders;
    private readonly List<int> sequence = new List<int>();
    private int currentStep;
    private float timer;
    private bool acceptingInput = false;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;

        colliders = new Collider2D[squares.Length];
        for (int i = 0; i < squares.Length; i++)
        {
            colliders[i] = squares[i].GetComponent<Collider2D>();
        }
    }

    private void Start()
    {
        foreach (SpriteRenderer sr in squares)
        {
            sr.color = normalColor;
        }

        GenerateSequence();
        StartCoroutine(ShowSequence());
    }

    private void Update()
    {
        if (gameOver || !acceptingInput) return;

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

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].OverlapPoint(mouseWorldPos))
                {
                    HandleSquareClicked(i);
                    return;
                }
            }
        }
    }

    private void GenerateSequence()
    {
        sequence.Clear();
        for (int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(Random.Range(0, squares.Length));
        }
    }

    private IEnumerator ShowSequence()
    {
        yield return new WaitForSeconds(startDelay);

        foreach (int index in sequence)
        {
            squares[index].color = highlightColor;
            yield return new WaitForSeconds(highlightDuration);
            squares[index].color = normalColor;
            yield return new WaitForSeconds(gapBetweenHighlights);
        }

        timer = inputTimeLimit;
        acceptingInput = true;
    }

    private void HandleSquareClicked(int index)
    {
        StartCoroutine(FlashSquare(index));

        if (index != sequence[currentStep])
        {
            EndRound(false);
            return;
        }

        currentStep++;
        if (currentStep >= sequence.Count)
        {
            EndRound(true);
        }
    }

    private IEnumerator FlashSquare(int index)
    {
        squares[index].color = highlightColor;
        yield return new WaitForSeconds(clickFlashDuration);
        squares[index].color = normalColor;
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        acceptingInput = false;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }
}
