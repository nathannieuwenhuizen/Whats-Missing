using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public static bool BEHIND_THE_SCENES_UNLOCKED {
         get { return PlayerPrefs.GetInt("BEHIND_THE_SCENES_UNLOCKED", 0) == 1; }
         set {  PlayerPrefs.SetInt("BEHIND_THE_SCENES_UNLOCKED", value ? 1 : 0); }
    }
    public static bool DIE_AREA_0 {
         get { return PlayerPrefs.GetInt("DIE_AREA_0", 0) == 1; }
         set {  PlayerPrefs.SetInt("DIE_AREA_0", value ? 1 : 0); }
    }
    public static bool DIE_AREA_1 {
         get { return PlayerPrefs.GetInt("DIE_AREA_1", 0) == 1; }
         set {  PlayerPrefs.SetInt("DIE_AREA_1", value ? 1 : 0); }
    }
    public static bool DIE_AREA_2 {
         get { return PlayerPrefs.GetInt("DIE_AREA_2", 0) == 1; }
         set {  PlayerPrefs.SetInt("DIE_AREA_2", value ? 1 : 0); }
    }
    public static bool DIE_AREA_3 {
         get { return PlayerPrefs.GetInt("DIE_AREA_3", 0) == 1; }
         set {  PlayerPrefs.SetInt("DIE_AREA_3", value ? 1 : 0); }
    }

    public void CheckSeekerfDeathAchievements() {
        if (DIE_AREA_0 && DIE_AREA_1 && DIE_AREA_2 && DIE_AREA_3) {
            // SteamAchievementHandler.Instance?.SetAchievement()
        }
    }
    
}

