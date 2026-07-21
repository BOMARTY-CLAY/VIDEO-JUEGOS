using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int healAmount;
    public bool isFullHeal;

    public GameObject healEffect;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isFullHeal)
            {
                HealthManager.instance.ResetHealth();
            }else{
                HealthManager.instance.AddHealth(healAmount);
            }

            Destroy(gameObject);

            Instantiate(healEffect, PlayerController.instance.transform.position + new Vector3(0f, 1f, 0f), PlayerController.instance.transform.rotation);

        }
    }
}
