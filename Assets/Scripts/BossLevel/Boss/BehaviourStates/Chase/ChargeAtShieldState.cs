using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {
    ///<summary>
    /// State that makes the boss attacks the shield only once and then goes bakc into the wnader state. 
    /// Gives the boss a bit more anger.
    ///</summary>
    public class ChargeAtShieldState : BaseChaseState
        {
            private Vector3 chargePos;

            public override void DrawDebug()
            {
                base.DrawDebug();
            }

            public override void Start()
            {
                stateName = "Charge at shield";

                base.Start();
                Positioner.BodyOrientation = BodyOrientation.toPath;

                chargePos = Boss.Forcefield.EdgePosition(Boss.Player.transform.position);
                Positioner.SetDestinationPath(chargePos, Boss.transform.position);

                bossAI.StartCoroutine(GoToShield());
                // bossAI.StartCoroutine(Extensions.AnimateCallBack(1f, 3f, AnimationCurve.EaseInOut(0,0,1,1), (float _val) => {
                //     positioner.SpeedScale = _val;
                // }, 2f));
            }

            public override void Run()
            {
                //no base call
            }

            public override void Exit()
            {
                Positioner.SpeedScale = 1f;
                base.Exit();
            }

            ///<summary>
            /// Goes to the edgeo f the siheld and hten performs an attack.
            ///</summary>
            public IEnumerator GoToShield() {

                //go to air position of the charge at shield if the boss is in the air steering mode.
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

                Positioner.SetDestinationPath(chargePos, Boss.transform.position);
                
                Positioner.SpeedScale = 1f;
                while (!Positioner.AtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) {
                    yield return new WaitForFixedUpdate();
                }
                MeleeAttack();

            }

            protected override IEnumerator Attacking()
            {
                //wait for attacking
                yield return base.Attacking();
                //go back to wandering state.
                Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
                StopChase();
            }
    }
}