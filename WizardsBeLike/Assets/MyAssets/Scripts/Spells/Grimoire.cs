using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Grimoire class that will hold spells and will be a ScriptableObject
[CreateAssetMenu(fileName = "New Grimoire", menuName = "Grimoire")]
public class Grimoire : ScriptableObject
{
    public string grimoireName;
    public ParticleSystem chargingEffectPrefab; // This is for the charging effect
    public ParticleSystem castingEffectPrefab; // Casting effect
    public List<ScriptableObject> spellObjects; // Use ScriptableObject here

    // Runtime list that is not displayed in the inspector but used in the game
    [HideInInspector] public List<Spell> spells = new List<Spell>();

    private void OnEnable()
    {
        // Clear the runtime spell list
        spells.Clear();

        // Go through all the scriptable objects and add them to the runtime spell list if they are Spells
        foreach (ScriptableObject spellObject in spellObjects)
        {
            Spell spell = spellObject as Spell; // Use 'as' for safe casting
            if (spell != null)
            {
                spells.Add(spell);
            }
    }
    }
}



