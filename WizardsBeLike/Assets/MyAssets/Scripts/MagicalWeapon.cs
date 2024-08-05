using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicalWeapon : MonoBehaviour
{
    public float aimSpeed = 50f;
    public float minAngle = 80f;    // Upwards (a bit more than straight up)
    public float maxAngle = 190f;   // Downwards (a bit above straight forward)
    public float initialAngle = 0f; // The initial angle the weapon should face
    public Transform pivot;         // The pivot point for the weapon, likely the character
    private float currentAngle;
    private float lastAngle;        // To store the last angle used
    public GameObject magicalWeaponGameObject; // The entire GameObject of the MagicalWeapon
    public GameObject magicalShieldGameObject; // The entire GameObject of the MagicalShield
    public GameObject shieldActiveEffect;      // The ShieldActiveEffect under MagicalShield GameObject
    private bool isShieldActive = false;
    public float shieldActiveDuration = 2f; // Duration for which the shield is active
    public GameObject toggleWeaponEffectPrefab; // The effect for toggling the weapon
    public GameObject shieldActivationEffectPrefab; // The effect for shield activation


    public float CurrentAngle { get { return currentAngle; } }
    public float LastAngle { get { return lastAngle; } }

    void Start()
    {
        if (pivot == null)
        {
            pivot = transform.parent;  // Assuming the weapon is a child of the character
        }

        // Set the current angle to the initial angle at the start
        currentAngle = initialAngle;

        // Apply the initial rotation
        ApplyRotationAndPosition();
    }

    void Update()
    {
        // Reference to the Player component to check if this is the active player
        Player playerComponent = GetComponentInParent<Player>();

        // If the player is not active, then don't allow weapon movement
        if (!playerComponent.isActivePlayer)
        {
            return;
        }

        // Ignore inputs if chat is active.
        if (ChatController.isChatActive)
        {
            return;
        }

        // Inverting the input by multiplying by -1
        float inputVertical = -Input.GetAxis("Vertical");
        currentAngle += inputVertical * aimSpeed * Time.deltaTime;

        // Clamp the current angle between minAngle and maxAngle
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        ApplyRotationAndPosition();

        // When 'R' key is pressed, record the last angle used
        //if (Input.GetKeyDown(KeyCode.R))
        //{
            //lastAngle = currentAngle;
        //}

        // Toggle weapon/shield on 'Q' key press
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
            //ToggleWeaponShield();
        //}

        // Activate shield effect on 'E' key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ActivateShield());
        }
    }

        private void ToggleWeaponShield()
    {
        isShieldActive = !isShieldActive;

        MeshRenderer weaponRenderer = magicalWeaponGameObject.GetComponent<MeshRenderer>();
        if (weaponRenderer != null)
        {
            weaponRenderer.enabled = !isShieldActive;
        }

        magicalShieldGameObject.SetActive(isShieldActive);

        // Instantiate toggle effect at the weapon's location
        if (toggleWeaponEffectPrefab != null)
        {
            Instantiate(toggleWeaponEffectPrefab, magicalWeaponGameObject.transform.position, Quaternion.identity);
        }
    }


    private IEnumerator ActivateShield()
    {
        // If the shield itself is not active, we don't want to show the effect.
        if (!isShieldActive)
        {
            yield break;
        }

        // Activate the Shield's active effect
        shieldActiveEffect.SetActive(true);

        // Instantiate activation effect at the shield's position
        if (shieldActivationEffectPrefab != null)
        {
            Instantiate(shieldActivationEffectPrefab, shieldActiveEffect.transform.position, Quaternion.identity);
        }

        // Wait for the duration while the shield is active
        yield return new WaitForSeconds(shieldActiveDuration);

        // Deactivate the Shield's active effect
        shieldActiveEffect.SetActive(false);
    }


    // Implement a method to be called by the SpellCasting script
    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    private void ApplyRotationAndPosition()
    {
        // Calculate the new rotation by applying the current angle to the pivot's forward direction
        Vector3 newDirection = Quaternion.AngleAxis(currentAngle, pivot.right) * pivot.forward;
        transform.rotation = Quaternion.LookRotation(-newDirection, pivot.up);

        // Keep the weapon at a fixed distance from the pivot point
        transform.position = pivot.position + (-newDirection) * 1f; // Adjust the multiplier to set the distance
    }

    public void RecordLastAngle()
    {
        lastAngle = currentAngle;
    }
}
