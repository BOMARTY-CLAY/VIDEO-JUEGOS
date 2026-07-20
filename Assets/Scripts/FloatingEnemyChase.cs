using UnityEngine;

public class FloatingEnemyChase : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] patrolPoints;

    public float patrolSpeed = 3f;

    [Header("Chase")]
    public float chaseSpeed = 6f;

    public float chaseDuration = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    public float reachDistance = 0.2f;

    private int currentPoint = 0;

    private Transform player;

    private bool isChasing = false;

    private float chaseTimer;

    //Nuevo
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        //nuevo
        startPosition = transform.position;
        startRotation = transform.rotation;

        //Desparentar puntos.
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPoints[i].parent = null;
        }
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            ChasePlayer();

            chaseTimer -= Time.deltaTime;

            if (chaseTimer <= 0)
            {
                isChasing = false;
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPoint];

        MoveTo(
            targetPoint.position,
            patrolSpeed
        );

        float distance =
            Vector3.Distance(
                transform.position,
                targetPoint.position
            );

        if (distance < reachDistance)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }
        }
    }

    void ChasePlayer()
    {
        MoveTo(player.position, chaseSpeed);
    }

    void MoveTo(Vector3 targetPosition, float speed)
    {
        Vector3 direction =
            (targetPosition - transform.position).normalized;

        transform.position +=
            direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isChasing)
        {
            player = other.transform;

            isChasing = true;

            chaseTimer = chaseDuration;
        }
    }
    //Nuevo
    public void ResetEnemy()
    {
        isChasing = false;

        player = null;

        chaseTimer = 0f;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}