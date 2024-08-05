using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class Damageable : MonoBehaviour
{
    public float health = 100f;
    public Slider healthBarSlider;
    public GameObject[] piecePrefabs;
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;
    public Vector3 explosionOffset = new Vector3(0, 1, 0);

    public AudioClip[] gruntSounds; // Array to hold grunt sounds
    private AudioSource audioSource; // AudioSource to play the grunt sounds

    void Start()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = health;
            healthBarSlider.value = health;
        }
        audioSource = gameObject.AddComponent<AudioSource>(); // Initialize the AudioSource
    }

    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        SendMessage("TriggerHitAnimation", SendMessageOptions.DontRequireReceiver);

        FloatingDamageDisplay floatingDamageDisplay = GetComponent<FloatingDamageDisplay>();
        if (floatingDamageDisplay != null)
        {
            floatingDamageDisplay.ShowDamage(damageAmount, transform.position);
        }

        // Play a random grunt sound
        if (gruntSounds.Length > 0)
        {
            AudioClip gruntSound = gruntSounds[Random.Range(0, gruntSounds.Length)];
            audioSource.PlayOneShot(gruntSound);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

        foreach (GameObject piecePrefab in piecePrefabs)
        {
            GameObject piece = Instantiate(piecePrefab, transform.position, transform.rotation);
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position + explosionOffset, explosionRadius);
            }
        }

        Player playerComponent = GetComponent<Player>();
        if (playerComponent != null)
        {
            GameManager.Instance.NotifyPlayerDeath(playerComponent);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Damageable: Player component not found on this GameObject.");
        }
    }

    public void ApplyHeal(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0f, healthBarSlider.maxValue); // Ensure health doesn't exceed max value
        healthBarSlider.value = health; // Update the health bar
    }
}
