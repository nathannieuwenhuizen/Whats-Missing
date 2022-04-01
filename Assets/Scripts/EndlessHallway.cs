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

    private List<GameObject> chunks = new List<GameObject>();


    [SerializeField]
    private Player player;
    
    private void SpawnChunks() {
        
        Vector3 spawnPos = startChunk.transform.position;
        for(int i = 0; i < ammountOfChunks; i++) {
            
            spawnPos += new Vector3(-chunkSize,0,0);
            GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)], spawnPos, Quaternion.identity);
            newChunk.name = "chunk #" + i;
            newChunk.transform.SetParent(transform);
            newChunk.active = true;
            chunks.Add(newChunk);
        }
    }
    private void Awake() {
        SpawnChunks();
    }

    private void Update() {
        if (player.transform.position.x > startChunk.transform.position.x) return;

        float delta = Mathf.Abs(startChunk.transform.position.x - player.transform.position.x );
        int indexDifference = Mathf.FloorToInt(delta / chunkSize);
        // Debug.Log("is behind = " +  StartChunkIsBehind());
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

        List<GameObject> temp = new List<GameObject>();

        foreach(GameObject chunk in chunks) {
            int i = chunks.IndexOf(chunk);
            if (i < ammountOfSegments) {
                temp.Add(chunk);
            } else {
                chunk.transform.position -= moveDelta;
            }
        }

        foreach(GameObject chunk in temp) {
            chunks.Remove(chunk);
        }
        foreach(GameObject chunk in temp) {
            chunk.transform.position = chunks[chunks.Count - 1].transform.position + new Vector3(-chunkSize,0, 0);
            chunks.Add(chunk);
        }
    }

}
