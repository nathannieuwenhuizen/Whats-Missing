using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash
{

    [SerializeField]
    private ParticleSystem waveEmitter;

    private bool enableEmitter = false;
    public bool EnableEmitter {
        get { return enableEmitter;}
        set { enableEmitter = value; }
    }

    public WaterSplash(ParticleSystem _waveEmitter) {
        waveEmitter = _waveEmitter;
    }

    public void OnFootStep() {
        if (WaterArea.ON_WATER_SURFACE) {

            float yPos = WaterArea.WATER_TRANSFORM.position.y;
            Vector3 temp = waveEmitter.transform.position;
            temp.y = yPos;
            waveEmitter.transform.position = temp;
            waveEmitter.Emit(1);
        }
    }
}
