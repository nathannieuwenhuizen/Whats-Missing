using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Boss {
    public class BossHitBox : DeathTrigger
    {
        public delegate void HitboxEvent(float _val);
        public static HitboxEvent OnHit;

        public override void OnAreaEnter(Player player)
        {
            if (Player.INVINCIBLE == false) {
                Debug.LogWarning("[PLAYER IS DEAD]");
                Debug.Log("[PLAYER IS DEAD]");
                AudioHandler.Instance?.PlaySound(SFXFiles.boss_attack_hit_player);
                OnHit?.Invoke(.7f);
                // base.OnAreaEnter(player);
            }
        }

        private void Update() {
            // if (Input.GetKeyDown(KeyCode.L)) {
            //     OnHit?.Invoke(.8f);
            // }
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
