using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDownAndDestroy : MonoBehaviour
{
    public float lifetime = 2f; // Time in seconds before the object starts scaling down
    public float scaleDuration = 1f; // Duration of the scale down effect

    void Start()
    {
        // Start the ScaleDown coroutine
        StartCoroutine(ScaleDown());
    }

    IEnumerator ScaleDown()
    {
        // Wait for the specified lifetime before starting the scale down effect
        yield return new WaitForSeconds(lifetime);

        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            // Calculate the new scale as a factor of the original scale and time elapsed
            float scale = 1 - (elapsedTime / scaleDuration);
            transform.localScale = originalScale * scale;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Once the object is scaled down to zero, destroy it
        Destroy(gameObject);
    }
}
