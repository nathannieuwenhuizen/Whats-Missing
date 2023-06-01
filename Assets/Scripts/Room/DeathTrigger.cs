using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : AreaTrigger, IRoomObject
{
    public bool InSpace { get; set; }
    private Collider coll;
    public Collider Coll {
        get { 
            if (coll == null) coll = GetComponent<Collider>();
            return coll;
        }
    }
    private void Awake() {
        coll = GetComponent<Collider>();
    }
    public override void OnAreaEnter(Player player)
    {
        base.OnAreaEnter(player);
        player.Die(false);
    }

    public virtual void OnRoomEnter()
    {
        coll.enabled = true;
    }

    public virtual void OnRoomLeave()
    {
        coll.enabled = false;
    }
}
