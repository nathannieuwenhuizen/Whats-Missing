using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpread : MonoBehaviour, ITriggerArea
{

    public delegate void EnterEvent(FireSpread spread);
    public static EnterEvent OnFireSpreadEnter;
    public static EnterEvent OnFireSpreadExit;
    private SFXInstance fireSound;


    public bool InsideArea { get; set; }

    private void Start() {
        if (fireSound == null) {
            fireSound =  AudioHandler.Instance.Play3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 40);
        }

    }

    public void OnAreaEnter(Player player)
    {
        Debug.Log("fire spread enter");
        OnFireSpreadEnter?.Invoke(this);
    }

    private void OnDisable() {
        OnFireSpreadExit?.Invoke(this);
    }

    public void OnAreaExit(Player player)
    {
        OnFireSpreadExit?.Invoke(this);

    }
}
