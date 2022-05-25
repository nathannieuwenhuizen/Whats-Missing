using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeGrass : MonoBehaviour
{
    [SerializeField]
    private IslandType islandType;

    private Vector3 startPos;

    private GrassComputeScript grass;


    private void Awake() {
        startPos = transform.position;
        grass = GetComponentInChildren<GrassComputeScript>();
    }
    private void OnEnable() {
        FloatingIsland.OnRoomEntering += UpdateGrassPosition;
        Wind.OnWindNormal += OnNormalWind;
        Wind.OnWindEnlarged += OnLargeWind;
    }

    public void OnLargeWind() {
        grass.windSpeed *= 10f;
        grass.windStrength *= 10f;
    }
    public void OnNormalWind() {
        grass.windSpeed /= 10f;
        grass.windStrength /= 10f;
    }

    private void OnDisable() {
        FloatingIsland.OnRoomEntering -= UpdateGrassPosition;
        Wind.OnWindNormal -= OnNormalWind;
        Wind.OnWindEnlarged -= OnLargeWind;

    }

    public void UpdateGrassPosition(FloatingIsland _floatingIsland) {
        if (startPos == null) startPos = transform.position;
        Debug.Log("update position");
        // transform.position = _floatingIsland.Room.transform.position + new Vector3(0,.1f,0) + startPos;
        
        transform.SetParent(_floatingIsland.Room.transform);
        transform.localPosition = startPos + new Vector3(14.2f,0,0);
        transform.localRotation = Quaternion.Euler(0,0,0);

        // transform.rotation = _floatingIsland.Room.transform.rotation;
        if (_floatingIsland.IslandType == islandType) {
            transform.SetParent(_floatingIsland.transform);
        } else {
        }
        
    }
}
