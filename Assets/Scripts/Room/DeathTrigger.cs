using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : AreaTrigger, IRoomObject
{
    public bool InSpace { get; set; }
    private Collider collider;
    private void Awake() {
        collider = GetComponent<Collider>();
    }
    public override void OnAreaEnter(Player player)
    {
        base.OnAreaEnter(player);

        player.Die(false);
    }

    public void OnRoomEnter()
    {
        collider.enabled = true;
    }

    public void OnRoomLeave()
    {
        collider.enabled = false;
    }
}
