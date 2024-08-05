using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement; // This namespace is required for SceneManager
using TMPro;
using UnityEngine.UI;



public class CharacterSelectionManager : MonoBehaviour
{
    public CharacterSelectionUI blackBeardSelectionUI;
    public CharacterSelectionUI blondeBeardSelectionUI;
    public CharacterSelectionUI greyBeardSelectionUI;
    public CharacterSelectionUI redBeardSelectionUI;

    public static string selectedStage;
    //public TMP_Dropdown stageDropdown; // Assign this in the Inspector

    public Toggle toggleTestingGrounds;
    public Toggle toggleSecondStage;
    public Toggle toggleOuterSpace;
    public Toggle toggleDungeons;

    private bool[] playersReady;
    public List<string> stages;



    // Start method
    void Start()
    {
        //PopulateStageDropdown(stageDropdown); // Assuming you have multiple stages

        // Add listeners for the Toggles
        toggleTestingGrounds.onValueChanged.AddListener(delegate { SetStage("TestingGrounds", toggleTestingGrounds.isOn); });
        toggleSecondStage.onValueChanged.AddListener(delegate { SetStage("TheWildWest", toggleSecondStage.isOn); });
        toggleOuterSpace.onValueChanged.AddListener(delegate { SetStage("OuterSpace", toggleOuterSpace.isOn); });
        toggleDungeons.onValueChanged.AddListener(delegate { SetStage("VolcanoDungeons", toggleDungeons.isOn); });




        playersReady = new bool[4]; // Assuming 4 players 

    }

    public void SetStage(string stageName, bool isOn)
    {
        if (isOn)
        {
            selectedStage = stageName;
        }
    }

    // This method should be called when a player presses the "Ready" button
    public void SetPlayerReady(int playerIndex, bool isReady)
    {
        if (playerIndex >= 0 && playerIndex < playersReady.Length)
        {
            playersReady[playerIndex] = isReady;
            Debug.Log($"Setting player {playerIndex} ready state to {isReady}");

            if (isReady)
            {
                string beardColor = GetBeardColorFromIndex(playerIndex);
                GetSelectionUIByBeardColor(beardColor).OnReadyButtonPressed(beardColor);
            }
        }
        else
        {
            Debug.LogError("Player index is out of range.");
        }
    }

    private string GetBeardColorFromIndex(int index)
    {
        // This assumes a direct mapping order similar to how you have your UI set up
        switch (index)
        {
            case 0:
                return "BlackBeard";
            case 1:
                return "BlondeBeard";
            case 2:
                return "GreyBeard";
            case 3:
                return "RedBeard";
            default:
                Debug.LogError("Invalid player index: " + index);
                return null; // or handle this case as you see fit
        }
    }

    // Check if all players are ready
    public bool AllPlayersReady()
    {
        foreach (bool isReady in playersReady)
        {
            if (!isReady)
            {
                return false; // If any player is not ready, return false
            }
        }
        return true; // All players are ready
    }

    // Method to populate the stage dropdown
    private void PopulateStageDropdown(TMP_Dropdown dropdown)
    {
        List<TMP_Dropdown.OptionData> stageOptions = new List<TMP_Dropdown.OptionData>();
        foreach (string stage in stages)
        {
            stageOptions.Add(new TMP_Dropdown.OptionData(stage));
        }
        dropdown.options = stageOptions;
    }

    public void StageSelected(int stageIndex)
    {
        // Assuming you have a list of stages like List<string> stages;
        selectedStage = stages[stageIndex];
    }

    // Hook this method up to the TMP_Dropdown's onValueChanged UnityEvent in the inspector.


    public void StartGame()
    {
        Debug.Log("Attempting to start game.");
        if (AllPlayersReady() && !string.IsNullOrEmpty(selectedStage))
        {
            TransitionalData.ClearPlayersData(); // Clear the list before adding new data

            bool hasRoyalTeamPlayer = false;
            bool hasGoldTeamPlayer = false;

            // Add the new player data to the list and check for team distribution
            if (blackBeardSelectionUI != null)
            {
                blackBeardSelectionUI.OnReadyButtonPressed("BlackBeard");
                hasRoyalTeamPlayer |= blackBeardSelectionUI.GetSelectedTeam() == Player.Team.Royal;
                hasGoldTeamPlayer |= blackBeardSelectionUI.GetSelectedTeam() == Player.Team.Gold;
            }
            if (blondeBeardSelectionUI != null)
            {
                blondeBeardSelectionUI.OnReadyButtonPressed("BlondeBeard");
                hasRoyalTeamPlayer |= blondeBeardSelectionUI.GetSelectedTeam() == Player.Team.Royal;
                hasGoldTeamPlayer |= blondeBeardSelectionUI.GetSelectedTeam() == Player.Team.Gold;
            }
            if (greyBeardSelectionUI != null)
            {
                greyBeardSelectionUI.OnReadyButtonPressed("GreyBeard");
                hasRoyalTeamPlayer |= greyBeardSelectionUI.GetSelectedTeam() == Player.Team.Royal;
                hasGoldTeamPlayer |= greyBeardSelectionUI.GetSelectedTeam() == Player.Team.Gold;
            }
            if (redBeardSelectionUI != null)
            {
                redBeardSelectionUI.OnReadyButtonPressed("RedBeard");
                hasRoyalTeamPlayer |= redBeardSelectionUI.GetSelectedTeam() == Player.Team.Royal;
                hasGoldTeamPlayer |= redBeardSelectionUI.GetSelectedTeam() == Player.Team.Gold;
            }

            if (hasRoyalTeamPlayer && hasGoldTeamPlayer)
            {
                Debug.Log("All players ready. Loading scene: " + selectedStage);
                SceneManager.LoadScene(selectedStage);
            }
            else
            {
                Debug.LogError("Both teams must have at least one player.");
                // Display an error message indicating that both teams need at least one player
            }
        }
        else
        {
            Debug.LogError("Not all players are ready or no stage selected.");
            // Display an error message or indicate that not all players are ready
        }
    }

    public void ReadyButtonPressed(string beardColor)
    {
        CharacterSelectionUI selectionUI = GetSelectionUIByBeardColor(beardColor);
        selectionUI.OnReadyButtonPressed(beardColor);
    }

    private CharacterSelectionUI GetSelectionUIByBeardColor(string beardColor)
    {
        switch (beardColor)
        {
            case "BlackBeard": return blackBeardSelectionUI;
            case "BlondeBeard": return blondeBeardSelectionUI;
            case "GreyBeard": return greyBeardSelectionUI;
            case "RedBeard": return redBeardSelectionUI;
            default: throw new ArgumentException("Invalid beard color!");
        }
    }

    // Call this method when all players are ready to start the game.
    public void LoadGameScene()
    {
        // Your logic to check if all players are ready
        // If they are, load the game scene
        SceneManager.LoadScene("GameScene");
    }


    public void SetPlayerOneReady(bool isReady)
    {
        SetPlayerReady(0, isReady);
    }

    public void SetPlayerTwoReady(bool isReady)
    {
        SetPlayerReady(1, isReady);
    }

    public void SetPlayerThreeReady(bool isReady)
    {
        SetPlayerReady(2, isReady);
    }

    public void SetPlayerFourReady(bool isReady)
    {
        SetPlayerReady(3, isReady);
    }

}

