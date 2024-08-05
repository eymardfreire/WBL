using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 2f; // Time in seconds after which the object will be destroyed

    void Start()
    {
        // Destroy this game object after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}
