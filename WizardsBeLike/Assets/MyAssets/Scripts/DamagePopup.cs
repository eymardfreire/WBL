using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
   [SerializeField] private TextMeshPro textMesh;
    private float disappearTimer = 1f; // Duration before the text disappears
    private Color textColor;
    private Vector3 moveVector;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public static DamagePopup Create(Vector3 position, int damageAmount, Transform uiCanvasTransform)
{
    // Ensure the path is correct, it should be just the name of the prefab if it's directly in the Resources folder
    var damagePopupPrefab = Resources.Load<DamagePopup>("DamagePopupPrefab");
    if (damagePopupPrefab == null) {
        Debug.LogError("DamagePopupPrefab could not be loaded. Check if the prefab exists in the Resources folder and the name is correct.");
        return null;
    }

    DamagePopup damagePopupInstance = Instantiate(damagePopupPrefab, position, Quaternion.identity);
    damagePopupInstance.Setup(damageAmount);

    if (uiCanvasTransform != null) {
        // Set the instantiated prefab as a child of the Canvas
        damagePopupInstance.transform.SetParent(uiCanvasTransform, false);
        damagePopupInstance.transform.localPosition = position; // Adjust as necessary for your UI layout
    }

    return damagePopupInstance;
}

    public void Setup(int damageAmount)
    {
        textMesh.text = damageAmount.ToString();
        textColor = textMesh.color;
        moveVector = new Vector3(0, 1) * 2f; // Move up by 2 units per second
        StartCoroutine(FadeAway());
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeSpeed = 3f; // Adjust fade speed as needed
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(disappearTimer);
        // Start fading
        while (textColor.a > 0)
        {
            textColor.a -= Time.deltaTime;
            textMesh.color = textColor;
            yield return null;
        }
        Destroy(gameObject);
    }

    
}
