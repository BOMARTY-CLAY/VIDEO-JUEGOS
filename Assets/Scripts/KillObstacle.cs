using UnityEngine;

public class KillObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();

        if (player != null)
        {
            player.Die();
        }
    }
}