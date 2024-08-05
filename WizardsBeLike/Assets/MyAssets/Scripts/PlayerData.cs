using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public Grimoire chosenGrimoire;
    public Item[] items = new Item[3];
    public Player.Team team;
    public GameObject characterPrefab;
    // ... Add other selections here ...
}
