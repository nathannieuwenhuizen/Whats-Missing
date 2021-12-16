using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour, ITriggerArea
{
        public bool InsideArea { get; set;} = false;

    public void OnAreaEnter(Player player)
    {
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_water;
    }

    public void OnAreaExit(Player player)
    {
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
    }

}
