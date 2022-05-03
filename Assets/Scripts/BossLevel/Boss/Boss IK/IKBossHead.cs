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
            // updateRotation = !updateRotation;
            // if (!updateRotation) return;

            Transform t = _animator.GetBoneTransform(HumanBodyBones.Spine);// _animator.GetBoneTransform(HumanBodyBones.Head);
            Transform t2 = _animator.GetBoneTransform(HumanBodyBones.Head);
            Quaternion animatinRotation = Quaternion.Euler( new Vector3(t2.localEulerAngles.x, t2.localEulerAngles.y, t2.localEulerAngles.z));;

            _animator.SetLookAtWeight(Weight, 0, .55f, 0, 0.5f);
            IKLookDirection = test.position - t2.position;

            Vector3 relativeDelta = t.position - t2.position;
            _animator.SetLookAtPosition(test.position - relativeDelta);

        }
    }
}