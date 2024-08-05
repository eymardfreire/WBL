using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    private Button button;
    public int itemIndex;

    // Static reference to the active player
    public static Player ActivePlayer { get; set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Use the static reference to the active player
        if (ActivePlayer != null && ActivePlayer.isActivePlayer && ActivePlayer.items.Count > itemIndex)
        {
            ActivePlayer.UseItem(itemIndex); // Pass the index instead of the item object
        }
    }
}


