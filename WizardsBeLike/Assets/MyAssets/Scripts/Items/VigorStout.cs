using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Vigor Stout item which recovers health by 50%
[CreateAssetMenu(menuName = "Items/Vigor Stout")]
public class VigorStout : Item
{
    public override void Use(Player player)
    {
        Damageable damageableComponent = player.GetComponent<Damageable>();
        if (damageableComponent != null)
        {
            float healAmount = damageableComponent.healthBarSlider.maxValue * 0.5f; // 50% of max health
            damageableComponent.ApplyHeal(healAmount);
            // Note: You'll need to implement ApplyHeal method in the Damageable class.
        }

        if (effectPrefab != null)
        {
            GameObject effectInstance = Instantiate(effectPrefab, player.effectPosition.position, Quaternion.identity);
            Object.Destroy(effectInstance, effectDuration); // Ensure the effect prefab is destroyed after duration
        }
    }
}
