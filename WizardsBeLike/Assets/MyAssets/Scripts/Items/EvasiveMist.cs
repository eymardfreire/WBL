using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Evasive Mist")]
public class EvasiveMist : Item
{
    public override void Use(Player player)
    {
        GameManager.Instance.NotifyPlayerEvasion(player);
        if (effectPrefab != null)
        {
            GameObject effectInstance = Instantiate(effectPrefab, player.effectPosition.position, Quaternion.identity);
            Object.Destroy(effectInstance, effectDuration); // Ensure the effect prefab is destroyed after duration
        }

        // End the turn after using the item
        GameManager.Instance.EndTurn();
    }
}
