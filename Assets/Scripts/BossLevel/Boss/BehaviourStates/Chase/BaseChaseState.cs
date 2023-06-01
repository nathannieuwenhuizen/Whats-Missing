using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    ///<summary>
    /// The basic chase phase. Three other chase states overrides from this one, depending on which movmenet behaviour is enabled.
    ///</summary>
    public class BaseChaseState : BaseBossState
    {
        public delegate void AttemptToHitShiled();
        public static AttemptToHitShiled AttemptingToHitShield;
        public static bool LAST_CHASE = false;


        public override void DrawDebug()
        {
            base.DrawDebug();
        }
        private float oldViewAngle;
        private float oldViewAlpha;
        private float sharpViewAngle = 15f;

        ///<summary>
        /// If the boss is attacking or not.
        ///</summary>
        private bool isAttacking = false;

        public override void Start()
        {
            bossAI.BossHead.SteeringEnabled = false;

            //head will look at the player
            Boss.Head.LookAtPlayer = true;

            //setting some values
            oldViewAngle = bossAI.BossEye.ViewAngle;
            oldViewAlpha = bossAI.BossEye.ViewAlpha;

            //animate the view angle
            bossAI.BossEye.AnimateViewAngle(sharpViewAngle);
        }


        ///<summary>
        /// Returns the distance from the boss to the player in units. 
        /// Used when to stop moving or if the boss should go in the chrage at shield phase.
        ///</summary>
        public float DistnaceToPlayer() {
            return Vector3.Distance(bossAI.transform.position, bossAI.Boss.Player.transform.position);
        }


        public override void Run()
        {
            //looks always ot the player
            bossAI.BossHead.SetAim(bossAI.Boss.Player.Camera.transform.position, Vector2.zero);
           
            bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
            if (bossAI.BossEye.DoesntNoticesPlayer) {
                // StopChase();
            }


            if (DistnaceToPlayer() < Boss.BOSS_MELEE_ATTACK_RANGE) MeleeAttack();

            
            // if the player is inside the shield
            if (Player.INVINCIBLE && !LAST_CHASE)
            {
                //get distance of player to boss
                float dist = DistnaceToPlayer();

                //go to charge at shield phase is the player is lcose enough
                if (dist < Boss.CHARGE_AT_SHIELD_THRESHHOLD && (isAttacking == false)) {
                    OnStateSwitch?.Invoke(bossAI.Behaviours.chargeAtShieldState);
                } else {
                    //if the playwr is too far, then stop the chase.
                    StopChase();
                }
            }
        }

        ///<summary>
        /// Stops the chase, gonig to the wander phase of the ai
        ///</summary>
        protected void StopChase() {
            bossAI.Behaviours.takeoffState.withCutscene = false;
            bossAI.Behaviours.takeoffState.nextState = bossAI.Behaviours.wanderState;
            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }

        public override void Exit()
        {
            bossAI.BossHead.SteeringEnabled = true;

            bossAI.BossEye.AnimateViewAngle(oldViewAngle);
            bossAI.BossEye.noticingValue = 0;
        }


        ///<summary>
        /// Do melee attack if the boss hasn't already attacked the player.
        ///</summary>
        protected void MeleeAttack() {
            if (isAttacking) return;
            isAttacking = true;
            bossAI.StartCoroutine(Attacking());
        }

        ///<summary>
        /// Coroutine of hte whol attack animation, after the animation has been done, the boss goes back to its old orientation.
        ///</summary>
        protected virtual IEnumerator Attacking() {
            BodyOrientation oldOrientation = Positioner.BodyOrientation;
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            AudioHandler.Instance?.Play3DSound(SFXFiles.boss_attack, bossAI.BossHead.transform);

            yield return bossAI.StartCoroutine(Boss.Body.BossAnimator.DoAttackAnimation());
            Positioner.BodyOrientation = oldOrientation;

            isAttacking = false;
        }

        ///<summary>
        /// Do melee attack if the boss hasn't already attacked the shield.
        ///</summary>
        protected void MeleeAttackAtShield() {
            if (isAttacking) return;
            isAttacking = true;
            AttemptingToHitShield?.Invoke();
            bossAI.StartCoroutine(AttackingShield());
        }

        ///<summary>
        /// Coroutine of hte whol attack animation, after the animation has been done, the boss goes back to its old orientation.
        ///</summary>
        protected virtual IEnumerator AttackingShield() {
            BodyOrientation oldOrientation = Positioner.BodyOrientation;
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            AudioHandler.Instance?.Play3DSound(SFXFiles.boss_attack, bossAI.BossHead.transform);

            yield return bossAI.StartCoroutine(Boss.Body.BossAnimator.DoFailedAttackAnimation());
            Positioner.BodyOrientation = oldOrientation;

            isAttacking = false;
        }
    }

}