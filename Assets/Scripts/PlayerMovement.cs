using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float speed;
    public float baseSpeed;
    public float addedSpeed;
    private float sprintSpeed;
    private Vector2 direction;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = baseSpeed;
    }
    void FixedUpdate()
    {
        // Kinematic bodies use MovePosition to smoothly handle physics and walls
        Vector2 targetPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        sprintSpeed = baseSpeed + addedSpeed;
        if (context.started)
        {
            speed = sprintSpeed;
        }
        if (context.canceled)
        {
            speed = baseSpeed;
        }
    }
}
