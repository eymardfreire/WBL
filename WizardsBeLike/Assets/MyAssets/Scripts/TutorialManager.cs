using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Button basicsButton;
    public Button grimoiresButton;
    public Button controlsButton;

    public GameObject basicsPanel;
    public GameObject grimoiresPanel;
    public GameObject controlsPanel;

    private GameObject currentPanel;

    void Start()
    {
        // Assign button listeners
        basicsButton.onClick.AddListener(() => ShowPanel(basicsPanel));
        grimoiresButton.onClick.AddListener(() => ShowPanel(grimoiresPanel));
        controlsButton.onClick.AddListener(() => ShowPanel(controlsPanel));

        // Initialize panels to be hidden
        HideAllPanels();
    }

    void Update()
    {
        // Check for the Escape key to close the current panel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAllPanels();
        }
    }

    public void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels first
        HideAllPanels();

        // Show the selected panel
        panelToShow.SetActive(true);
        currentPanel = panelToShow;
    }

    public void HideAllPanels()
    {
        // Set all panels to inactive
        basicsPanel.SetActive(false);
        grimoiresPanel.SetActive(false);
        controlsPanel.SetActive(false);

        currentPanel = null;
    }

    public void OnBackButtonPressed()
    {
        // Hide all panels when the back button is pressed
        HideAllPanels();
    }

    public void QuitGame()
    {
        // If we are running in the Unity editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing the game in the editor
        #else
        Application.Quit(); // Quit the application when built
        #endif
    }

}
