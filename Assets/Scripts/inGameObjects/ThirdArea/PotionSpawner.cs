using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPoint {
    [SerializeField]
    public Transform spawnLocation;
    [SerializeField]
    public GameObject potionPrefab;
    [SerializeField]
    public ChangeType changeType;
}

///<summary>
/// Responisble for spawning the potions inside the room. 
///</summary>
public class PotionSpawner : RoomObject
{
    [SerializeField]
    private SpawnPoint[] spawnPoints;
    
    public RoomPotionData potionData {
        get; set;
    }

    private bool spawned = false;

    ///<summary>
    /// SpawnsAll the potions
    ///</summary>
    protected override void Awake() {
        base.Awake();
        largeScale = normalScale;
        word = "potionspawner";
    }
    private void Start() {
        // foreach(SpawnPoint point in spawnPoints) {
        //     SpawnPotion(point);
        // }
    }
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (spawned) return;
        spawned = true;

        foreach(SpawnPoint point in spawnPoints) {
            SpawnPotion(point);
        }
    }


    ///<summary>
    /// Spawns a potion on the designated spawnPoint
    ///</summary>
    public void SpawnPotion(SpawnPoint spawnPoint) {
        //returns if it isn't able to spawn a potion
        if (!CanSpawnPotion(spawnPoint)) return;
        Potion potion = Instantiate(spawnPoint.potionPrefab, spawnPoint.spawnLocation.position, Quaternion.identity).GetComponent<Potion>();
        potion.gameObject.transform.parent = spawnPoint.spawnLocation;
        spawnPoint.changeType = potion.ChangeType;
        Vector3 endScale = Vector3.one * potion.NormalScale;
        potion.transform.localScale = Vector3.zero;
        StartCoroutine(potion.transform.AnimatingLocalScale(endScale, AnimationCurve.EaseInOut(0,0,1,1), 2f));
    }

    private void OnEnable() {
        Potion.OnBreak += RefillPotion;
    }

    private void OnDisable() {
        Potion.OnBreak -= RefillPotion;
    }


    ///<summary>
    /// Returns true if it can spawn a potion with the potion data.
    ///</summary>
    private bool CanSpawnPotion(SpawnPoint _spawnPoint) {
        // if (potionData == null) return true;
        if (!potionData.missingPotionAvailable && _spawnPoint.changeType == ChangeType.missing) return false;
        if (!potionData.tooBigPotionAvailable && _spawnPoint.changeType == ChangeType.tooBig) return false;
        if (!potionData.tooSmallPotionAvailable && _spawnPoint.changeType == ChangeType.tooSmall) return false;
        return true;
    }

    ///<summary>
    /// Refills the potion spawn point with a new potion.
    ///</summary>
    private void RefillPotion(Potion potion) {
        if (!InSpace) return;
        SpawnPoint selectedSpawnPoint = new List<SpawnPoint>(spawnPoints).Find(x => x.changeType == potion.ChangeType);
        SpawnPotion(selectedSpawnPoint);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        foreach(SpawnPoint point in spawnPoints) {
            if (point.spawnLocation != null) {
                Gizmos.DrawWireSphere(point.spawnLocation.position, .5f);
            }
        }
    }
}
