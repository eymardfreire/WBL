using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MarketMayhemSpell", menuName = "Spells/MarketMayhemSpell")]
public class MarketMayhemSpell : Spell, ISuperSpell
{
    public float bullMinDamage = 40f;
    public float bullMaxDamage = 60f;
    public float bearMinDamage = 70f;
    public float bearMaxDamage = 90f;
    public float bullImpactRadius; // Radius for applying damage
    public float bearImpactRadius;
    public float deformRadius; // Radius for deforming the mesh
    public GameObject bullPrefab; // Assign the bull prefab through the inspector
    public GameObject bearPrefab; // Assign the bear prefab through the inspector
    public GameObject explosionEffectPrefab; // Assign this through the inspector
    public GameObject collisionEffectPrefab; // Assign this through the inspector
    public float delayBeforeSummoningBear = 1f; // Time before the bear is summoned after the bull explodes
    public Vector3 bearSummonScale = new Vector3(1, 1, 1); // Adjust the bear summon scale as needed
    public float delayBeforeExplosion = 0f; // Set to 0 for instant explosion on impact
    public float timeScaleDuringCast = 0.5f; // Time scale to slow down the game during casting
    public float cooldownTime = 180f; // Cooldown time for this specific super spell
    public float CooldownTime => cooldownTime;
    


    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        SpellCasting spellCastingComponent = playerMovement.GetComponent<SpellCasting>();
        if (spellCastingComponent != null)
        {
            spellCastingComponent.StartCoroutine(CastSequence(castingPoint, playerMovement, magicalWeapon, currentCastingPower));
        }
    }

    private IEnumerator CastSequence(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        // Save the original values of the light intensity, skybox exposure, and time scale
        Light currentDirectionalLight = FindObjectOfType<Light>();
        float originalLightIntensity = currentDirectionalLight.intensity;
        Material currentSkyboxMaterial = RenderSettings.skybox;
        float originalExposure = currentSkyboxMaterial.GetFloat("_Exposure");
        float originalTimeScale = Time.timeScale;

        // Slow down time immediately
        Time.timeScale = timeScaleDuringCast;

        // Gradually reduce light intensity and skybox exposure
        float transitionDuration = 1.0f; // Duration of the transition in seconds
        float transitionProgress = 0.0f; // Progress of the transition

        while (transitionProgress < 1.0f)
        {
            transitionProgress += Time.unscaledDeltaTime / transitionDuration;
            float intensity = Mathf.Lerp(originalLightIntensity, originalLightIntensity * 0.5f, transitionProgress);
            float exposure = Mathf.Lerp(originalExposure, originalExposure * 0.5f, transitionProgress);
            currentDirectionalLight.intensity = intensity;
            currentSkyboxMaterial.SetFloat("_Exposure", exposure);
            yield return null;
        }

        // Instantiate the bull and apply forces immediately after altering the environment
        GameObject bullInstance = Instantiate(bullPrefab, castingPoint.position, Quaternion.identity);
        MarketMayhemCollisionHandler marketMayhemCollisionHandler = bullInstance.GetComponent<MarketMayhemCollisionHandler>();
        if (marketMayhemCollisionHandler != null)
        {
            marketMayhemCollisionHandler.Setup(bullMinDamage, bullMaxDamage, bullImpactRadius, deformRadius, explosionEffectPrefab, bearPrefab, bearSummonScale, bearMinDamage, bearMaxDamage, bearImpactRadius, collisionEffectPrefab, delayBeforeSummoningBear);
            ApplyInitialVelocity(bullInstance.GetComponent<Rigidbody>(), playerMovement, magicalWeapon, currentCastingPower);
        }

        // Wait for the effect to end
        yield return new WaitForSeconds(0f / timeScaleDuringCast); // Adjust this time as needed for your effect

        // Gradually restore light intensity and skybox exposure
        transitionProgress = 0.0f; // Reset progress for the restoration phase
        while (transitionProgress < 1.0f)
        {
            transitionProgress += Time.unscaledDeltaTime / transitionDuration;
            float intensity = Mathf.Lerp(originalLightIntensity * 0.5f, originalLightIntensity, transitionProgress);
            float exposure = Mathf.Lerp(originalExposure * 0.5f, originalExposure, transitionProgress);
            currentDirectionalLight.intensity = intensity;
            currentSkyboxMaterial.SetFloat("_Exposure", exposure);
            yield return null;
        }

        // Restore time scale to normal
        Time.timeScale = originalTimeScale;
    }
    
    private void ApplyInitialVelocity(Rigidbody rb, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance;
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0;
        float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
        rb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

        if (windManager != null)
        {
            Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
            // Start coroutine to apply wind over time
            playerMovement.StartCoroutine(ApplyWindOverTime(rb, windForce));
        }
    }

    private IEnumerator ApplyWindOverTime(Rigidbody rb, Vector3 windForce)
    {
        // Delay before starting to apply wind
        float windApplicationDelay = 0.5f; // Adjust the delay to your preference
        yield return new WaitForSeconds(windApplicationDelay);

        // Duration over which wind reaches full strength
        float windRampUpTime = 1f; // Adjust the ramp-up time to your preference
        float time = 0;

        while (time < windRampUpTime)
        {
            // Check if the Rigidbody still exists before accessing it
            if (rb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            rb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (rb != null)
        {
            rb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }
}

