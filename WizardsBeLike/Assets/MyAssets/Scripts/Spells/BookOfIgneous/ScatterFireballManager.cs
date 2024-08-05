using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterFireballManager : MonoBehaviour
{
    public static ScatterFireballManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartScatterFireballSequence(Spell spell, Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        StartCoroutine(ScatterFireballSequence(spell, castingPoint, playerMovement, magicalWeapon, currentCastingPower));
    }

    private IEnumerator ScatterFireballSequence(Spell spell, Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        for (int i = 0; i < 3; i++)
        {
            // Your instantiation logic here, similar to the original Cast method but adapted for sequential instantiation

            yield return new WaitForSeconds(0.1f); // Adjust delay as needed
        }
    }
}

