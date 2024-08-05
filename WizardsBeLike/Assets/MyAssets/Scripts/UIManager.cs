using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject royalVictoryPanel;
    public GameObject goldDefeatPanel;
    public GameObject goldVictoryPanel;
    public GameObject royalDefeatPanel;
    // ... other UI references ...

    // Static singleton property
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // Optional: if you want to persist across scene loads
            // DontDestroyOnLoad(gameObject);
        }
    }


    // Call this method to display the end game panel with the correct data
    public void ShowEndGamePanel(bool victory, Player.Team team)
    {
        // Deactivate all panels first
        royalVictoryPanel.SetActive(false);
        goldDefeatPanel.SetActive(false);
        goldVictoryPanel.SetActive(false);
        royalDefeatPanel.SetActive(false);

        // Activate the appropriate panel and set the player death counts
        if (victory)
        {
            if (team == Player.Team.Royal)
            {
                royalVictoryPanel.SetActive(true);
                // Update the player death counts on the royal victory panel
            }
            else if (team == Player.Team.Gold)
            {
                goldVictoryPanel.SetActive(true);
                // Update the player death counts on the gold victory panel
            }
        }
        else
        {
            if (team == Player.Team.Royal)
            {
                goldVictoryPanel.SetActive(true); // Gold team wins if Royal team is defeated
                                                  // Update the player death counts on the gold victory panel
            }
            else if (team == Player.Team.Gold)
            {
                royalVictoryPanel.SetActive(true); // Royal team wins if Gold team is defeated
                                                   // Update the player death counts on the royal victory panel
            }
        }
    }


    // You'll need to implement the method to update the player death counts in the panels
}
