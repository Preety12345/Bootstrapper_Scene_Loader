using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    [SerializeField]private float force = 5f;
    private Rigidbody rb;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new();
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += Jump;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnDestroy()
    {
        inputActions.Player.Disable();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    private void Movement()
    {
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(inputVector.x, 0, inputVector.y);
    }
}
