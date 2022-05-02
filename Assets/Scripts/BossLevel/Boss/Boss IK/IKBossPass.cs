using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

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

            ikBossHead = new IKBossHead(transform, test);

            leftLeg = new IKBossLeg(transform, AvatarIKGoal.LeftFoot);
            rightLeg = new IKBossLeg(transform, AvatarIKGoal.RightFoot);
            leftArm = new IKBossArm(transform, AvatarIKGoal.LeftHand);
            rightArm = new IKBossArm(transform, AvatarIKGoal.RightHand);
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

            // animator.SetLookAtWeight(1, 0, 1, 0);
            // animator.SetLookAtPosition(test.position);

            // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            // animator.SetIKPosition(AvatarIKGoal.LeftFoot, test.position); 
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