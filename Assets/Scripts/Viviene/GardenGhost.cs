using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The garden ghost of Gregories daughter in the garden level
///</summary>

public class GardenGhost : AreaTrigger, IRoomObject
{
    private bool disappear = false;

    public bool InSpace { get; set; } = false;

    public override void OnAreaEnter(Player player) {
        base.OnAreaEnter(player);
    }

    public override void OnAreaExit(Player player) {
        base.OnAreaExit(player);
        if (disappear) return;
        disappear = true;
        Vanish();
    }

    public void OnRoomEnter()
    {
        if (!disappear) {
            //make sounds
        }
    }

    public void OnRoomLeave()
    {
        if (!disappear) {
            StopSound();
        }
    }

    public void StopSound() {
        //stop sounds
    }
    

    public void Vanish() {
        //do vnaish animation
    }

}
