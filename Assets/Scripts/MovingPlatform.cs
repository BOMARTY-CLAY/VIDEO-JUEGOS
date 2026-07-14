using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3;

    private Vector3 currentTarget;
    private float proximityThreshold = 0.2f;

    // --- VARIABLES PARA EL NUEVO MÉTODO ---
    private CharacterController playerOnPlatform;
    private Vector3 lastPosition;

    void Start()
    {
        pointA.parent = null;
        pointB.parent = null;
        currentTarget = pointB.position;
        
        // Guardamos la posición inicial antes de empezar a movernos
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        // 1. Calculamos el objetivo de la plataforma
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);

        if(distanceToTarget < proximityThreshold)
        {
            currentTarget = (currentTarget == pointA.position) ? pointB.position : pointA.position;
        }

        Vector3 direction = (currentTarget - transform.position).normalized;
        
        // 2. Movemos la plataforma
        transform.position += direction * speed * Time.deltaTime;

        // 3. LA SOLUCIÓN: Calculamos la distancia exacta que se movió la plataforma en este fotograma (Delta)
        Vector3 platformMovement = transform.position - lastPosition;

        // Si el jugador está parado sobre la plataforma, lo obligamos a moverse esa misma distancia
        if (playerOnPlatform != null)
        {
            playerOnPlatform.Move(platformMovement);
        }

        // 4. Actualizamos la posición antigua para el siguiente fotograma
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // En lugar de pasar una velocidad, capturamos el componente del jugador directamente
            playerOnPlatform = other.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Cuando el jugador salta o sale del área, lo soltamos
            playerOnPlatform = null;
        }
    }
}