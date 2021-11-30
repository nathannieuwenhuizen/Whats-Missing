using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : RoomObject
{
    [SerializeField]
    private Vector3 size = new Vector3(5,5, 5);
    public static Vector3 DIRECTION = new Vector3(1,0,0);
    public static float SPEED = .01f;
    [SerializeField]
    private GameObject cloudSpherePrefab;

    private List<CloudSphere> cloudSpheres = new List<CloudSphere>();
    [SerializeField]
    private float ammountOfClouds = 20;

    [SerializeField]
    private float minScale  = 5f;

    [SerializeField]
    private float maxScale  = 5f;

    private CloudSphere SpawnCloud() {
        GameObject go = Instantiate(cloudSpherePrefab);
        go.transform.SetParent(transform);
        CloudSphere sphere = go.GetComponent<CloudSphere>();
        sphere.cloud = this;
        return sphere;
    }

    private void Start() {
        for(int i = 0; i < ammountOfClouds; i++) {
            CloudSphere sphere = SpawnCloud();
            resetCloud(sphere);
        }
    }

    public void resetCloud(CloudSphere sphere) {
        sphere.gameObject.transform.localPosition = new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            Random.Range(-size.y / 2f, size.y / 2f),
            Random.Range(-size.z / 2f, size.z / 2f)
        );
        sphere.lifeTime = Random.Range(5, 10);
        sphere.gameObject.transform.localScale = new Vector3(
            Random.Range(minScale, maxScale),
            Random.Range(minScale, maxScale),
            Random.Range(minScale, maxScale)
        );
        sphere.Reset();
    }

    private void Reset() {
        Word = "cloud";
        AlternativeWords = new string[] { "clouds", "smoke", "mist" };
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
