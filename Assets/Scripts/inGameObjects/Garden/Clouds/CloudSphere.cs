using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSphere : MonoBehaviour
{
    public Cloud cloud;
    public float lifeTime = 10f;
    [SerializeField]
    private Material cloudMaterial;
    [SerializeField]
    private float currentLifetime = 0;

    private float FresnelPower{
        get => cloudMaterial.GetFloat("_FresnelPower");
        set => cloudMaterial.SetFloat("_FresnelPower", value);
    }
    private void Awake() {
        cloudMaterial = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        transform.localPosition += Cloud.DIRECTION * Cloud.SPEED * Room.TimeScale * Time.deltaTime;
        currentLifetime += Time.deltaTime;
        FresnelPower = -2f + Mathf.Sin(Mathf.PI * (currentLifetime / lifeTime)) * 5f; 
        if (currentLifetime > lifeTime) {
            cloud.resetCloud(this);
        }
    }
    public void Reset() {
        currentLifetime = 0;
    }
}
