using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value;

    public GameObject coinEffect;

    public int soundToPlay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddCoins(value);
            Instantiate(coinEffect, transform.position, transform.rotation);
            Destroy(gameObject);
            AudioManager.instance.PlaySFX(soundToPlay);
        }
    }
}
