using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [Header("Attack Settings")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseKnockback = 5f;
    [SerializeField] private float attackInterval = 5f;
    [Header("Player Settings")]
    [SerializeField] private float baseHealth = 100f;

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

        if (baseHealth <= 0)
        {
            Debug.Log("Player is dead");
            // Handle player death here
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public float GetDamage()
    {
        return baseDamage; // Replace with calculation adding bonus damage or other calclulations from item/powerups
    }

    public float GetAttackInterval()
    {
        return attackInterval; // Same as above
    }

    public float GetKnockback()
    {
        return baseKnockback; // Same as above
    }

    public void DealDamageToPlayer(float damage)
    {
        baseHealth -= damage;
    }
}
