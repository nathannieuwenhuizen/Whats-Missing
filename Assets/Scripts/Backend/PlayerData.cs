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

    public static bool GARDEN_GHOST_VANISH_0 {
         get { return PlayerPrefs.GetInt("GARDEN_GHOST_VANISH_0", 0) == 1; }
         set {  PlayerPrefs.SetInt("GARDEN_GHOST_VANISH_0", value ? 1 : 0); }
    }
    public static bool GARDEN_GHOST_VANISH_1 {
         get { return PlayerPrefs.GetInt("GARDEN_GHOST_VANISH_1", 0) == 1; }
         set {  PlayerPrefs.SetInt("GARDEN_GHOST_VANISH_1", value ? 1 : 0); }
    }
    public static bool GARDEN_GHOST_VANISH_2 {
         get { return PlayerPrefs.GetInt("GARDEN_GHOST_VANISH_2", 0) == 1; }
         set {  PlayerPrefs.SetInt("GARDEN_GHOST_VANISH_2", value ? 1 : 0); }
    }
    public static bool GARDEN_GHOST_VANISH_3 {
         get { return PlayerPrefs.GetInt("GARDEN_GHOST_VANISH_3", 0) == 1; }
         set {  PlayerPrefs.SetInt("GARDEN_GHOST_VANISH_3", value ? 1 : 0); }
    }
    public static bool GARDEN_GHOST_VANISH_4 {
         get { return PlayerPrefs.GetInt("GARDEN_GHOST_VANISH_4", 0) == 1; }
         set {  PlayerPrefs.SetInt("GARDEN_GHOST_VANISH_4", value ? 1 : 0); }
    }

    public static int TIME_IN_MS {
         get { return PlayerPrefs.GetInt("TIME_IN_MS", 0); }
         set {  PlayerPrefs.SetInt("TIME_IN_MS", value); }
    }

    public static void CheckSeekerfDeathAchievements() {
     Debug.Log("check seeker of death");
        if (DIE_AREA_0 && DIE_AREA_1 && DIE_AREA_2 && DIE_AREA_3) {
          Debug.Log("seeker achievment!");
            SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.SeekerOfDeaths);
        }
    }
    public static void CheckChasingThePastAchievements() {
     Debug.Log("check chasing the past");
        if (GARDEN_GHOST_VANISH_0 && GARDEN_GHOST_VANISH_1 && GARDEN_GHOST_VANISH_2 && GARDEN_GHOST_VANISH_3) {
          Debug.Log("chasing past achievement!");
            SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.ChasingThePast);
        }
    }
    
}

