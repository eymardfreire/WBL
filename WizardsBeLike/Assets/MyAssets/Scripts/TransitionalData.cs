using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionalData : MonoBehaviour
{
    public static List<PlayerData> PlayersData = new List<PlayerData>();

    public static void ClearPlayersData()
    {
        PlayersData.Clear();
    }
}
