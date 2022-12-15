using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessHallway : MonoBehaviour
{
    [SerializeField]
    private GameObject[] chunkPrefabs;
    [SerializeField]
    private float chunkSize = 10;
    [SerializeField]
    private int ammountOfChunks = 20;

    [SerializeField]
    private int indexBehindCamToTeleport = 5;

    [SerializeField]
    private GameObject startChunk;

    private List<HallwayChunk> chunks = new List<HallwayChunk>();

    [SerializeField]
    private Player player;

    [SerializeField]
    private PortalDoor endlessHallwayDoor;
    [SerializeField]
    private PortalDoor finalRoomDoor;
    
    private void SpawnChunks() {
        
        Vector3 spawnPos = startChunk.transform.position;
        for(int i = 0; i < ammountOfChunks; i++) {
            
            spawnPos += new Vector3(-chunkSize,0,0);
            GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)], spawnPos, Quaternion.identity);
            newChunk.name = "chunk #" + i;
            newChunk.transform.SetParent(transform);
            newChunk.active = true;
            chunks.Add(newChunk.GetComponent<HallwayChunk>());
            newChunk.GetComponent<HallwayChunk>().SetCeiling(i%2 != 0);
            newChunk.GetComponent<HallwayChunk>().ResetFurniture();
        }
    }
    private void Awake() {
        SpawnChunks();
        endlessHallwayDoor.Player = player;
        finalRoomDoor.Player = player;
        endlessHallwayDoor.ConnectedDoor = finalRoomDoor;
        finalRoomDoor.ConnectedDoor = endlessHallwayDoor;
        endlessHallwayDoor.InSpace = true;
        finalRoomDoor.InSpace = true;
        endlessHallwayDoor.Locked = false;
        finalRoomDoor.Locked = false;
    }


    private void Update() {
        if (player.transform.position.x > startChunk.transform.position.x) return;

        float delta = Mathf.Abs(startChunk.transform.position.x - player.transform.position.x );
        int indexDifference = Mathf.FloorToInt(delta / chunkSize);
        if (StartChunkIsBehind()) {
            if (indexDifference > 1) SetPlayerBack(indexDifference - 1);
        } else {
            if (indexDifference > indexBehindCamToTeleport) SetPlayerBack(indexDifference - 1 - indexBehindCamToTeleport);

        }
    }

    private bool StartChunkIsBehind() {
        Vector3 forward = player.transform.forward;
        Vector3 toOther = startChunk.transform.position - player.transform.position;
        float angle = Vector3.Angle(forward, toOther);
        if (Vector3.Dot(forward, toOther) < 0)
        {
            return true;
        }
        return false;
    }


    private void SetPlayerBack(float ammountOfSegments) {
        Vector3 moveDelta = new Vector3(-chunkSize,0,0) * ammountOfSegments;
        player.transform.position -= moveDelta;
        player.Movement.SetOldPosToTransform();

        //make new list
        List<HallwayChunk> temp = new List<HallwayChunk>();
        foreach(HallwayChunk chunk in chunks) {
            int i = chunks.IndexOf(chunk);
            if (i < ammountOfSegments) {
                temp.Add(chunk);
            } else {
                chunk.transform.position -= moveDelta;
            }
        }
        //clear new chunk list
        foreach(HallwayChunk chunk in temp) {
            chunks.Remove(chunk);
        }

        //set new chunk back to start of environment
        foreach(HallwayChunk chunk in temp) {
            Debug.Log("chunk: " + chunk.name);
            chunk.ResetFurniture();
            chunk.transform.position = chunks[chunks.Count - 1].transform.position + new Vector3(-chunkSize,0, 0);
            chunks.Add(chunk);
        }
    }

}
