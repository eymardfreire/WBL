using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpellCaster : MonoBehaviour
{
    public Transform playerTarget; // Assign your player's transform in the inspector
    public Grimoire[] grimoires; // Assign the grimoires in the inspector
    private int currentGrimoireIndex = 0;
    private int currentSpellIndex = 0;
    public Transform castingPoint; // Assign the casting point transform in the inspector
    public float castingPower = 50f; // Adjust this as needed
    public float castingAngle = 45f; // Adjust this as needed
    public bool overrideAngle = true; // Set to false if you want to use the manual angle set in the Inspector


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CastSpellAtPlayer();
        }

        // Switch grimoire with key 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentGrimoireIndex = (currentGrimoireIndex + 1) % grimoires.Length;
        }

        // Switch spells within the grimoire with key 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Grimoire currentGrimoire = grimoires[currentGrimoireIndex];
            currentSpellIndex = (currentSpellIndex + 1) % currentGrimoire.spells.Count;
        }
    }

    private void CastSpellAtPlayer()
    {
        Grimoire currentGrimoire = grimoires[currentGrimoireIndex];
        if (currentGrimoire.spells.Count > currentSpellIndex)
        {
            Spell spellToCast = currentGrimoire.spells[currentSpellIndex];
            DummyMagicalWeapon dummyWeapon = GetComponent<DummyMagicalWeapon>();

            // Check if we want to override the casting angle based on player position
            if (overrideAngle)
            {
                // Calculate the direction to the player
                Vector3 directionToPlayer = playerTarget.position - castingPoint.position;
                // Calculate the angle from the dummy to the player in degrees
                castingAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            }

            // Now use the adjusted angle and the desired power to cast the spell
            dummyWeapon.CastSpell(castingPoint, spellToCast, castingAngle, castingPower);
        }
    }

    public void CastSpell(Transform castingPoint, Spell spellToCast, float angle, float power)
    {
        // Adjust the angle to Unity's coordinate system if necessary. This might need to be negative or offset by 180 degrees.
        float adjustedAngle = angle - 180f;

        // Convert the angle to a direction vector
        Vector2 direction = new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));

        // Instantiate the spell's effect
        GameObject spellEffect = Instantiate(spellToCast.effectPrefab, castingPoint.position, Quaternion.Euler(0f, 0f, adjustedAngle));

        // Apply the power to the spell's Rigidbody if it has one
        Rigidbody spellRb = spellEffect.GetComponent<Rigidbody>();
        if (spellRb != null)
        {
            spellRb.velocity = direction * power; // Set the velocity in the direction calculated above
        }

        // Use a type check or some identifier to determine which collision script to add
        if (spellToCast is FireballSpell)
        {
            FireballSpell fireballSpell = (FireballSpell)spellToCast;
            FireballCollisionHandler collisionHandler = spellEffect.AddComponent<FireballCollisionHandler>();
            collisionHandler.Setup(fireballSpell.minDamage, fireballSpell.maxDamage, fireballSpell.impactRadius, fireballSpell.explosionEffectPrefab);
        }
    }
}
