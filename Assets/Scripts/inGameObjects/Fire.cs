using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    [SerializeField]
    private MeshRenderer mr;
    private Material material;

    protected void Awake() {
        material = mr.material;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += CheckTimeScale;
        TimeProperty.onTimeAppearing += CheckTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= CheckTimeScale;
        TimeProperty.onTimeAppearing -= CheckTimeScale;
    }

    SFXInstance Instance;
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (Instance == null) {
            Instance =  AudioHandler.Instance.Player3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 15);
        }
        Instance.AudioSource.Play();
    }


    public void CheckTimeScale() {
        if (InSpace == false) return;

        material.SetFloat("_RoomTime", Room.TimeScale);
        if (Instance != null){
            Instance.AudioSource.mute = Room.TimeScale == 0;
        }
    }

    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        StopAllCoroutines();
        if (Instance == null)
            Instance.AudioSource.Stop();
    }


    private void Reset() {
        Word = "Fire";
        AlternativeWords = new string[] {"Flame"};
    }
}
