using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{
    [Header("Puntos de Patrullaje")]
    public Transform[] patrolPoints;

    [Header("Configuración de Movimiento")]
    public float moveSpeed = 3f;      // Velocidad a la que avanza
    public float rotSpeed = 5f;       // Velocidad a la que gira (Slerp)
    public float minDistance = 0.2f;  // A qué distancia cuenta como "llegó al punto"

    private int currentPointIndex = 0;

    void Start()
    {
        // TRUCO: Desvinculamos los puntos del enemigo al iniciar para que sean fijos en el mapa
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
            {
                point.SetParent(null);
            }
        }
    }

    void Update()
    {
        // Si no hay puntos asignados, no hacemos nada
        if (patrolPoints.Length == 0) return;

        // Obtenemos el punto objetivo actual
        Transform targetPoint = patrolPoints[currentPointIndex];

        // 1. Calcular la dirección hacia el punto
        Vector3 direction = targetPoint.position - transform.position;

        // 2. Rotación suave hacia la dirección
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }

        // 3. Mover al enemigo hacia el punto
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // 4. Comprobar si ya estamos cerca del punto
        if (Vector3.Distance(transform.position, targetPoint.position) < minDistance)
        {
            currentPointIndex++;

            // Si llega al último punto, reinicia al primero
            if (currentPointIndex >= patrolPoints.Length)
            {
                currentPointIndex = 0;
            }
        }
    }

    // Lógica para matar al jugador al tocarlo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}