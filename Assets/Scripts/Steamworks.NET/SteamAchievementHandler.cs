using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamAchievementHandler : Singleton<SteamAchievementHandler>
{

    Dictionary<SteamAchievement, string> Achievements = new Dictionary<SteamAchievement, string>() {
        {SteamAchievement.Bargaining, "Bargaining"},
        {SteamAchievement.Denial, "Denial"},
        {SteamAchievement.Depression, "Depression"},
        {SteamAchievement.Anger, "Anger"},
        {SteamAchievement.Acceptance, "Acceptance"},

        {SteamAchievement.CatKiller, "CatKiller"},
        {SteamAchievement.EarlyBirdCatchesTheApple, "EarlyBirdCatchesTheApple"},
        {SteamAchievement.AgainTheApple, "AgainTheApple"},
        {SteamAchievement.StealMyBreathAway, "StealMyBreathAway"},
        {SteamAchievement.WhoNeedsAirAnyway, "WhoNeeds(Ch)airAnyway"},
        {SteamAchievement.SunDeath, "SunDeath"},
        {SteamAchievement.LetItBurn, "LetItBurn"},
        {SteamAchievement.QUACK, "QUACK"},
        {SteamAchievement.SeekerOfDeaths, "SeekerOfDeaths"},
        {SteamAchievement.PotionOfDeath, "PotionOfDeath"},
        {SteamAchievement.TheLibraryIsClosed, "TheLibraryIsClosed"},
        {SteamAchievement.Speedrunner, "Speedrunner"},
    };

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
        // ;SteamUserStats.RequestCurrentStats();
        // achievementName = SteamUserStats.GetAchievementName(0);
        // SteamUserStats.RequestCurrentStats();
        // Debug.Log("achievement name 0 = " + SteamUserStats.GetAchievementName(0));
        // Debug.Log("achievement name 1 = " + SteamUserStats.GetAchievementName(1));

        // // if (Input.GetKeyDown(KeyCode.Space)) {
        // //     Debug.Log("achievement should be granted!");
        // //     SteamUserStats.SetAchievement("ACH_TEST");
        // //     SteamUserStats.StoreStats();
        // // }

        // if (SteamManager.Initialized) {
        //     string name = SteamFriends.GetPersonaName ();
        //     Debug.Log (name+" - "+SteamUser.GetSteamID() );

        //     CGameID m_GameID = new CGameID (SteamUtils.GetAppID ());
        //     Debug.Log ("number of achievements: " + SteamUserStats.GetNumAchievements ());
        //     Debug.Log ("gameID: " + m_GameID);

        // } else {
        //     Debug.Log ("Steam not initialized");
        // }
    }

    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
         
    void OnEnable() 
    {
        if (!SteamManager.Initialized)
            return;
        m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
        // SteamUserStats.SetAchievement("ACH_TEST");
        // SteamUserStats.StoreStats();
    }

    public void SetAchievement(SteamAchievement _achievement) {
        if (!SteamManager.Initialized) {
            Debug.Log("something went wrong...");
            return;
        }
        ;SteamUserStats.RequestCurrentStats();
        string _name = SteamUserStats.GetAchievementName((uint)_achievement);
        SteamUserStats.RequestCurrentStats();

        Debug.Log("achievement should be granted!" + Achievements[_achievement]); 
        SteamUserStats.SetAchievement(Achievements[_achievement]);
        SteamUserStats.StoreStats();
    }
    
    void OnAchievementStored (UserAchievementStored_t pCallback)
    {
        
    }
}
