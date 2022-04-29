using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

[RequireComponent(typeof(Animator))]
    public class IKBossPass : MonoBehaviour
    {
        private Animator animator;
        [SerializeField]
        private IKBossBone headBone;
        public Animator Animator { get => animator; set => animator = value; }

        [SerializeField]
        private Transform test;

        private IKBossHead ikBossHead;
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

        void Awake () 
        {
            animator = GetComponent<Animator>();
            ikBossHead = new IKBossHead(transform);
            leftLeg = new IKBossLeg(transform, AvatarIKGoal.LeftFoot);
            rightLeg = new IKBossLeg(transform, AvatarIKGoal.RightFoot);
        }

        private void OnEnable() {
            IKPass.OnIKUpdate += OnAnimatorIK;
        }

        private void OnDisable() {
            IKPass.OnIKUpdate -= OnAnimatorIK;
            
        }

        private void LateUpdate() {
            // OnAnimatorIK();
        }


        void OnAnimatorIK() {
            if(animator == null) return;

            leftLeg.UpdateIK(animator);
            rightLeg.UpdateIK(animator);
            // headBone.UpdateIK(animator);

            // ikBossHead.IKLookPosition = test.position;
            // ikBossHead.UpdateIK(animator);

            // animator.SetLookAtWeight(1, 0, 1, 0);
            // animator.SetLookAtPosition(test.position);

            // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            // animator.SetIKPosition(AvatarIKGoal.LeftFoot, test.position); 
        }

        private void OnDrawGizmos() {
            if (leftLeg != null ) {
                leftLeg.OnDrawGizmos();
            }
        }
        
    }

}