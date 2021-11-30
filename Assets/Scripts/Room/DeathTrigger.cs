using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : AreaTrigger
{
    public override void OnAreaEnter(Player player)
    {
        base.OnAreaEnter(player);

        player.Die(false);
    }
}
