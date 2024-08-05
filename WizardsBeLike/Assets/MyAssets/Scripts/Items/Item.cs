using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName; // Name of the item
    //public Sprite icon; // The UI icon for the item

    public GameObject effectPrefab; // The particle effect prefab
    public float effectDuration = 2.0f; // Duration to wait before destroying the effect


    // Abstract method to use the item
    public abstract void Use(Player player);
}
