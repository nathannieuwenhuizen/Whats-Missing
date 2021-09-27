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
    void Start()
    {
        SetClockToNow();
        StartCoroutine(Ticking());
    }
    public void SetClockToNow() {
        Debug.Log(System.DateTime.Now.ToString("HH:mm:ss")); 

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
        float rotation = -90 + (360 * precentage);
        if (!animating) {
            arrow.transform.rotation = Quaternion.Euler(rotation, 0,0);
        } else {
            StartCoroutine(TickAnimation(arrow, rotation));
        }
    }

    private IEnumerator TickAnimation(Transform arrow, float val) {
        Quaternion start = arrow.transform.rotation;
        Quaternion end = Quaternion.Euler(val, 0,0);

        Debug.Log("start: " + start + " | end: " + val);
        float index = 0;
        while (index < tickDuration) {
            arrow.transform.rotation = Quaternion.SlerpUnclamped(start, end, tickAnimationCurve.Evaluate(index/ tickDuration));
            yield return new WaitForEndOfFrame();
            index += Time.deltaTime;
        }
        arrow.transform.rotation = Quaternion.Euler(val, 0,0);
    }

    private void Reset() {
        Word = "clock";
    }

}
