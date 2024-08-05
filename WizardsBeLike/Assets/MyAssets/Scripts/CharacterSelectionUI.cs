using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour
{
    public TMP_InputField characterNameInput;
    public TMP_Dropdown grimoireDropdown, itemSlot1Dropdown, itemSlot2Dropdown, itemSlot3Dropdown, teamDropdown;
    public GameObject wizardBlackBeardPrefab; // Assign the correct prefab for each character
    public GameObject wizardBlondeBeardPrefab;
    public GameObject wizardGreyBeardPrefab;
    public GameObject wizardRedBeardPrefab;

    public Grimoire[] grimoireOptions;
    public Item[] itemOptions; // Assign this array in the Inspector with all your Item ScriptableObjects.



    private Dictionary<string, GameObject> beardToPrefabMapping;

    private void Start()
    {
        // Initialize your prefab mapping here, linking the string identifiers to the correct prefabs
        beardToPrefabMapping = new Dictionary<string, GameObject>
        {
            { "BlackBeard", wizardBlackBeardPrefab },
            { "BlondeBeard", wizardBlondeBeardPrefab },
            { "GreyBeard", wizardGreyBeardPrefab },
            { "RedBeard", wizardRedBeardPrefab }
        };

        PopulateGrimoireDropdown();
        PopulateItemDropdowns();
        PopulateTeamDropdown();


    }

    public void OnReadyButtonPressed(string beardColor)
    {
        GameObject selectedPrefab = beardToPrefabMapping[beardColor];
        PlayerData newPlayerData = new PlayerData
        {
            playerName = characterNameInput.text,
            chosenGrimoire = GetSelectedGrimoire(),
            items = new Item[] {
            GetSelectedItem(itemSlot1Dropdown),
            GetSelectedItem(itemSlot2Dropdown),
            GetSelectedItem(itemSlot3Dropdown)
        },
            team = GetSelectedTeam(),
            characterPrefab = selectedPrefab
        };

        TransitionalData.PlayersData.Add(newPlayerData);
        // Trigger any necessary logic to check all players are ready and to load the game scene
    }


    private void PopulateItemDropdowns()
    {
        // Populate each item dropdown with the item names.
        // This assumes you have a separate dropdown for each item slot.
        PopulateItemDropdown(itemSlot1Dropdown, itemOptions);
        PopulateItemDropdown(itemSlot2Dropdown, itemOptions);
        PopulateItemDropdown(itemSlot3Dropdown, itemOptions);
    }

    private void PopulateItemDropdown(TMP_Dropdown dropdown, Item[] items)
    {
        dropdown.ClearOptions();
        List<string> itemNames = new List<string>();
        foreach (var item in items)
        {
            itemNames.Add(item.itemName);
        }
        dropdown.AddOptions(itemNames);
    }

    // Get the selected Item from the dropdown
    private Item GetSelectedItem(TMP_Dropdown dropdown)
    {
        if (dropdown.value >= 0 && dropdown.value < itemOptions.Length)
        {
            return itemOptions[dropdown.value];
        }
        else
        {
            Debug.LogError("The selected item index is out of range.");
            return null;
        }
    }

    // Populate the grimoire dropdown with the names of the grimoires
    private void PopulateGrimoireDropdown()
    {
        if (grimoireDropdown != null)
        {
            grimoireDropdown.ClearOptions();
            List<string> grimoireNames = new List<string>();
            foreach (var grimoire in grimoireOptions)
            {
                grimoireNames.Add(grimoire.grimoireName);
            }
            grimoireDropdown.AddOptions(grimoireNames);
        }
        else
        {
            Debug.LogError("Grimoire Dropdown is not assigned in the inspector!");
        }
    }


    // Gets the selected Grimoire object based on the selected dropdown index
    private Grimoire GetSelectedGrimoire()
    {
        if (grimoireDropdown.value < grimoireOptions.Length)
        {
            return grimoireOptions[grimoireDropdown.value];
        }
        else
        {
            Debug.LogError("The selected grimoire index is out of range.");
            return null;
        }
    }

    private void PopulateTeamDropdown()
    {
        teamDropdown.ClearOptions();
        List<string> teamNames = new List<string>
    {
        Player.Team.Royal.ToString(),
        Player.Team.Gold.ToString()
        // Add more teams here if necessary
    };
        teamDropdown.AddOptions(teamNames);
    }

    public Player.Team GetSelectedTeam()
    {
        return (Player.Team)teamDropdown.value;
    }
}

