using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;

    private Vector2 movement;
    private CharacterController controller;
    private InputSystem_Actions input;
    private Transform swordTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new();
        swordTransform = transform.Find("SwordParent");
    }

    private void OnEnable()
    {
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Enable();
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Disable();
    }

    private void Update()
    {
        Vector3 moveDirection = new(movement.x, 0, movement.y);
        controller.Move(speed * Time.deltaTime * moveDirection);
        swordTransform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
}
