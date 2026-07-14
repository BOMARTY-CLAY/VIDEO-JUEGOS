using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el modelo 3D de los pinchos (el objeto hijo)")]
    public Transform spikesTransform; 

    [Header("Configuración de Altura (Eje Y Local)")]
    public float hiddenY = -2f;   // Altura cuando están escondidos
    public float visibleY = 0f;   // Altura cuando salen para matar

    [Header("Tiempos y Velocidad")]
    public float speed = 15f;         // Velocidad a la que salen los pinchos
    public float visibleTime = 2f;    // Cuánto tiempo se quedan arriba

    private bool isTriggered = false;

    void Start()
    {
        // Forzamos a que los pinchos empiecen ocultos al iniciar el nivel
        if (spikesTransform != null)
        {
            Vector3 startPos = spikesTransform.localPosition;
            startPos.y = hiddenY;
            spikesTransform.localPosition = startPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos que sea el player y que los pinchos no estén ya activados
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(SpikeRoutine());
        }
    }

    private IEnumerator SpikeRoutine()
    {
        if (spikesTransform == null) yield break;

        // 1. SUBIR LOS PINCHOS (Salen de golpe)
        Vector3 targetVisiblePos = new Vector3(spikesTransform.localPosition.x, visibleY, spikesTransform.localPosition.z);
        
        // Mientras no lleguen a la posición objetivo, los movemos fotograma a fotograma
        while (Vector3.Distance(spikesTransform.localPosition, targetVisiblePos) > 0.01f)
        {
            spikesTransform.localPosition = Vector3.MoveTowards(spikesTransform.localPosition, targetVisiblePos, speed * Time.deltaTime);
            yield return null; 
        }
        spikesTransform.localPosition = targetVisiblePos; // Asegurar posición exacta

        // 2. ESPERAR ARRIBA A LA VISTA DEL PLAYER
        yield return new WaitForSeconds(visibleTime);

        // 3. BAJAR LOS PINCHOS (Se vuelven a ocultar)
        Vector3 targetHiddenPos = new Vector3(spikesTransform.localPosition.x, hiddenY, spikesTransform.localPosition.z);

        while (Vector3.Distance(spikesTransform.localPosition, targetHiddenPos) > 0.01f)
        {
            spikesTransform.localPosition = Vector3.MoveTowards(spikesTransform.localPosition, targetHiddenPos, speed * Time.deltaTime);
            yield return null;
        }
        spikesTransform.localPosition = targetHiddenPos;

        // 4. RESETEAR PARA QUE PUEDAN VOLVER A ACTIVARSE
        isTriggered = false;
    }
}