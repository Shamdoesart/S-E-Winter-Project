

using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.5f;
    public float rotationSpeed = 10f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Vector2 moveInput;
    [SerializeField]private bool isJumping;

    [Header("Camera Settings")]
    public Transform cam;

    private PlayerInput inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize the input actions
        inputActions = new PlayerInput();

        // Hook up input events
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => isJumping = true;
    }

    private void OnEnable()
    {
        // Enable the input actions when the object is enabled
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions when the object is disabled
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        Move();
        if (isJumping)
        {
            Jump();
            isJumping = false;
            
        }
    }

    private void Move()
    {

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        if(moveDirection.magnitude>= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            Vector3 velocity = moveDir.normalized * moveSpeed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;

        }

        
     
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer));
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }



}
