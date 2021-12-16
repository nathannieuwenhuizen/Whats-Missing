using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIsland : RoomObject, ITriggerArea
{
    public bool InsideArea { get; set; }
    private Transform oldParent;

    private void Reset() {
        Word = "ground";
        AlternativeWords = new string[] { "island" };
    }

    public void OnAreaEnter(Player player)
    {
        if (oldParent == transform) return;
        
        oldParent = player.transform.parent;
        player.transform.SetParent(transform);
    }

    public void OnAreaExit(Player player)
    {
        player.transform.SetParent(oldParent);
    }
}
