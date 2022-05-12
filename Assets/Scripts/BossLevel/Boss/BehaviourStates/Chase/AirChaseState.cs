using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    public class AirChaseState : BaseChaseState
    {
        private Vector3 startChasePos;
        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        public override void Start()
        {
            base.Start();
            Positioner.BodyOrientation = BodyOrientation.toPath;
            Positioner.BodyMovementType = BodyMovementType.freeFloat;
            stateName = "Air Chase Chase";
            Positioner.MovementEnabled = true;
            Positioner.InAir = true;
            UpdateBossChasePath(true);

        }

        ///<summary>
        /// Calculates for the boss, if the reset bool is true, the begin pos will be again set to the boss current position.
        ///</summary>
        private void UpdateBossChasePath(bool _resetBeginPos = false) {
            if (_resetBeginPos) startChasePos = bossAI.transform.position +  Vector3.up * (Boss.BOSS_GROUND_OFFSET);

            Positioner.SetDestinationPath(bossAI.Boss.Player.transform, startChasePos);
            // positioner.SetDestinationPath(bossAI.Boss.Player.transform.position + Vector3.up * (Boss.BOSS_GROUND_OFFSET), startChasePos);
        }

        public override void Run()
        {
            base.Run();
            if (Positioner.AtPosition(Boss.BOSS_ATTACK_PLAYER_RANGE)) Attack();
            
            UpdateBossChasePath(false);
            
            if (BossAI.PlayerIsInForceField)
                OnStateSwitch?.Invoke(bossAI.Behaviours.chargeAtShieldState);
            
        }


        public override void Exit()
        {
            base.Exit();
        }
    }
}