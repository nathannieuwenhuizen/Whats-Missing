using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Water : RoomObject, ITriggerArea
{

    private Material watermaterial;

    public bool InsideArea { get; set;} = false;

    private void Awake() {
        watermaterial = GetComponent<MeshRenderer>().material;
    }


    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
    }


    public void UpdateTimeScale() {
        if (InSpace == false) return;
        watermaterial.SetFloat("_RoomTime", Room.TimeScale);
    }

    private void Reset() {
        Word = "water";
        AlternativeWords = new string[] { "waters", "lake", "pond", "pool", "liquid" };
    }

    public void OnAreaEnter(Player player)
    {
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_water;
        Debug.Log("water enter");
    }

    public void OnAreaExit(Player player)
    {
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
        Debug.Log("water leave");

    }
}
