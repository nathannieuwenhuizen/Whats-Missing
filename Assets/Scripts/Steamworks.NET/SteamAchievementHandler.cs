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
        {SteamAchievement.ChasingThePast, "ChasingThePast"},
        {SteamAchievement.GoingInCircles, "GoingInCircles"},
        {SteamAchievement.PowerOfTheWind, "PowerOfTheWind"},
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
#if UNITY_EDITOR
        SteamUserStats.ResetAllStats(true);
#endif
    }

    public string achievementName;

    private void Update() {
        if (!SteamManager.Initialized) {
            Debug.Log("something went wrong...");
            return;
        }
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

        // bool alreadyAchieved = true;
        // SteamUserStats.GetAchievement(_name, out alreadyAchieved);
        // if (alreadyAchieved) return;

        Debug.Log("achievement should be granted!" + Achievements[_achievement]); 
        SteamUserStats.SetAchievement(Achievements[_achievement]);
        SteamUserStats.StoreStats();
    }
    
    void OnAchievementStored (UserAchievementStored_t pCallback)
    {
        
    }
}