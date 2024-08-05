using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Grimoire chosenGrimoire;
    public string playerName;
    //public bool isGoldTeam;
    public TMP_Text playerNameText;
    public Image nameTagImage;
    public GameObject optionsSkipPanel;
    public GameObject spellsPanel;
    public GameObject itemsPanel;
    public GameObject chatPanel;
    public GameObject auraEffect;
    public Transform effectPosition; // Assign this in the inspector


    public int lives; // Add this
    public Team team; // Add this, assuming you have a Team enum or class
    public GameObject characterPrefab; // Add this



    public bool isActivePlayer = false;

    public bool isRespawning = false;
    public bool isDead = false;

    // Reference to player components
    public PlayerMovement playerMovement;
    public SpellCasting spellCasting;
    private MagicalWeapon magicalWeapon;

    private List<GameObject> damageTextInstances = new List<GameObject>();

    // Items system variables
    public List<Item> items = new List<Item>(); // Holds the player's items
    public Button[] itemButtons; // References to the item buttons in the UI

    void Awake()
    {
        // Get the components
        playerMovement = GetComponent<PlayerMovement>();
        spellCasting = GetComponent<SpellCasting>();
        magicalWeapon = GetComponent<MagicalWeapon>();
        // ... initialize other components as needed ...

        // Set the player's name in the UI at the start
        UpdatePlayerUI();
    }

    void Start()
    {
        // Update the item button texts
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].GetComponentInChildren<TMP_Text>().text = items.Count > i ? items[i].itemName : "";
        }
    }


    // This function will be called by the GameManager to start the player's turn
    public void BeginTurn()
    {
        
        isActivePlayer = true;
        ItemButton.ActivePlayer = this; // Set this player as the active player for the item buttons
        playerMovement.UnlockPosition();

        playerMovement.ResetMovementAmount();
        playerMovement.EnableMovement(true);
        this.SetUIActive(true);
        if (auraEffect != null)
            auraEffect.SetActive(true); // Activate the aura effect

        spellCasting.ResetCharging(); // Reset charging (you should implement this)


    }

    // This function will be called by the GameManager to end the player's turn
    public void EndTurn()
    {

        isActivePlayer = false;
        ItemButton.ActivePlayer = null; // Clear the active player as the turn ends
        playerMovement.LockPosition();

        playerMovement.EnableMovement(false);
        // Disable other controls or UI elements specific to the active player here
        this.SetUIActive(false);
        if (auraEffect != null)
            auraEffect.SetActive(false); // Deactivate the aura effect

    }

    public void UpdatePlayerUI()
    {
        if (playerNameText != null)
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                playerNameText.text = playerName;
            }
        }

        // Change the color of the name tag based on the team
        if (nameTagImage != null)
        {
            Color goldTeamColor = new Color(1, 0.843f, 0); // Gold color
            Color royalTeamColor = new Color(147f / 255f, 56f / 255f, 255f / 255f); // Royal color with #9338FF hex code
            nameTagImage.color = team == Team.Gold ? goldTeamColor : royalTeamColor;
        }
    }

    public void SetUIActive(bool active)
    {
        optionsSkipPanel.SetActive(active);
        spellsPanel.SetActive(active);
        itemsPanel.SetActive(active);
        chatPanel.SetActive(active);
    }

    public enum Team
    {
        Royal,
        Gold
        // ... other teams if any
    }

    public void ResetHealth()
    {
        Damageable damageableComponent = GetComponent<Damageable>();
        if (damageableComponent != null)
        {
            // Reset the health to the max value, which is initially assigned to health variable in Damageable
            damageableComponent.health = damageableComponent.healthBarSlider.maxValue;

            // Update the health bar slider to match the reset health
            damageableComponent.healthBarSlider.value = damageableComponent.health;

            // If there are any other UI elements or effects related to health reset, update them here
        }
        else
        {
            Debug.LogError("ResetHealth: Damageable component not found on this GameObject.");
        }
    }
    public void RegisterDamageText(GameObject textInstance)
    {
        damageTextInstances.Add(textInstance);
    }

    public void ClearDamageTexts()
    {
        foreach (var textInstance in damageTextInstances)
        {
            Destroy(textInstance);
        }
        damageTextInstances.Clear();
    }

    public void UseItem(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < items.Count && items[itemIndex] != null)
        {
            // Get a reference to the item to use
            Item itemToUse = items[itemIndex];

            itemToUse.Use(this); // Use the item

            // Check if the item has an effect prefab assigned
            if (itemToUse.effectPrefab != null)
            {
                // Instantiate the effect at the position specified by effectPosition
                GameObject effectInstance = Instantiate(itemToUse.effectPrefab, effectPosition.position, Quaternion.identity);
                // Destroy the effect after the duration specified by the item
                Destroy(effectInstance, itemToUse.effectDuration);
            }

            // After using the item, set it to null in the items list
            items[itemIndex] = null;

            // Update the UI for this specific item button
            UpdateItemUI(itemIndex);
        }
    }


    private void UpdateItemUI(int itemIndex)
    {
        // Update just the button that was used
        TMP_Text buttonText = itemButtons[itemIndex].GetComponentInChildren<TMP_Text>();
        buttonText.text = ""; // Clear the button text
        itemButtons[itemIndex].interactable = false; // Disable the button

        // Remove listeners for this button
        itemButtons[itemIndex].onClick.RemoveAllListeners();
    }

    public void AssignNameText(TMP_Text nameTextUI)
    {
        playerNameText = nameTextUI;
        UpdatePlayerUI();
    }
    public void OnSkipTurnButtonPressed()
    {
        GameManager.Instance?.SkipTurn();
    }
}


