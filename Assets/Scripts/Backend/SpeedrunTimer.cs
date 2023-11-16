using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    private float currentTimeInMS;
    private bool running = false;

    public delegate void TimerAction(float ms); 
    public static event TimerAction OnTimerUpdate;


    private void LoadTime() {
        currentTimeInMS = PlayerData.TIME_IN_MS;
        OnTimerUpdate?.Invoke(currentTimeInMS);
    }

    private void Start() {
        LoadTime();
        Resume();
    }

    private void Resume() {
        running = true;

    }
    private void Pause() {
        running = false;

    }

    private void StopTimer() {
        Pause();
        float endTime = (currentTimeInMS / 1000);
        Debug.Log("end time = " + endTime);
        if (endTime < 15 * 60) {
            Debug.Log("speedrunner achieved");
            SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.Speedrunner);
        }
    }

    private void OnEnable() {
        PauseScreen.OnPause += Pause;
        PauseScreen.OnResume += Resume;
        CharacterAnimationPlayer.OnCutsceneStart += Pause;
        CharacterAnimationPlayer.OnCutsceneEnd += Resume;
        CreditsRoller.OnCreditsStart += StopTimer;
    }
    private void OnDisable() {
        PauseScreen.OnPause -= Pause;
        PauseScreen.OnResume -= Resume;
        CharacterAnimationPlayer.OnCutsceneStart -= Pause;
        CharacterAnimationPlayer.OnCutsceneEnd -= Resume;
        CreditsRoller.OnCreditsStart -= StopTimer;
        PlayerData.TIME_IN_MS = (int)currentTimeInMS;

    }

    private void Update() {
        if (running) {
            currentTimeInMS += Time.deltaTime * 1000;
            OnTimerUpdate?.Invoke(currentTimeInMS);
        }
    }
}
