using UnityEngine;
using UnityEngine.InputSystem;
public class CatchObject : MonoBehaviour
{
    [SerializeField] private Collider2D catcher;
    [SerializeField] private Collider2D fallingObject;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float startDelay;
    [SerializeField] private float edgePadding;
    [SerializeField] private float spawnMargin;
    [SerializeField] private float missMargin;

    private Camera mainCamera;
    private float startTimer;
    private bool gameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        startTimer = startDelay;
        PlaceFallingObject();
    }

    private void Update()
    {
        if (gameOver) return;

        FollowCursor();

        if (startTimer > 0f)
        {
            startTimer -= Time.deltaTime;
            return;
        }

        fallingObject.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        CheckResult();
    }

    private void FollowCursor()
    {
        if (Mouse.current == null) return;

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float x = Mathf.Clamp(mouseWorldPos.x, -camWidth + edgePadding, camWidth - edgePadding);

        Vector3 pos = catcher.transform.position;
        catcher.transform.position = new Vector3(x, pos.y, pos.z);
    }

    private void PlaceFallingObject()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float x = Random.Range(-camWidth + edgePadding, camWidth - edgePadding);
        float y = camHeight + spawnMargin;

        fallingObject.transform.position = new Vector3(x, y, fallingObject.transform.position.z);
    }

    private void CheckResult()
    {
        Physics2D.SyncTransforms();

        if (catcher.Distance(fallingObject).isOverlapped)
        {
            EndRound(true);
            return;
        }

        if (fallingObject.transform.position.y < catcher.transform.position.y - missMargin)
        {
            EndRound(false);
        }
    }

    private void EndRound(bool won)
    {
        gameOver = true;
        Debug.Log(won ? "¡GANASTE!" : "PERDISTE.");
        GameManager.ReportResult(won);
    }
}
