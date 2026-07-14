using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class playerController : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -20f;
    public float jumpHeight = 2f;
    public Transform spawnPoint;
    public float respawnHeightOffset = 2.0f;
    public float rotationSpeed = 10f;

    Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool jumpPressed;

    private Animator animator;
    private bool isDead = false;

    public Vector3 externalMoveSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        
        animator = GetComponentInChildren<Animator>(); 
    }

    void Update()
    {
        if (isDead) return;
        
        bool isGrounded = controller.isGrounded;
        float speedValue = moveInput.magnitude;

        animator.SetFloat("Speed", speedValue);
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (jumpPressed && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
            animator.SetTrigger("Jump");
        }

        verticalVelocity += gravity * Time.deltaTime;
        animator.SetFloat("VerticalVelocity", verticalVelocity);

        // --- MOVIMIENTO RELATIVO A LA CÁMARA ---
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // Rotación del Player
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // --- CORRECCIÓN DE MATEMÁTICAS AQUÍ ---
        // 1. Movimiento horizontal (teclas/mando)
        Vector3 horizontalMove = moveDirection.normalized * speed;
        
        // 2. Movimiento vertical (gravedad/salto)
        Vector3 verticalMove = Vector3.up * verticalVelocity;

        // 3. Sumamos todo limpiamente y multiplicamos por el tiempo de un solo golpe
        Vector3 finalMovement = horizontalMove + verticalMove + externalMoveSpeed;
        
        controller.Move(finalMovement * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }

    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
        StartCoroutine(RespawnCoutine());
    }

    private IEnumerator RespawnCoutine()
    {
        yield return new WaitForSeconds(2f);

        controller.enabled = false;
        Vector3 respawnPosition = spawnPoint.position + Vector3.up * respawnHeightOffset;
        transform.position = respawnPosition;
        verticalVelocity = 0f;
        controller.enabled = true;
        
        animator.ResetTrigger("Die"); 
        animator.Play("Locomotion");

        isDead = false;
    }
}