using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTerrainRenderer : MonoBehaviour
{

    private Vector3 startPos;
    private RenderTerrainMap terrainMap;


    private void Awake() {
        startPos = transform.position;
        terrainMap = GetComponent<RenderTerrainMap>();
    }
    private void OnEnable() {
        FloatingIsland.OnRoomEntering += UpdateGrassPosition;
    }

    private void OnDestroy() {
        
    }

    private void OnDisable() {
        FloatingIsland.OnRoomEntering -= UpdateGrassPosition;
    }

    public void UpdateGrassPosition(FloatingIsland _floatingIsland) {
        // return;

        if (startPos == null) startPos = transform.position;
        Debug.Log("update position");


        transform.position = _floatingIsland.Room.transform.position + new Vector3(0,.1f,0) + startPos;
        transform.SetParent(_floatingIsland.Room.transform);
        if (_floatingIsland.Renderers.Length > 0) {
            terrainMap.renderers = _floatingIsland.Renderers;
        }
        terrainMap.DrawDiffuseMap();
    }
}
