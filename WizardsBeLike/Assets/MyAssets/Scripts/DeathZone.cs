using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public float damageAmount = 100f; // Set this to the player's max health

    private void OnTriggerEnter(Collider other)
    {
        // First, handle damageable entities
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(damageAmount);
            return; // Early exit to prevent destruction
        }

        // If it's not damageable, check if it should be destroyed
        if (other.CompareTag("DestroyOnFall"))
        {
            Destroy(other.gameObject);
        }
    }
}

