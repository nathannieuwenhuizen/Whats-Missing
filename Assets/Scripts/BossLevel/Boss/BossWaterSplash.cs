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

    private void Awake() {
        ps.emissionRate = 0;
    }
    private void Update() {
        if (waterArea.gameObject.activeSelf && waterArea.transform.position.y > bossPositioner.transform.position.y) {
            transform.position = new Vector3(transform.position.x, waterArea.transform.position.y, transform.position.z);
            ps.emissionRate = bossPositioner.SteeringBehaviour.Velocity.magnitude > 0.1f ? 40f : 0;
        } else {
            ps.emissionRate = 0;
        }
    }

}
