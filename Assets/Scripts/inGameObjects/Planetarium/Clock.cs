using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : RoomObject
{

    [SerializeField]
    private Transform secondArrow;
    [SerializeField]
    private Transform minuteArrow;
    [SerializeField]
    private Transform hourArrow;

    [SerializeField]
    private AnimationCurve tickAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);

    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;

    [SerializeField]
    [Range(0,1f)]
    private float tickDuration = .5f;

    private bool running = true;
    private bool animating = false;

    private IEnumerator Ticking() {
        animating = true;
        while(running) {
            yield return new WaitForSeconds(1f);
            while (Room.TimeScale == 0)
            {
                 yield return new WaitForEndOfFrame();
            }
            Seconds++;
        }
    }
    // void Start()
    // {
    //     SetClockToNow();
    //     StartCoroutine(Ticking());
    // }
    public void SetClockToNow() {
        Seconds = System.DateTime.Now.Second;
        Minutes = System.DateTime.Now.Minute;
        Hours = System.DateTime.Now.Hour;
    }

    public int Seconds {
        get => seconds;
        set {
            seconds = value;
            if (seconds >= 60) {
                seconds = 0;
                Minutes++;
            }
            UpdateArrow(secondArrow, value);
        }
    }
    public int Minutes {
        get => minutes;
        set {
            minutes = value;
            if (minutes >= 60) {
                minutes = 0;
                Hours++;
            }
            UpdateArrow(minuteArrow, value);
        }
    }
    public int Hours {
        get => hours;
        set {
            hours = value;
            if (hours >= 60) {
                hours = 0;
            }
            UpdateArrow(hourArrow, value);
        }
    }

    public void UpdateArrow(Transform arrow, int val) {
        float total = arrow == hourArrow ? 12f : 60f;
        float precentage = (float)val / total;
        float rotation =  (360 * precentage);
        if (!animating) {
            arrow.transform.localRotation = Quaternion.Euler(rotation, 0,0);
        } else {
            if (arrow == secondArrow) {
                AudioHandler.Instance.Player3DSound(SFXFiles.clock_ticking, transform, .5f, 1f, false, false, 40);
            }
            StartCoroutine(TickAnimation(arrow, rotation));
        }
    }

    private IEnumerator TickAnimation(Transform arrow, float val) {
        Quaternion start = arrow.transform.localRotation;
        Quaternion end = Quaternion.Euler(val, 0,0);

        float index = 0;
        while (index < tickDuration) {
            arrow.transform.localRotation = Quaternion.SlerpUnclamped(start, end, tickAnimationCurve.Evaluate(index/ tickDuration));
            yield return new WaitForEndOfFrame();
            index += Time.deltaTime;
        }
        arrow.transform.localRotation = Quaternion.Euler(val, 0,0);
    }

    public override void OnRoomEnter()
    {
        SetClockToNow();
        StopAllCoroutines();
        StartCoroutine(Ticking());
        base.OnRoomEnter();
    }

    public override void OnRoomLeave()
    {
        StopAllCoroutines();
        base.OnRoomLeave();
    }

    private void Reset() {
        Word = "clock";
    }

}
