using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!SteamManager.Initialized) {
            Debug.Log("something went wrong...");
            return;
        }

        // string name = SteamFriends.GetPersonaName();
        // Debug.Log("name: " + name);
        SteamUserStats.ResetAllStats(true);



        
    }

    public string achievementName;

    private void Update() {
        if (!SteamManager.Initialized) {
            Debug.Log("something went wrong...");
            return;
        }
        ;SteamUserStats.RequestCurrentStats();
        achievementName = SteamUserStats.GetAchievementName(0);
        SteamUserStats.RequestCurrentStats();
        Debug.Log("achievement name 0 = " + SteamUserStats.GetAchievementName(0));
        Debug.Log("achievement name 1 = " + SteamUserStats.GetAchievementName(1));

        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Debug.Log("achievement should be granted!");
        //     SteamUserStats.SetAchievement("ACH_TEST");
        //     SteamUserStats.StoreStats();
        // }

        if (SteamManager.Initialized) {
            string name = SteamFriends.GetPersonaName ();
            Debug.Log (name+" - "+SteamUser.GetSteamID() );

            CGameID m_GameID = new CGameID (SteamUtils.GetAppID ());
            Debug.Log ("number of achievements: " + SteamUserStats.GetNumAchievements ());
            Debug.Log ("gameID: " + m_GameID);

        } else {
            Debug.Log ("Steam not initialized");
        }

    }

    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
         
    void OnEnable() 
    {
        if (!SteamManager.Initialized)
            return;
        m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
        SteamUserStats.SetAchievement("ACH_TEST");
        SteamUserStats.StoreStats();
    }
    
    void OnAchievementStored (UserAchievementStored_t pCallback)
    {
        
    }
}
