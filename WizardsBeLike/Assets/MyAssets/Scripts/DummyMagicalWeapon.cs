using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMagicalWeapon : MonoBehaviour
{
    public void CastSpell(Transform castingPoint, Spell spellToCast, float angle, float power)
    {
        // The passed-in 'angle' should already be in degrees and ready for use
        // Unity's forward vector is (0, 0, 1), so to rotate around the z-axis, we use Vector3.forward
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject spellEffect = Instantiate(spellToCast.effectPrefab, castingPoint.position, rotation);

        Rigidbody spellRb = spellEffect.GetComponent<Rigidbody>();
        if (spellRb != null)
        {
            // We use the rotation to turn the Vector3.right into the direction we want to cast the spell
            Vector3 forceDirection = rotation * Vector3.right; // Assumes that the right direction is the forward direction for the spell
            spellRb.velocity = forceDirection * power; // Apply the velocity using the calculated direction and power
        }
    }
}
