using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BarrelBomberSpell", menuName = "Spells/BarrelBomberSpell")]
public class BarrelBomberSpell : Spell
{
    public float minDamage = 40f;
    public float maxDamage = 60f;
    public float impactRadius; // Radius for applying damage
    public float deformRadius; // Radius for deforming the mesh
    public GameObject explosionEffectPrefab; // Assign this through the inspector
    public float delayBeforeExplosion = 2f; // Time before the barrel explodes after the first bounce
    public GameObject[] piecePrefabs;
    public float explosionForce = 1000f; // Adjust the force as needed
    public float explosionRadius = 5f; // Adjust the radius as needed
    public Vector3 explosionOffset = new Vector3(0, 1, 0); // Adjust the offset as needed


    // Other properties and methods remain the same as FireballSpell...

   public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
{
    WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

    // Instantiate the barrel at the casting point without any rotation
    GameObject barrelInstance = UnityEngine.Object.Instantiate(effectPrefab, castingPoint.position, Quaternion.identity);

    // Calculate the initial velocity based on the casting power and weapon angle
    float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
    Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

    // Determine the character's current forward direction
    Vector3 characterForward = playerMovement.transform.forward;
    characterForward.z = 0; // Ignore the z-axis as we're working in 2D

    // Apply the initial velocity to the barrel's Rigidbody component
    Rigidbody barrelRb = barrelInstance.GetComponent<Rigidbody>();
    if (barrelRb != null)
    {
        // Adjust the x component of the direction based on the character's forward direction
        float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
        barrelRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

        // Apply wind effect if WindManager is available
        if (windManager != null)
        {
            Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
            barrelRb.velocity += windForce;
        }
    }

    // Add the BarrelCollisionHandler component to the barrel instance and pass all required arguments
    BarrelCollisionHandler collisionHandler = barrelInstance.AddComponent<BarrelCollisionHandler>();
    collisionHandler.Setup(minDamage, maxDamage, impactRadius, deformRadius, explosionEffectPrefab, delayBeforeExplosion);
    collisionHandler.piecePrefabs = this.piecePrefabs;
}
}
