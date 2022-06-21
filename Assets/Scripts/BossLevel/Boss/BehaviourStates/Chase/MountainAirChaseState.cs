using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    public class MountainAirChaseState : BaseChaseState
    {
        private MountainAttackPose oldAttackPose;

        private bool withPathOffset = true;
        private Vector3 startChasePos;
        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        public override void Start()
        {
            base.Start();
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            stateName = "Mountain Air Chase Chase";
            Positioner.MovementEnabled = true;
            Positioner.InAir = true;
            Positioner.SpeedScale = 2f;


            MountainAttackPose.OnPlayerEnteringAttackArea += UpdateDestination;
            UpdateBossChasePath(true);
        }

        public void UpdateDestination(MountainAttackPose _pose) {
            if (oldAttackPose != _pose) {
                withPathOffset = true;
                UpdateBossChasePath(true);
                Positioner.SpeedScale = 2f;
            }
            oldAttackPose = _pose;
        }


        private void UpdateBossChasePath(bool _resetBeginPos = false) {
            if (_resetBeginPos) startChasePos = bossAI.transform.position;
            Positioner.SetDestinationPath(bossAI.MountainAttackPosition.BeginPosition, startChasePos, withPathOffset);
        }
        public override void Run()
        {
            base.Run();
            //check to do melee attack.
            if (Vector3.Distance(bossAI.transform.position, bossAI.Boss.Player.transform.position) < Boss.BOSS_MELEE_ATTACK_RANGE) MeleeAttack();
            
            //go to charge at shield phase from here
            if (Player.INVINCIBLE)
                OnStateSwitch?.Invoke(bossAI.Behaviours.chargeAtShieldState);
            
            //move towards the player
            if (withPathOffset) {
                if (Positioner.AtPosition(.5f)) {
                    withPathOffset = false;
                    startChasePos = bossAI.transform.position;
                }
            } else {
                GoAsCloseToPlayer();
                // UpdateBossChasePath(true);
            }
        }

        private void GoAsCloseToPlayer() {
            Positioner.SpeedScale = 1f;
            // if (!Positioner.AtPosition(.5f)) startChasePos = bossAI.transform.position;
            Positioner.SetDestinationPath(bossAI.MountainAttackPosition.PosClosestToPlayerButWithinRange(Positioner.BossMountain, bossAI.Boss.Player), startChasePos, false);
        }



        public override void Exit()
        {
            Positioner.SpeedScale = 1f;
            MountainAttackPose.OnPlayerEnteringAttackArea -= UpdateDestination;
            base.Exit();
        }
    }
}