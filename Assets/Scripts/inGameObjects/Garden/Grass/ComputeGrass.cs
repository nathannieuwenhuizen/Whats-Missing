using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ComputeGrass : MonoBehaviour
{
    [SerializeField]
    private IslandType islandType;

    private Vector3 startPos;

    private GrassComputeScript grass;

    private float normalWindSpeed;
    private float normalWindStrength;

    private void Awake() {
        startPos = transform.position;
        grass = GetComponentInChildren<GrassComputeScript>();
        normalWindSpeed = grass.windSpeed;
        normalWindStrength = grass.windStrength;
    }
    public void OnLargeWind() {
        grass.windSpeed = normalWindSpeed * 10f;
        grass.windStrength = normalWindStrength * 10f;
    }
    public void OnNormalWind() {
        grass.windSpeed = normalWindSpeed;
        grass.windStrength = normalWindStrength;
    }

    private void UpdateGrassTimeScale() {
        grass.windSpeed = normalWindSpeed * Room.TimeScale;
    }

    private void OnEnable() {
        FloatingIsland.OnRoomEntering += UpdateGrassPosition;
        Wind.OnWindNormal += OnNormalWind;
        Wind.OnWindEnlarged += OnLargeWind;
        TimeProperty.onTimeMissing += UpdateGrassTimeScale;
        TimeProperty.onTimeAppearing += UpdateGrassTimeScale;

    }

    private void OnDisable() {
        FloatingIsland.OnRoomEntering -= UpdateGrassPosition;
        Wind.OnWindNormal -= OnNormalWind;
        Wind.OnWindEnlarged -= OnLargeWind;
        TimeProperty.onTimeMissing -= UpdateGrassTimeScale;
        TimeProperty.onTimeAppearing -= UpdateGrassTimeScale;

    }

    public void UpdateGrassPosition(FloatingIsland _floatingIsland) {

        if (_floatingIsland.IslandType == islandType) {
            if (startPos == null) startPos = transform.position;
            transform.SetParent(_floatingIsland.Room.transform);
            transform.localPosition = startPos + new Vector3(14.2f,0,0);
            transform.localRotation = Quaternion.Euler(0,0,0);

            Debug.Log("set grass parent: " + _floatingIsland.name);
            transform.SetParent(_floatingIsland.transform);
            // Vector3 delta = _floatingIsland.transform.position - _floatingIsland.Room.transform.position;
            // transform.localPosition = (startPos + new Vector3(14.2f,0,0) - delta) * 0.01f;
            // transform.localRotation = Quaternion.Euler(0,0,0);
        } 
        if (islandType == IslandType.main) {
            if (startPos == null) startPos = transform.position;
            transform.SetParent(_floatingIsland.Room.transform);
            transform.localPosition = startPos + new Vector3(14.2f,0,0);
            transform.localRotation = Quaternion.Euler(0,0,0);
        }
        
    }
}
