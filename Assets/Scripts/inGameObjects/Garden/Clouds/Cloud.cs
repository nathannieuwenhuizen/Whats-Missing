using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : RoomObject
{
    [SerializeField]
    private ParticleSystem cloudsParticleSystem;


    public Cloud() {
        normalScale = 1f;
        shrinkScale = 0.01f;
    }
    private void Reset() {
        Word = "cloud";
        AlternativeWords = new string[] { "clouds", "smoke", "mist" };
    }

    private void Awake() {
        cloudsParticleSystem.gameObject.SetActive(false);
    }

    public override void OnRoomEnter()
    {
        cloudsParticleSystem.gameObject.SetActive(true);
        base.OnRoomEnter();
    }

    public override void OnRoomLeave()
    {
        cloudsParticleSystem.gameObject.SetActive(false);
        base.OnRoomLeave();
    }
}
