using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    public float fallDelay = 1f;       // Tiempo que espera antes de caer
    public float respawnDelay = 3f;    // Tiempo antes de volver a la posición original
    public float fallSpeed = 17f;      // Velocidad de caída (el video recomienda entre 10 y 20)

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    // Variables de control (evitan activaciones múltiples)
    private bool hasFallen = false;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Guardamos la posición y rotación originales para el Reset
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        // Controlamos cuándo aplicar la velocidad de caída de forma manual.
        // Esto evita los errores comunes del Rigidbody cinemático al transicionar.
        if (isFalling)
        {
            rb.linearVelocity = new Vector3(0, -fallSpeed, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Identificamos cuando el player salta sobre la plataforma
        if (other.CompareTag("Player") && !hasFallen)
        {
            hasFallen = true;
            StartCoroutine(FallAndRespawnRoutine());
        }
    }

    private IEnumerator FallAndRespawnRoutine()
    {
        // 1. Esperamos antes de empezar a caer
        yield return new WaitForSeconds(fallDelay);

        // 2. Aplicamos la caída
        isFalling = true;
        rb.isKinematic = false; // Permitimos que el motor físico actúe

        // 3. Dejamos que caiga durante 2 segundos antes de "desaparecerla"
        yield return new WaitForSeconds(2f);
        
        // Detenemos el movimiento de la plataforma temporalmente
        isFalling = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        
        // Hacemos que la plataforma sea invisible e intocable mientras reaparece
        CambiarEstadoPlataforma(false);

        // 4. Esperamos el tiempo de reaparición
        yield return new WaitForSeconds(respawnDelay);

        // 5. Restablecemos la plataforma
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        // La devolvemos a su posición original
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // La hacemos visible y sólida de nuevo
        CambiarEstadoPlataforma(true);

        // Reiniciamos los estados
        hasFallen = false;
        isFalling = false;
        rb.isKinematic = true;
    }

    // Función auxiliar para activar/desactivar todos los colliders y la malla (Mesh)
    private void CambiarEstadoPlataforma(bool estado)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = estado;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = estado;
        }
    }
}
