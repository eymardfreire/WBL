using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cool Libation item which reduces Super Spell cooldown by 60 seconds
[CreateAssetMenu(menuName = "Items/Cool Libation")]
public class CoolLibation : Item
{
    public override void Use(Player player)
    {
        player.spellCasting.ReduceSuperSpellCooldown(60f); // Reduce cooldown by 60 seconds
        // Note: You'll need to implement ReduceSuperSpellCooldown method in the SpellCasting class.
    }
}
