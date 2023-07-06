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
        private Coroutine goToShieldCoroutine;
        private Coroutine knockbackCoroutine;

        
        public override void DrawDebug()
        {
            base.DrawDebug();
        }
        

        public override void Start()
        {
            stateName = "Charge at shield";

            base.Start();
            Debug.Log("Charge at shield state now!");
            // Positioner.BodyOrientation = BodyOrientation.toPath;

            chargePos = Boss.Forcefield.EdgePosition(Boss.Player.transform.position);
            Positioner.SetDestinationPath(chargePos, Boss.transform.position);

            goToShieldCoroutine = bossAI.StartCoroutine(GoToShield());
            ForcefieldDemo.Forcefield.OnForceFieldImpact += Knockback;
        }

        public override void Run()
        {
            //no base call
        }

        public override void Exit()
        {
            ForcefieldDemo.Forcefield.OnForceFieldImpact -= Knockback;
            Positioner.SpeedScale = 1f;
            if (goToShieldCoroutine != null) bossAI.StopCoroutine(goToShieldCoroutine);
            if (knockbackCoroutine != null) bossAI.StopCoroutine(knockbackCoroutine);

            base.Exit();
        }

        ///<summary>
        /// Goes to the edgeo f the siheld and hten performs an attack.
        ///</summary>
        public IEnumerator GoToShield() {
            Positioner.RotationEnabled = true;
            Positioner.MovementEnabled = true;

            //go to air position of the charge at shield if the boss is in the air steering mode.
            if (Positioner.BodyMovementType == BodyMovementType.airSteeringAtMountain) {
                Positioner.BodyOrientation = BodyOrientation.toPlayer;
                Positioner.SetDestinationPath(bossAI.ChargePosition.position, bossAI.transform.position, true, 10f);
                Positioner.SpeedScale = 2f;
                Positioner.SteeringBehaviour.MaxForce *= 10f;

                while(!Positioner.AtPosition(3f)) {
                    yield return new WaitForFixedUpdate();
                }
                Positioner.SteeringBehaviour.MaxForce /= 10f;
            }

            //now go to the shield edge position
            chargePos = Boss.Forcefield.EdgePosition(bossAI.transform.position, 1f);
            Positioner.BodyMovementType = BodyMovementType.freeFloat;

            Positioner.SetDestinationPath(chargePos, Boss.transform.position);
            
            Positioner.SpeedScale = 1f;
            while (!Positioner.AtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) {
                yield return new WaitForFixedUpdate();
            }
            MeleeAttackAtShield();
        }

        public void Knockback() {
            knockbackCoroutine = bossAI.StartCoroutine(KnockingBack());
        }
        public IEnumerator KnockingBack() {
            chargePos = Boss.Forcefield.EdgePosition(bossAI.transform.position, 8f) + Vector3.up * 5f;
            Positioner.SetDestinationPath(chargePos, Boss.transform.position);
            yield return new WaitForSeconds(2f);
            chargePos.y += 6f;
            Positioner.SpeedScale = .5f;
            Positioner.SetDestinationPath(chargePos, Boss.transform.position);
        }

        protected override IEnumerator AttackingShield()
        {
            //wait for attacking
            yield return base.AttackingShield();
            //go back to wandering state.
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            StopChase();
        }
    }
}