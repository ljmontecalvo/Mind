using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputAction movement;
    private Rigidbody2D rb;

    public float speed = 5f;
    public Color offColor;
    public Color onColor;

    private Vector2 moveDirection;


    private void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 movementValue = movement.ReadValue<Vector2>();
        moveDirection = new Vector2(movementValue.x, movementValue.y).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}
