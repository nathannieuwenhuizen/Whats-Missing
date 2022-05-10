using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    [System.Serializable]
    public class IKBossHead : IKBossBone
    {

        [SerializeField]
        private Transform test;
        private float angleCap = 80f;

        public IKBossHead(Transform _transform, IKBossPass _ikPass, Transform _test) : base(_transform, _ikPass)
        {
            transform = _transform;
            test = _test;
        }


        public override void UpdatePositionIK(Animator _animator)
        {
            base.UpdatePositionIK(_animator);
        }
        private bool updateRotation = false;

        public override void UpdateRotationIK(Animator _animator)
        {
            _animator.SetLookAtWeight(Weight, 0, .55f, 0, 0.5f);

            Transform t = _animator.GetBoneTransform(HumanBodyBones.Spine);

            // IKLookDirection = test.position - boneTransform.position;
            Vector3 relativeDelta = t.position - boneTransform.position;

            _animator.SetLookAtPosition(IKLookDirection + relativeDelta);

        }

        public override void OnEnable()
        {
            base.OnEnable();
            BossHead.OnHeadAimUpdate += SetAimPosition;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BossHead.OnHeadAimUpdate -= SetAimPosition;
        }

        private void SetAimPosition(Vector3 _position) {
            Weight = 1;
            IKLookDirection = _position;
        }
    }
}