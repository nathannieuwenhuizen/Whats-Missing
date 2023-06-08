using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopDebrees : MonoBehaviour
{
    public delegate void BasaltEvent();
    public static BasaltEvent OnDestroy;

    private ParticleSystem[] pss;

    private float timeScale = 2f;

    public float TimeScale {
        get { return timeScale;}
        set { 
            timeScale = value;
            foreach(ParticleSystem ps in pss) {
                ParticleSystem.MainModule main = ps.main;
                main.simulationSpeed = value;
            } 
        }
    }

    private void OnEnable() {
        pss = GetComponentsInChildren<ParticleSystem>();
        // // ps.playOnAwake = false;
        // foreach(ParticleSystem ps in pss) ps.Pause();
        StopTime();
        OnDestroy.Invoke();
    }
    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     if (simulate) foreach(ParticleSystem ps in pss) ps.Simulate(Time.deltaTime * timeScale, true, false);
    // }
    private void StopTime() {
        Debug.Log("stop time!");
        AudioHandler.Instance?.Play3DSound(SFXFiles.rock_smash_time_stop, transform, 1f, 1f, false, true, 100f, false).Play();
        StartCoroutine( Extensions.AnimateCallBack(1,0, AnimationCurve.Linear(0,0,1,1), (float v) => {
            TimeScale = v;
        }, 2f));
    }

}