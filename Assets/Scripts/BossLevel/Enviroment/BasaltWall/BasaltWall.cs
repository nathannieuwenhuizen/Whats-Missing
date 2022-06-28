using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasaltWall : MonoBehaviour, ITriggerArea
{
    [SerializeField]
    private Transform forceOrigin;
    [SerializeField]
    private GameObject particles;

    public delegate void BasaltEvent();
    public static BasaltEvent OnDestroy;


    public bool InsideArea { get; set; } = false;
    private bool canBeDestroyed = false;
    private bool destroyed = false;
    public void OnAreaEnter(Player player)
    {
        if (canBeDestroyed) DestroyWall();
    }

    public void OnAreaExit(Player player)
    {
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.D))  {
        //     DestroyWall();
        // }
    }

    private void OnEnable() {
        MirrorShard.OnPickedUp += OnShardPickup;
    }

    private void OnDisable() {
        MirrorShard.OnPickedUp -= OnShardPickup;
    }

    private void OnShardPickup(MirrorShard _shard) {
        int index = _shard.ShardIndex;
        if (index == 1) {
            canBeDestroyed = true;
        }
    }

    public void DestroyWall() {
        if (destroyed) return;
        destroyed = true;


        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<ParticleSystem>() == null) {
                child.AddComponent<DestroyableMesh>();
                StartCoroutine(child.GetComponent<DestroyableMesh>().SplitMesh(true, forceOrigin.position));
            }
        }
        particles.SetActive(true);
    }
}
