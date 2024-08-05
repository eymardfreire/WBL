using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollision : MonoBehaviour
{
     // This function is called when another collider makes contact with this object's collider (2D physics only)
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision object has a Fireball component
        var fireball = collision.collider.GetComponent<Fireball>();
        if (fireball != null)
        {
            // Optionally, you can handle fireball-specific logic here, such as creating impact effects

            // If the fireball should be destroyed on impact, destroy it
            Destroy(collision.gameObject);
        }
    }

    // Optionally, you could also handle triggers if your spells use trigger colliders
    void OnTriggerEnter(Collider other)
    {
        // Similar check as above
        var fireball = other.GetComponent<Fireball>();
        if (fireball != null)
        {
            // Handle trigger-based logic here
        }
    }
}
