using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Boss {
    public class BossHitBox : DeathTrigger
    {
        public override void OnAreaEnter(Player player)
        {
            if (Player.INVINCIBLE == false) {
                AudioHandler.Instance?.PlaySound(SFXFiles.boss_attack_hit_player);
                // base.OnAreaEnter(player);
            }
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
