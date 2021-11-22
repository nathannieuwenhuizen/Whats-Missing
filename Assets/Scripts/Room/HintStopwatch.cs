using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintStopwatch : MonoBehaviour
{
    [HideInInspector]
    public Room room;

    private float currentDuration = 0;
    private bool timesUp = false;

    private Coroutine waitingCoroutine;

    private float duration;
    public float Duration {
        get { return duration;}
        set { duration = Mathf.Max(1f, value); }
    }

    public void Resume() {
        waitingCoroutine = StartCoroutine(Waiting());
    }

    private IEnumerator Waiting() {
        while (currentDuration < duration) {
            yield return new WaitForSeconds(1f);
            currentDuration++;
        }
        TimesUp();
    }
    private void TimesUp() {
        if (timesUp) return;
        timesUp = true;
        room.ShowMirrorToggleHint();
    }

    public void Pause() {
        if (waitingCoroutine != null) {
            StopCoroutine(waitingCoroutine);
        }
    }
}
