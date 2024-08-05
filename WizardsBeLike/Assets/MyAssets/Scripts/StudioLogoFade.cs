using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StudioLogoFade : MonoBehaviour
{
    public CanvasGroup fadePanelCanvasGroup;
    public float fadeDuration = 2.0f;
    public float displayDuration = 2.0f;
    public string titleSceneName = "TitleLogo";

    private float fadeTimer;
    private bool isFadingIn = true;

    void Start()
    {
        // Start with a full alpha value (full black screen)
        fadePanelCanvasGroup.alpha = 1f;
        // Begin fading to clear
        StartCoroutine(FadeToClear());
    }

    IEnumerator FadeToClear()
    {
        // Lerp the alpha value of the panel to 0 over the fade duration
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            fadePanelCanvasGroup.alpha = alpha;
            yield return null;
        }
        // Ensure it's fully transparent after fading
        fadePanelCanvasGroup.alpha = 0f;

        // Wait for display duration with the logo fully visible
        yield return new WaitForSeconds(displayDuration);

        // Start fading to black before changing scene
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        fadeTimer = 0f; // Reset timer for the fade to black

        // Lerp the alpha value of the panel to 1 over the fade duration
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            fadePanelCanvasGroup.alpha = alpha;
            yield return null;
        }
        // Ensure it's fully opaque after fading
        fadePanelCanvasGroup.alpha = 1f;

        // Change to the Title scene
        SceneManager.LoadScene(titleSceneName);
    }
}
