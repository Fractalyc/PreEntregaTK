using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoomCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D bounds;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private float playerNudge = 1f;

    private Camera cam;
    private PlayerController playerController;
    private Rigidbody2D playerBody;

    private Vector2 origin;
    private float roomWidth;
    private float roomHeight;
    private Vector2Int currentRoom;
    private bool transitioning = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        playerController = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
        roomHeight = cam.orthographicSize * 2f;
        roomWidth = roomHeight * cam.aspect;
        origin = transform.position;

        
        currentRoom = GetRoomAt(player.position);
        transform.position = GetRoomCenter(currentRoom);
    }

    private void Update()
    {
        if (transitioning) return;

        Vector2Int room = GetRoomAt(player.position);
        if (room != currentRoom)
        {
            StartCoroutine(Transition(room));
        }
    }

    private Vector2Int GetRoomAt(Vector2 position)
    {
        int x = Mathf.RoundToInt((position.x - origin.x) / roomWidth);
        int y = Mathf.RoundToInt((position.y - origin.y) / roomHeight);
        return new Vector2Int(x, y);
    }

    private Vector3 GetRoomCenter(Vector2Int room)
    {
        Vector3 center = new Vector3(
            origin.x + room.x * roomWidth,
            origin.y + room.y * roomHeight,
            transform.position.z);
        return ClampToBounds(center);
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        if (bounds == null) return pos;

        Bounds b = bounds.bounds;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = b.min.x + halfW;
        float maxX = b.max.x - halfW;
        float minY = b.min.y + halfH;
        float maxY = b.max.y - halfH;

        // If the bounds are smaller than the view on an axis, center on that axis
        pos.x = minX <= maxX ? Mathf.Clamp(pos.x, minX, maxX) : b.center.x;
        pos.y = minY <= maxY ? Mathf.Clamp(pos.y, minY, maxY) : b.center.y;
        return pos;
    }

    private IEnumerator Transition(Vector2Int newRoom)
    {
        transitioning = true;

        
        playerController.enabled = false;

        Vector2 direction = new Vector2(
            Mathf.Clamp(newRoom.x - currentRoom.x, -1, 1),
            Mathf.Clamp(newRoom.y - currentRoom.y, -1, 1));

        Vector3 camStart = transform.position;
        Vector3 camEnd = GetRoomCenter(newRoom);
        Vector2 playerStart = player.position;
        Vector2 playerEnd = playerStart + direction * playerNudge;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            transform.position = Vector3.Lerp(camStart, camEnd, t);
            MovePlayer(Vector2.Lerp(playerStart, playerEnd, t));

            yield return null;
        }

        transform.position = camEnd;
        currentRoom = newRoom;

        playerController.enabled = true;
        transitioning = false;
    }

    private void MovePlayer(Vector2 position)
    {
        if (playerBody != null)
        {
            playerBody.position = position;
        }
        else
        {
            player.position = new Vector3(position.x, position.y, player.position.z);
        }
    }
}