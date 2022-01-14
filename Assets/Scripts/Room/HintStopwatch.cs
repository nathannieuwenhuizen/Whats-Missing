using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintStopwatch : MonoBehaviour
{
    [HideInInspector]
    public Room room;

    private float currentDuration = 0;
    private bool timesUp = false;

    public bool timerForSecondHint = false;

    private void Awake() {
        room = GetComponent<Room>();
    }

    private Coroutine waitingCoroutine;

    private float duration;
    public float Duration {
        get { return duration;}
        set { duration = Mathf.Max(1f, value); }
    }
    private IEnumerator Waiting() {
        while (currentDuration < duration) {
            yield return new WaitForSeconds(1f);
            currentDuration++;
        }
        TimesUp();
    }

    public void Pause() {
        if (waitingCoroutine != null) {
            StopCoroutine(waitingCoroutine);
        }
    }
    public void Resume() {
        waitingCoroutine = StartCoroutine(Waiting());
    }

    private void OnEnable() {
        MirrorCanvas.OnShowHint += StartTimerSecondHint;
    }

    private void OnDisable() {
        MirrorCanvas.OnShowHint -= StartTimerSecondHint;
    }

    public void StartTimerSecondHint(string hint, float _duration) {
        if (!room.InArea) return;
        if (timerForSecondHint) return;
        timerForSecondHint = true;
        Duration = _duration;
        Reset();

    }

    public void Reset() {
        timesUp = false;
        currentDuration = 0;
        Resume();
    }

    private void TimesUp() {
        if (timesUp) return;
        timesUp = true;
        if (timerForSecondHint) room.ShowMirrorToggleSecondHint();
        else room.ShowMirrorToggleHint();
    }

}
