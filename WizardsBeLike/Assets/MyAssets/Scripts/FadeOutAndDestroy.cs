using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAndDestroy : MonoBehaviour
{
    public float lifetime = 2f; // Time in seconds after which the object will be destroyed
    public float fadeDuration = 1f; // Duration of the fade out effect

    void Start()
    {
        // Start the FadeOut coroutine
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Wait for the specified lifetime before starting the fade out effect
        yield return new WaitForSeconds(lifetime - fadeDuration);

        float elapsedTime = 0f;
        Renderer renderer = GetComponent<Renderer>();
        Color startColor = renderer.material.color;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the new color
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            renderer.material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Once the object is fully transparent, destroy it
        Destroy(gameObject);
    }
}
