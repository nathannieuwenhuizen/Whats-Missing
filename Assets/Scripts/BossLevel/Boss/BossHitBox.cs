using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Boss {
    public class BossHitBox : DeathTrigger
    {
        public override void OnAreaEnter(Player player)
        {
            if (BossAI.PlayerIsInForceField == false)
                base.OnAreaEnter(player);
        }
        public override void OnRoomEnter()
        {
            // base.OnRoomEnter();
        }
        public override void OnRoomLeave()
        {
            // base.OnRoomLeave();
        }
    }
}
