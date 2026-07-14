using UnityEngine;
using System.Collections;

public class GhostPlatform : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    public float visibleTime = 2f;    // Cuánto tiempo se puede pisar y ver
    public float invisibleTime = 2f;  // Cuánto tiempo desaparece

    void Start()
    {
        // Iniciamos el ciclo infinito de aparecer/desaparecer desde que empieza el juego
        StartCoroutine(GhostCycleRoutine());
    }

    private IEnumerator GhostCycleRoutine()
    {
        // Este bucle 'while (true)' hace que la rutina se repita infinitamente
        while (true)
        {
            // 1. ESTADO VISIBLE Y SÓLIDO
            CambiarEstadoPlataforma(true);
            yield return new WaitForSeconds(visibleTime);

            // 2. ESTADO INVISIBLE E INTOCABLE
            CambiarEstadoPlataforma(false);
            yield return new WaitForSeconds(invisibleTime);
        }
    }

    // Función auxiliar mejorada para apagar/prender mallas y colisiones en padres e hijos
    private void CambiarEstadoPlataforma(bool estado)
    {
        // Busca TODOS los MeshRenderers en este objeto y en sus hijos y los apaga/prende
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = estado;
        }

        // Hace lo mismo con todos los Colliders para que no puedas chocar con ella cuando está invisible
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = estado;
        }
    }
}