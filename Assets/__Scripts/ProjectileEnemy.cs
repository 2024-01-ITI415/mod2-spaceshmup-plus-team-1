using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public float lifetime = 5f; // Time in seconds before the projectile is automatically destroyed

    void Start()
    {
        // Destroy the projectile after a set amount of time
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit the player's shield
        if (other.CompareTag("Hero"))
        {
            // Reduce the player's shield level
            Hero.S.shieldLevel--;

            // Optionally, you can add some visual or audio feedback for the hit
        }

        // Destroy the projectile after it hits something
        Destroy(gameObject);
    }
}