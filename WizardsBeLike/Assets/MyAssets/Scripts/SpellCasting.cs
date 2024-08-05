using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;



public class SpellCasting : MonoBehaviour
{

    public MagicalWeapon magicalWeapon;
    public Transform castingPoint;
    //public Grimoire currentGrimoire; // This will be set to the grimoire the player has chosen
    public PlayerMovement playerMovement;
    public float maxCastingPower = 100f;
    public float chargeRate = 20f;
    private float currentCastingPower = 0f;
    private bool isCharging = false;
    public AudioClip chargingSound;
    public TextMeshProUGUI cooldownText;
    public Button superSpellButton;
    private float superSpellCooldownTimer = 0f;
    public TextMeshProUGUI superSpellText; // The text that says "SUPER SPELL"
    private bool isSuperSpellOnCooldown = false;
    public Transform chargingEffectPoint;
    private AudioSource audioSource;
    private ParticleSystem chargingEffectInstance;
    private Animator animator;
    private int currentSpellIndex = 0; // Index of the current spell
    public float CurrentCastingPower { get { return currentCastingPower; } }
    public bool IsCharging { get { return isCharging; } }
    private int superSpellIndex = 3; // Assuming index 3 is the super spell
    public Player player; // Add this line


    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void Start()
    {
        animator = GetComponent<Animator>(); // Ensure you have an Animator component attached to the GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chargingSound;
        audioSource.loop = true;
        superSpellText.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleSuperSpellCooldown();

        // Ignore inputs if chat is active.
        if (ChatController.isChatActive)
        {
            return;
        }

        HandleInput();

        if (player.isActivePlayer)
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSpell(0);
            //if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSpell(1);
            //if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSpell(2);
        }
    }

    private void HandleInput()
    {
        if (!player.isActivePlayer || ChatController.isChatActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCharging();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            FinishCharging();
        }

        if (isCharging)
        {
            ChargeSpell();
        }
    }

    private void StartCharging()
    {
        Grimoire currentGrimoire = player.chosenGrimoire; // Get the chosen grimoire from Player
        if (!isCharging && playerMovement.CanMove && !playerMovement.IsMoving() && !magicalWeapon.IsShieldActive())
         {
            isCharging = true;
            currentCastingPower = 0f;
            playerMovement.EnableMovement(false);
            if (animator != null)
            {
                animator.SetBool("IsCharging", true);
            }
            audioSource.Play();

            // Instantiate the grimoire's charging effect when charging starts
            if (currentGrimoire.chargingEffectPrefab != null && chargingEffectPoint != null)
            {
                chargingEffectInstance = Instantiate(currentGrimoire.chargingEffectPrefab, chargingEffectPoint.position, Quaternion.identity, chargingEffectPoint);
                chargingEffectInstance.Play();
            }
         }
    }

    private void ChargeSpell()
    {
        currentCastingPower += chargeRate * Time.deltaTime;
        currentCastingPower = Mathf.Min(currentCastingPower, maxCastingPower);
    }

    private void FinishCharging()
    {
        if (isCharging)
        {
            isCharging = false;
            if (animator != null)
            {
                animator.SetBool("IsCharging", false);
                animator.SetTrigger("CastSpell"); // This sets the trigger
            }
            CastSpell();
            audioSource.Stop();
            if (chargingEffectInstance != null)
            {
                chargingEffectInstance.Stop();
                Destroy(chargingEffectInstance.gameObject, chargingEffectInstance.main.duration);
            }
        }
    }

    public void ChangeSpell(int spellIndex)
    {
        if (spellIndex >= 0 && spellIndex < player.chosenGrimoire.spells.Count)
        {
            currentSpellIndex = spellIndex;
            // Optionally do something to indicate the new spell is selected
        }
    }

    public void CastSpell()
{
        if (player.chosenGrimoire != null && player.chosenGrimoire.spells.Count > currentSpellIndex)
        {
            // First, instantiate the casting effect
            if (player.chosenGrimoire.castingEffectPrefab != null)
            {
                ParticleSystem castingEffectInstance = Instantiate(player.chosenGrimoire.castingEffectPrefab, castingPoint.position, Quaternion.identity);
                castingEffectInstance.Play();
            }

            Spell spellToCast = player.chosenGrimoire.spells[currentSpellIndex];
            spellToCast.Cast(castingPoint, playerMovement, magicalWeapon, currentCastingPower);

            // Record the last used angle immediately after casting the spell
            magicalWeapon.RecordLastAngle();

            if (spellToCast is ISuperSpell superSpell)
            {
                StartSuperSpellCooldown(superSpell.CooldownTime);
                ChangeSpell(0); // Switch to a default spell
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndTurn();
            }
        }
        // Reset casting power after casting
        currentCastingPower = 0f;
        playerMovement.EnableMovement(true); // Enable movement after the spell is cast
    }

   private void HandleSuperSpellCooldown()
   {
        if (isSuperSpellOnCooldown)
        {
            superSpellCooldownTimer -= Time.deltaTime;
            if (superSpellCooldownTimer <= 0f)
            {
                isSuperSpellOnCooldown = false;
                superSpellCooldownTimer = 0f;
                superSpellButton.interactable = true; // Re-enable the button
                superSpellText.gameObject.SetActive(true);
                cooldownText.gameObject.SetActive(false);
                //cooldownText.text = "SUPER SPELL"; // Reset the button text
            }
            else
            {
            // Update the text with the remaining cooldown time, rounded up
            cooldownText.text = Mathf.Ceil(superSpellCooldownTimer).ToString();
            }
        }
   }

    public void StartSuperSpellCooldown(float cooldownTime)
    {
        if (!isSuperSpellOnCooldown)
        {
            isSuperSpellOnCooldown = true;
            superSpellCooldownTimer = cooldownTime;
            superSpellButton.interactable = false; // Disable the button
            superSpellText.gameObject.SetActive(false); // Hide the Super Spell text
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = cooldownTime.ToString("F0"); // Update the text with full cooldown time
        }
    }

    public void StopChargingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void ResetCharging()
    {
        isCharging = false;
        currentCastingPower = 0f;
        // Also reset any UI elements or states related to charging here
    }

    public void ReduceSuperSpellCooldown(float reductionTime)
    {
        if (isSuperSpellOnCooldown)
        {
            superSpellCooldownTimer -= reductionTime;
            superSpellCooldownTimer = Mathf.Max(superSpellCooldownTimer, 0f); // Ensure cooldown doesn't go below zero
            // Update the UI if needed
        }
    }

    public void InterruptSpellCasting()
    {
        if (isCharging)
        {
            isCharging = false;
            if (animator != null)
            {
                animator.SetBool("IsCharging", false);
            }
            audioSource.Stop();
            if (chargingEffectInstance != null)
            {
                chargingEffectInstance.Stop();
                Destroy(chargingEffectInstance.gameObject, chargingEffectInstance.main.duration);
            }
            currentCastingPower = 0f;
            playerMovement.EnableMovement(true);
        }
    }

}
