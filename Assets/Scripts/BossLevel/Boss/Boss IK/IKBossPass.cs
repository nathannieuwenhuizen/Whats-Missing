using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    ///<summary>
    /// The globsl ik pass for the boss.
    ///</summary>
    [RequireComponent(typeof(Animator))]
    public class IKBossPass : MonoBehaviour
    {
        private Animator animator;
        public Animator Animator { get => animator; set => animator = value; }

        [SerializeField]
        private Transform test;

        [SerializeField]
        private IKBossHead ikBossHead;
        public IKBossHead IKBossHead {
            get { return ikBossHead;}
        }


        [SerializeField]
        private IKBossLeg leftLeg;
        public IKBossLeg LeftLeg {
            get { return leftLeg;}
        }

        [SerializeField]
        private IKBossLeg rightLeg;
        public IKBossLeg RightLeg {
            get { return RightLeg;}
        }

        [SerializeField]
        private IKBossArm rightArm;
        public IKBossArm RightArm {
            get { return rightArm;}
        }

        [SerializeField]
        private IKBossArm leftArm;
        public IKBossArm LeftArm {
            get { return leftArm;}
        }

        void Awake () 
        {
            animator = GetComponent<Animator>();

            ikBossHead = new IKBossHead(transform, this, test);

            leftLeg = new IKBossLeg(transform, this, AvatarIKGoal.LeftFoot);
            rightLeg = new IKBossLeg(transform, this, AvatarIKGoal.RightFoot);
            leftArm = new IKBossArm(transform, this, AvatarIKGoal.LeftHand);
            rightArm = new IKBossArm(transform, this, AvatarIKGoal.RightHand);
        }

        public void SetLimbsActive(bool _val) {
            rightArm.IsActive = _val;
            leftArm.IsActive = _val;
            
            rightLeg.IsActive = _val;
            leftLeg.IsActive = _val;
        }


        private void OnEnable() {
            IKPass.OnIKUpdate += OnAnimatorIK;
        }

        private void OnDisable() {
            IKPass.OnIKUpdate -= OnAnimatorIK;
            
        }

        private void LateUpdate() {
            // OnAnimatorIK();
            ikBossHead.UpdateIK(animator);
        }


        void OnAnimatorIK() {
            if(animator == null) return;

            leftLeg.UpdateIK(animator);
            rightLeg.UpdateIK(animator);

            leftArm.UpdateIK(animator);
            rightArm.UpdateIK(animator);

            ikBossHead.UpdateIK(animator);
        }

        private void OnDrawGizmos() {
            if (leftLeg != null ) {
                leftLeg.OnDrawGizmos();
                rightLeg.OnDrawGizmos();
                leftArm.OnDrawGizmos();
                rightArm.OnDrawGizmos();
            }
        }
        
    }

}