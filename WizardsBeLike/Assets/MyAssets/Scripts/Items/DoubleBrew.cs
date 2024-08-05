using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Double Brew item which allows casting two spells in one turn
[CreateAssetMenu(menuName = "Items/Double Brew")]
public class DoubleBrew : Item
{
    public override void Use(Player player)
    {
        player.spellCasting.CastSpell(); // Cast the first spell
        player.spellCasting.CastSpell(); // Cast the second spell
        // Note: You will need to make sure that CastSpell can be called twice like this
        // and that it only works for Type I and Type II spells.
    }
}
