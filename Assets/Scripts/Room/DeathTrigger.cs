using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeathTrigger : AreaTrigger, IRoomObject
{
    public bool InSpace { get; set; }
    private BoxCollider boxCollider;
    private void Awake() {
        boxCollider = GetComponent<BoxCollider>();
    }
    public override void OnAreaEnter(Player player)
    {
        base.OnAreaEnter(player);

        player.Die(false);
    }

    public void OnRoomEnter()
    {
        boxCollider.enabled = true;
    }

    public void OnRoomLeave()
    {
        boxCollider.enabled = false;
    }
}
