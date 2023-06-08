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
    private AcceptanceVivienne vivienne;

    [SerializeField]
    private Door door;

    [SerializeField]
    private SceneLightSetting hallwayLightning;
    [SerializeField]
    private SceneLightSetting bedroomLightning;
    
    
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
        door.Player = player;
        door.InSpace = true;
        door.Locked = false;

        hallwayLightning.startIntensity = hallwayLightning.directionalLight.intensity;
        bedroomLightning.startIntensity = bedroomLightning.directionalLight.intensity;
        bedroomLightning.directionalLight.enabled = false;
    }
    private void Start() {
        door.Locked = false;
        player.Respawn();
        StartCoroutine(PlayingMusic());
        
    }

    public IEnumerator PlayingMusic() {
        yield return new WaitForSeconds(4f);
        PlayAreaMusic();
    }

    ///<summary>
    /// Plays the area music according in which area you're in
    ///</summary>
    private void PlayAreaMusic() {
        //fade audio listener
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 1;
        AudioHandler.Instance.FadeListener(1f);

        //play music
        AudioHandler.Instance.PlayMusic(MusicFiles.EndlessHallway, 1f,  0f );
    }

    private void OnEnable() {
        Door.OnPassingThrough += OnDoorPass;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnDoorPass;
    }

    private void OnDoorPass(Door door) {
        StartCoroutine(AnimateSceneLights(door));
    }
    public IEnumerator AnimateSceneLights(Door _door) {
        float i = 0;
        float duration = 1f;
        SceneLightSetting start, end;

        if (_door.PlayerIsAtStartSide()) {
            start = hallwayLightning;
            end = bedroomLightning;
        } else {
            start = bedroomLightning;
            end = hallwayLightning;
        }
        start.directionalLight.enabled = true;
        end.directionalLight.enabled = true;

        while (i  < duration) {
            i += Time.deltaTime;
            float percentage = AnimationCurve.EaseInOut(0,0,1,1).Evaluate(i / duration);
            UpdateRenderSettings(SceneLightSetting.LerpUnclamped(start ,end, percentage));
            start.directionalLight.intensity = Mathf.Lerp(start.startIntensity, 0, percentage);
            end.directionalLight.intensity = Mathf.Lerp(0, end.startIntensity, percentage);
            yield return new WaitForEndOfFrame();
        } 
        // start.directionalLight.enabled = false;

    }

    private void UpdateRenderSettings(SceneLightSetting setting) {
        RenderSettings.ambientGroundColor = setting.ambientColors.groundColor;
        RenderSettings.ambientEquatorColor = setting.ambientColors.equatorColor;
        RenderSettings.ambientSkyColor = setting.ambientColors.skyColor;
        RenderSettings.sun.color = setting.ambientColors.sunColor;
        RenderSettings.fogDensity = setting.fogStrength;
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
        vivienne.transform.position -= moveDelta;
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
[System.Serializable]
public class SceneLightSetting {
    public Light directionalLight;
    [HideInInspector]
    public float startIntensity;
    public AmbientColors ambientColors;
    public float fogStrength;

    public static SceneLightSetting LerpUnclamped (SceneLightSetting a, SceneLightSetting b, float t) {
        return new SceneLightSetting() {
            ambientColors = AmbientColors.LerpUnclamped(a.ambientColors,b.ambientColors,t),
            fogStrength = Mathf.Lerp(a.fogStrength, b.fogStrength, t)
        };
    }
}