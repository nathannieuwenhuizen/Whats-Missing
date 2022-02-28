using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour, ITriggerArea
{
    public bool InsideArea { get; set;} = false;
    public static bool IN_WATER = false;

    private void OnDisable() {
        IN_WATER = false;
    }
    public void OnAreaEnter(Player player)
    {
        IN_WATER = true;
        // FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_water;
    }

    public void OnAreaExit(Player player)
    {
        IN_WATER = false;
        // FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
    }

}
