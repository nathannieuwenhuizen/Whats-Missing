using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    ///<summary>
    /// Lits of all the animation parameters the boss has
    ///</summary>
    public static class BossAnimatorParam{
        //states and triggers
        public static readonly string TRIGGER_INTRO = "intro";
        public static readonly string TRIGGER_ATTACK = "attack";
        public static readonly string TRIGGER_TRANSFORM = "transform";
        public static readonly string TRIGGER_DEATH = "death";
        public static readonly string TRIGGER_MIRROR_ATTACK = "mirror_attack";

        public static readonly string BOOL_INAIR = "inAir";

        public static readonly string FLOAT_CRAWLSPEED = "crawl_speed";
        //inverse kinematics
        public static readonly string FLOAT_IK_LF = "ik_lf";
        public static readonly string FLOAT_IK_RF = "ik_rf";
        public static readonly string FLOAT_IK_LH = "ik_lh";
        public static readonly string FLOAT_IK_RH = "ik_rh";
        public static readonly string FLOAT_ATTACKWEIGHT = "attackWeight";
    }
    public class BossAnimator
    {
        private Animator animator;
        private IKBossPass IKPass;
        private Boss boss;
        public Boss Boss {
            get { return boss;}
        }

        public BossAnimator(Boss _boss, IKBossPass _IKPass) {
            boss = _boss;
            IKPass =_IKPass;
            animator = _IKPass.Animator;
        }

        public Animator Animator {
            get { return animator;}
        }

        ///<summary>
        /// Sets a trigger of the animator
        ///</summary>
        public void SetTrigger(string _triggerkey, bool _applyRootMotion = true) {
            animator.SetTrigger(_triggerkey);
            animator.applyRootMotion = _applyRootMotion;
        }

        ///<summary>
        /// Sets the boolean of the animator
        ///</summary>
        public void SetBool(string _key, bool _val) {
            animator.SetBool(_key, _val);
        }
        ///<summary>
        /// Sets the boolean of the animator
        ///</summary>
        public void SetFloat(string _key, float _val) {
            animator.SetFloat(_key, Mathf.Max(0,_val));
        }

        ///<summary>
        /// Sets the float of the animator (cant go lower htan 0 though)
        ///</summary>
        public void SetInt(string _key, int _val) {
            animator.SetInteger(_key, _val);
        }

        public IEnumerator DoTriggerAnimation(string _triggerKey, bool _applyRootMotion, float _delayBeforeCallback, Action _callback) {
            SetTrigger(_triggerKey, _applyRootMotion);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(_delayBeforeCallback);
            _callback();
        }

        private bool attacking = false;
        public bool Attacking {
            get { return attacking;}
        }


        public IEnumerator DoAttackAnimation() {
            attacking = true;
            SetTrigger(BossAnimatorParam.TRIGGER_ATTACK);
            yield return new WaitForFixedUpdate();
            boss.StartCoroutine(boss.Body.Arm.UpdatingArmFX(this));
            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;

            // Debug.Log("animation clip lenght: " + clipLength);
            IKPass.RightArm.EnableRayCast = false;
            float index = 0;
            while (index < 3f) {
                index += Time.deltaTime;
                IKPass.RightArm.IKPosition = boss.Player.transform.position;
                IKPass.RightArm.Weight = animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
                boss.Body.ToggleDeathColliders(IKPass.RightArm.Weight > 0);
                yield return new WaitForFixedUpdate();
            }
            attacking = false;
        }
        ///<summary>
        /// Does an animation and when the 
        ///</summary>
        public IEnumerator DoMirrorAttack(Action mirrorAttack, Action endCallback) {
            SetTrigger(BossAnimatorParam.TRIGGER_MIRROR_ATTACK);
            yield return new WaitForSeconds(1.6f);
            mirrorAttack();
            yield return new WaitForSeconds(1.3f);
            endCallback();
        }
    }
}