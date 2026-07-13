using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // <- CORRECCIÓN 1: Esto quita el error en rojo de tu imagen

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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        
        // Inicializa el Animator buscando en los objetos hijos
        animator = GetComponentInChildren<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; // Evita que el jugador se mueva si está muerto
        bool isGrounded = controller.isGrounded;
        
        // Calcula la magnitud del movimiento
        float speedValue = moveInput.magnitude;

        // Envía el valor al parámetro "Speed" del Animator Controller
        animator.SetFloat("Speed", speedValue);

        // Indicamos si es personaje está en el suelo o no, para controlar la animación de salto
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        //salto
        if (jumpPressed && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;

            // Activar la animación de salto
            animator.SetTrigger("Jump");
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Animación de salto: Calcula la velocidad vertical y la envía al Animator Controller
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

        // Movimiento original
        Vector3 move = moveDirection.normalized;
        move.y = verticalVelocity / speed;

        controller.Move(move * speed * Time.deltaTime);
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
        
        // CORRECCIÓN 2: Cambié "SetTrigger" por "ResetTrigger". 
        // Esto quita el "saltito" raro/bucle de muerte al reaparecer.
        animator.ResetTrigger("Die"); 
        animator.Play("Locomotion");

        isDead = false;

    }
}