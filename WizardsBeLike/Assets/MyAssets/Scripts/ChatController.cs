using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatController : MonoBehaviour
{
    public TMP_InputField[] playerInputFields; // Input fields for all players
    public TextMeshProUGUI[] playerChatBubbles; // Chat bubbles for all players
    public int maxMessageLength = 50; // Maximum length of the message
    public GameObject[] chatBubbles; // Assuming these are your chat bubble GameObjects
    public float messageDisplayTime = 3f; // Time to display chat bubble
    public static bool isChatActive = false;

    void Start()
    {
        // Register the OnEndEdit event listener for each player's input field
        foreach (TMP_InputField inputField in playerInputFields)
        {
            inputField.onEndEdit.AddListener(delegate { OnEndEdit(inputField); });
        }
    }

    private void OnEndEdit(TMP_InputField inputField)
    {
        // Find out the index of the input field that was edited
        int playerIndex = System.Array.IndexOf(playerInputFields, inputField);

        // If the input was not due to a loss of focus, send the message
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSendButtonClicked(playerIndex);
        }
    }


    // Call this method when a player sends a message
    public void PlayerSendMessage(int playerIndex)
    {
        if (playerIndex < playerInputFields.Length && playerIndex < playerChatBubbles.Length)
        {
            string message = playerInputFields[playerIndex].text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Update the corresponding player's chat bubble and clear the input field
                playerChatBubbles[playerIndex].text = message;
                playerInputFields[playerIndex].text = "";

                // Call to resize the chat bubble based on the message
                AdjustChatBubbleSize(playerChatBubbles[playerIndex], message);

                // TODO: Send message to server to broadcast to other clients
            }
        }
    }

    // Resizes the chat bubble based on the message length
    private void AdjustChatBubbleSize(TextMeshProUGUI chatBubble, string message)
    {
        // Logic to resize the chat bubble...
        // Example, adjusting the rectTransform to fit the text content
    }

    // Update this function to handle message broadcasting in multiplayer
    private void BroadcastMessage(string message, int playerIndex)
    {
        // Multiplayer code to send the message to the server and then to other clients
    }

    public void OnSendButtonClicked(int playerIndex)
    {
        // Validate playerIndex and message content before sending
        if (playerIndex >= 0 && playerIndex < playerInputFields.Length)
        {
            string message = playerInputFields[playerIndex].text;
            if (!string.IsNullOrWhiteSpace(message) && message.Length <= maxMessageLength)
            {
                // Update chat bubble and clear input field
                playerChatBubbles[playerIndex].text = message;
                playerInputFields[playerIndex].text = "";

                // Adjust chat bubble size
                AdjustChatBubbleSize(playerChatBubbles[playerIndex], message);

                // Handle networking to broadcast message
                BroadcastMessage(message, playerIndex);
            }
        }
        // Show chat bubble
        StartCoroutine(DisplayChatBubble(playerIndex));
    }

    private IEnumerator DisplayChatBubble(int playerIndex)
    {
        chatBubbles[playerIndex].SetActive(true);
        yield return new WaitForSeconds(messageDisplayTime);
        chatBubbles[playerIndex].SetActive(false);
    }

    // Call this when the input field is selected
    public void OnChatInputSelected()
    {
        isChatActive = true;
        // Possibly disable other input-related UI or game elements here
    }

    // Call this when the input field is deselected or after sending the message
    public void OnChatInputDeselected()
    {
        isChatActive = false;
        // Possibly re-enable other input-related UI or game elements here
    }

    private void Update()
    {
        // Example to check for chat input activity in the Update method
        if (isChatActive)
        {
            // Disable specific input actions
        }
        else
        {
            // Enable specific input actions
        }
    }

}
