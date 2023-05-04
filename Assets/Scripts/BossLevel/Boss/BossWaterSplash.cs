using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boss;
 
public class BossWaterSplash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    [SerializeField]
    private BossPositioner bossPositioner;
    [SerializeField]
    private WaterArea waterArea;

    private SFXInstance waterSound;

    private void Awake() {
        ps.emissionRate = 0;


    }
    private void Update() {
        if (waterArea.gameObject.activeSelf && waterArea.transform.position.y > bossPositioner.transform.position.y) {
            transform.position = new Vector3(transform.position.x, waterArea.transform.position.y, transform.position.z);
            ps.emissionRate = bossPositioner.SteeringBehaviour.Velocity.magnitude > 0.1f ? 40f : 0;
            if (waterSound == null) {
                waterSound =  AudioHandler.Instance?.Play3DSound(SFXFiles.rain, transform, 1f, 1, true, true, 150f, false);
                waterSound.Play();
            }
            waterSound.Volume = 1f;
        } else {
            ps.emissionRate = 0;
            if (waterSound != null) waterSound.Volume = 0;
        }
    }

    private void OnDisable() {
        waterSound?.Pause();
    }
    private void OnEnable() {
        waterSound?.Play();
    }

}
