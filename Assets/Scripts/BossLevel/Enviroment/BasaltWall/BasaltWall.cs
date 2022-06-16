using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasaltWall : MonoBehaviour, ITriggerArea
{
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
        if (Input.GetKeyDown(KeyCode.D))  {
            DestroyWall();
        }
    }

    public void DestroyWall() {
        if (destroyed) return;
        destroyed = true;

        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.AddComponent<DestroyableMesh>();
            StartCoroutine(child.GetComponent<DestroyableMesh>().SplitMesh(true));
        }
    }
}
