using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledParticleSystem : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
     void Update()
     {
         if (Time.timeScale < 0.01f)
         {
             ps.Simulate(Time.unscaledDeltaTime, true, false);
         }
     }
}
