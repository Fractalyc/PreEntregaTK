using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class MovingSquare : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private float edgePadding;

    [SerializeField] private float timeLimit;

    private Camera mainCamera;
    private Collider2D squareCollider;
    private Vector2 velocity;
    private float timer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        squareCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        timer = timeLimit;
        MoveToRandomStartPosition();
        PickRandomDirection();
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

        MoveAndBounce();
        HandleClickInput();
    }

    private void MoveAndBounce()
    {
        Vector3 pos = transform.position;
        pos += (Vector3)(velocity * Time.deltaTime);

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float minX = -camWidth + edgePadding;
        float maxX = camWidth - edgePadding;
        float minY = -camHeight + edgePadding;
        float maxY = camHeight - edgePadding;

        if (pos.x < minX)
        {
            pos.x = minX;
            velocity.x = Mathf.Abs(velocity.x);
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            velocity.x = -Mathf.Abs(velocity.x);
        }

        if (pos.y < minY)
        {
            pos.y = minY;
            velocity.y = Mathf.Abs(velocity.y);
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            velocity.y = -Mathf.Abs(velocity.y);
        }

        transform.position = pos;
    }

    private void HandleClickInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (squareCollider.OverlapPoint(mouseWorldPos))
            {
                EndRound(true);
            }
            else
            {
                EndRound(false);
            }
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log((won ? "¡GANASTE! " : "PERDISTE. "));
        GameManager.ReportResult(won);
    }

    private void MoveToRandomStartPosition()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float x = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
        float y = Random.Range(-camHeight + edgePadding, camHeight - edgePadding);

        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void PickRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        velocity = direction * moveSpeed;
    }
}