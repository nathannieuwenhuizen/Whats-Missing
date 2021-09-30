using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomParticleSystem : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
        // ps.playOnAwake = false;
        ps.Pause();
    }
    // Update is called once per frame
     void Update()
     {
        ps.Simulate(Time.deltaTime * Room.TimeScale, true, false);
     }
}
