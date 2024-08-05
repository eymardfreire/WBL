using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance { get; private set; } // Singleton instance
     public Vector2 windDirection;
    public float windStrength;
    public float maxWindStrength = 50f; // Maximum wind strength
    public TMP_Text windForceText; // Assign in the inspector
    public Image windArrowImage; // Assign in the inspector
    public AudioClip windChangeSound; // Assign this in the inspector with your wind change sound effect
    private AudioSource audioSource; // AudioSource component to play the sound


    void Awake()
    {
        // If there is an instance, and it's not me, destroy myself.
        if (Instance != null && Instance != this)
        {   
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionally keep the wind manager across scenes
        }

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            // If AudioSource component wasn't found, add it dynamically
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
            //RandomizeWind();
            // Now passing the wind strength and the calculated angle to the UpdateWindUI method
            //UpdateWindUI(windStrength, Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg);
        //}
    }

    public void RandomizeWind()
    {
        // Randomize wind strength between 0 and maxWindStrength
        windStrength = Random.Range(0, maxWindStrength);

        // Randomize wind direction in degrees (0 to 360)
        float windAngle = Random.Range(0f, 360f);
        windDirection = new Vector2(Mathf.Cos(windAngle * Mathf.Deg2Rad), Mathf.Sin(windAngle * Mathf.Deg2Rad));

        // Update the UI to reflect these new wind values
        UpdateWindUI(windStrength, Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg);

        // Play wind change sound
        PlayWindChangeSound();
    }


    private void PlayWindChangeSound()
    {
        if (windChangeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(windChangeSound); // Play the wind change sound effect
        }
    }

    private void UpdateWindUI(float strength, float angle)
    {
        // Update the wind force text
        if (windForceText != null)
        {
            windForceText.text = strength.ToString("F0"); // "F0" for no decimal places
        }

        // Rotate the wind arrow image to match the wind direction
        if (windArrowImage != null)
        {
            // Adjust the angle so that it represents the wind direction correctly
            // The arrow will point in the direction where the wind is blowing to
            float angleToUse = angle - 90f; // Subtract 90 because UI rotation is different from world rotation
            windArrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, angleToUse);
        }
    }
}