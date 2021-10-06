using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    [SerializeField]
    private MeshRenderer mr;
    private Material material;


    protected override void Awake() {
        base.Awake();
        material = mr.material;
    }

    SFXInstance Instance;
    public override void OnRoomEnter()
    {
        StartCoroutine(CheckTimeScale());
        if (Instance == null) {
            Instance =  AudioHandler.Instance.Player3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 15);
        }
        Instance.AudioSource.Play();
    }

    public IEnumerator CheckTimeScale() {
        while (true) {
            yield return new WaitForEndOfFrame();
            material.SetFloat("_RoomTime", Room.TimeScale);
        }
    }

    public override void OnRoomLeave()
    {
        StopAllCoroutines();
        if (Instance == null)
            Instance.AudioSource.Stop();
    }


    private void Reset() {
        Word = "Fire";
        AlternativeWords = new string[] {"Flame"};
    }
}
