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
            Hit(.7f);
            // base.OnAreaEnter(player);
        }

        public void Hit(float damage) {
            if (Player.INVINCIBLE == false) {
            AudioHandler.Instance?.PlaySound(SFXFiles.boss_attack_hit_player);
            OnHit?.Invoke(damage);
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
