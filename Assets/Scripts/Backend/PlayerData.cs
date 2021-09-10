using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static string PLAYER_NAME {
        get => PlayerPrefs.GetString("PlayerName", "");
        set => PlayerPrefs.SetString("PlayerName", value);
    }
}
