using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    public class MountainAirChaseState : BaseChaseState
    {
        private Vector3 startChasePos;
        private Vector3 destinationChasePos;
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


            MountainAttackPose.OnPlayerEnteringAttackArea += UpdateDestination;
            UpdateBossChasePath(true);
        }

        public void UpdateDestination(Vector3 _position) {
            destinationChasePos = _position;
            UpdateBossChasePath(false);
            Debug.Log("new chase pos");
        }

        private void UpdateBossChasePath(bool _resetBeginPos = false) {
            if (_resetBeginPos) startChasePos = bossAI.transform.position;
            Positioner.SetDestinationPath(destinationChasePos, startChasePos);
        }
        public override void Run()
        {
            base.Run();
            if (Positioner.AtPosition(Boss.BOSS_MELEE_ATTACK_RANGE)) MeleeAttack();
            UpdateBossChasePath(false);
            
            if (BossAI.PlayerIsInForceField)
                OnStateSwitch?.Invoke(bossAI.Behaviours.chargeAtShieldState);
            
        }


        public override void Exit()
        {
            MountainAttackPose.OnPlayerEnteringAttackArea -= UpdateDestination;
            base.Exit();
        }
    }
}