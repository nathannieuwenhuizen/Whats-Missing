using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {

public class ChargeAtShieldState : BaseChaseState
    {
        private BossPositioner positioner;
        private Vector3 chargePos;

        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        public override void Start()
        {
            stateName = "Charge at shield";

            base.Start();
            positioner = bossAI.Boss.BossPositioner;
            positioner.BodyOrientation = BodyOrientation.toPath;

            chargePos = Boss.Forcefield.EdgePosition(Boss.Player.transform.position);
            positioner.SetDestinationPath(chargePos, Boss.transform.position);

            bossAI.StartCoroutine(GoToShield());
            // bossAI.StartCoroutine(Extensions.AnimateCallBack(1f, 3f, AnimationCurve.EaseInOut(0,0,1,1), (float _val) => {
            //     positioner.SpeedScale = _val;
            // }, 2f));
        }

        public override void Run()
        {
            // base.Run();
            // if (positioner.AtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) MeleeAttack();

        }

        public override void Exit()
        {
            positioner.SpeedScale = 1f;
            base.Exit();
        }

        public IEnumerator GoToShield() {
            //go to air position of the charge at shield
            if (Positioner.BodyMovementType == BodyMovementType.airSteeringAtMountain) {
                Positioner.BodyOrientation = BodyOrientation.toPlayer;
                Positioner.RotationEnabled = true;
                Positioner.MovementEnabled = true;
                Positioner.SetDestinationPath(bossAI.ChargePosition.position, bossAI.transform.position, true, 10f);
                Positioner.SpeedScale = 2f;
                Positioner.SteeringBehaviour.MaxForce *= 10f;

                while(!Positioner.AtPosition(3f)) {
                    yield return new WaitForFixedUpdate();
                }
                Positioner.SteeringBehaviour.MaxForce /= 10f;
            }

            //now land on the shield position
            chargePos = Boss.Forcefield.EdgePosition(bossAI.ChargePosition.position);
            Positioner.BodyMovementType = BodyMovementType.freeFloat;

            positioner.SetDestinationPath(chargePos, Boss.transform.position);
            
            Positioner.SpeedScale = 1f;
            while (!positioner.AtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) {
                yield return new WaitForFixedUpdate();
            }
            MeleeAttack();

        }


        protected override IEnumerator Attacking()
        {
            yield return base.Attacking();
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            StopChase();
        }
}
}