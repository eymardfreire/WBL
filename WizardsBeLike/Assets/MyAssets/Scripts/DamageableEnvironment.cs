using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class DamageableEnvironment : MonoBehaviour
{
    public float health = 100f;
    public GameObject[] piecePrefabs; // Assign all the piece prefabs in the inspector
    public float explosionForce = 1000f; // Adjust the force as needed
    public float explosionRadius = 5f; // Adjust the radius as needed
    public Vector3 explosionOffset = new Vector3(0, 1, 0); // Adjust the offset as needed

    void Start()
    {
       
    }

    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die(); // Call a separate method to handle the object's destruction
        }
        
    }

    private void Die()
    {
        // Instantiate all pieces and apply an explosion force
        foreach (GameObject piecePrefab in piecePrefabs)
        {
            GameObject piece = Instantiate(piecePrefab, transform.position, transform.rotation);
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position + explosionOffset, explosionRadius);
            }
        }

        // Optionally, play an explosion sound or particle effect here

        // Destroy the Dummy object
        Destroy(gameObject);
    }
}

