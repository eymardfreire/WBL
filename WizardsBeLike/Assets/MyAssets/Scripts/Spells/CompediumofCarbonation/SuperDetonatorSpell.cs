using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SuperDetonatorSpell", menuName = "Spells/SuperDetonatorSpell")]
public class SuperDetonatorSpell : Spell, ISuperSpell
{
    public GameObject explosionEffectPrefab; // Assign a suitable effect through the inspector
    public float timeScaleDuringCast = 0.5f;
    public float cooldownTime = 60f; // Cooldown time for this specific super spell
    public float CooldownTime => cooldownTime;

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
{
    SpellCasting spellCastingComponent = playerMovement.GetComponent<SpellCasting>();
    if (spellCastingComponent != null)
    {
        // Pass the spellCastingComponent reference to the CastSequence
        spellCastingComponent.StartCoroutine(CastSequence(spellCastingComponent));
    }
}

    private IEnumerator CastSequence(SpellCasting spellCastingComponent) // Include the parameter here
{
    // Similar setup as in SuperFireballSpell for time scale and scene effects
    Light currentDirectionalLight = FindObjectOfType<Light>();
    float originalLightIntensity = currentDirectionalLight.intensity;
    Material currentSkyboxMaterial = RenderSettings.skybox;
    float originalExposure = currentSkyboxMaterial.GetFloat("_Exposure");
    float originalTimeScale = Time.timeScale;

    Time.timeScale = timeScaleDuringCast;
    // Use the passed spellCastingComponent to start the coroutine
    yield return spellCastingComponent.StartCoroutine(AdjustSceneEffects(currentDirectionalLight, currentSkyboxMaterial, 0, true)); // Darken and slow time

    // Activate all barrels
    BrewersMinefieldCollisionHandler[] barrels = FindObjectsOfType<BrewersMinefieldCollisionHandler>();
    foreach (var barrel in barrels)
    {
        barrel.Explode();
    }

    // Again, use the passed spellCastingComponent to start the coroutine
    yield return spellCastingComponent.StartCoroutine(AdjustSceneEffects(currentDirectionalLight, currentSkyboxMaterial, 1, false)); // Restore scene

    Time.timeScale = originalTimeScale;
    currentDirectionalLight.intensity = originalLightIntensity;
    currentSkyboxMaterial.SetFloat("_Exposure", originalExposure);
}

    private IEnumerator AdjustSceneEffects(Light light, Material skyboxMaterial, float targetValue, bool isDarkening)
    {
        float startValue = isDarkening ? 1 : 0;
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime)
        {
            light.intensity = Mathf.Lerp(startValue, targetValue, t);
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(startValue, targetValue, t));
            yield return null;
        }
    }
}