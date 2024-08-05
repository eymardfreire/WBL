using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for spells; it does not need to inherit from MonoBehaviour
public abstract class Spell : ScriptableObject // Inherits from ScriptableObject
{
    public string spellName;
    public GameObject effectPrefab;

    // Abstract method to cast the spell
    public abstract void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower);

}
