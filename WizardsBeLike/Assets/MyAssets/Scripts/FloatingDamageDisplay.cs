using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingDamageDisplay : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Canvas activeCanvas; // Assign this in the inspector
    public Vector2 offset; // Offset for the UI position
    public Player player;

    public void ShowDamage(float damageAmount, Vector3 position)
    {
        // Instantiate the damage text prefab as a child of the Dummy's Canvas
        GameObject damageText = Instantiate(damageTextPrefab, activeCanvas.transform, false);

        // Convert the world position of the impact to screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        
        // Convert screen position to UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(activeCanvas.transform as RectTransform, screenPosition, activeCanvas.worldCamera, out Vector2 uiPosition);
        damageText.transform.localPosition = uiPosition + offset; // Apply the offset here

        // Get the TextMeshProUGUI component and set the text to the rounded damage amount
        TextMeshProUGUI text = damageText.GetComponent<TextMeshProUGUI>();
        int damageRounded = Mathf.RoundToInt(damageAmount); // Round to nearest integer
        text.text = damageRounded.ToString(); // Set text to rounded damage

        // Make the text float upwards and destroy it after a certain amount of time
        StartCoroutine(MakeTextFloatAndDestroy(damageText));
        player.RegisterDamageText(damageText);

    }

    public void MakeTextDisappear()
    {
        // This function now simply starts the coroutine for each damage text instance in the scene
        foreach (var damageText in FindObjectsOfType<FloatingDamageDisplay>())
        {
            StartCoroutine(damageText.MakeTextFloatAndDestroy(damageText.gameObject));
        }
    }

    // Existing method with parameter, can be private if only used internally
    private IEnumerator MakeTextFloatAndDestroy(GameObject damageText)
    {
        float speed = 1.0f;
        float destroyTime = 2.0f;
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        while (destroyTime > 0)
        {
            rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            destroyTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(damageText);
    }

    public void HideDamage()
    {
        Destroy(gameObject);
    }
}