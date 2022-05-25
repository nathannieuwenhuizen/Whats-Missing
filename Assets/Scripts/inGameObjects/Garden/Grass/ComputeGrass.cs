using System.Collections;
using System.Collections.Generic;
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
        if (startPos == null) startPos = transform.position;

        Debug.Log("update position");
        
        transform.SetParent(_floatingIsland.Room.transform);
        transform.localPosition = startPos + new Vector3(14.2f,0,0);
        transform.localRotation = Quaternion.Euler(0,0,0);
        if (_floatingIsland.IslandType == islandType) {
            transform.SetParent(_floatingIsland.transform);
        } else {
        }
        
    }
}
