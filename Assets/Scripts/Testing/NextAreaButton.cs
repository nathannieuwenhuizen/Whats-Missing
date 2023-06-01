using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// Used for testing.
///</summary>
public class NextAreaButton : InteractabelObject
{
    [SerializeField]
    private Room room;
    [SerializeField]
    private BossRoom bossLevel;
    public override void Interact()
    {
        base.Interact();
        if (room != null) room.Area.EndOfArea();
        if (bossLevel != null) bossLevel.EndOfArea();
    }
}
