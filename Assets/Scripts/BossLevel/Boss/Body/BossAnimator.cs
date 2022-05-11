using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    public class BossAnimator
    {
        private Animator animator;
        private IKBossPass IKPass;
        private Boss boss;

        public BossAnimator(Boss _boss, IKBossPass _IKPass) {
            boss = _boss;
            IKPass =_IKPass;
            animator = _IKPass.Animator;
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
        public void SetInt(string _key, int _val) {
            animator.SetInteger(_key, _val);
        }

        public IEnumerator DoAttackAnimation() {
            SetTrigger("attack");
            yield return new WaitForFixedUpdate();
            Debug.Log("animation clip lenght: " + animator.GetCurrentAnimatorStateInfo(0).length);
            IKPass.RightArm.EnableRayCast = false;
            float index = 0;
            while (index < 3f) {
                index += Time.deltaTime;
                IKPass.RightArm.IKPosition = boss.Player.transform.position;
                IKPass.RightArm.Weight = animator.GetFloat("attackWeight");
                boss.Body.ToggleDeathColliders(IKPass.RightArm.Weight > 0);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}