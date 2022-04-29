using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    [System.Serializable]
    public class IKBossHead : IKBossBone
    {

        [SerializeField]
        private Transform test;

        public IKBossHead(Transform _transform, Transform _test) : base(_transform)
        {
            transform = _transform;
            test = _test;
        }


        public override void UpdatePositionIK(Animator _animator)
        {
            base.UpdatePositionIK(_animator);
        }
        public override void UpdateRotationIK(Animator _animator)
        {
            // _animator.SetLookAtWeight(1);
            Transform t = _animator.transform;// _animator.GetBoneTransform(HumanBodyBones.Head);
            Transform t2 = _animator.GetBoneTransform(HumanBodyBones.Head);
            IKLookDirection = test.position - t2.position;
            // _animator.SetLookAtPosition(IKLookDirection);
            // _animator.SetLookAtPosition(t2.InverseTransformVector(IKLookDirection));
            // _animator.SetLookAtPosition(t.TransformPoint(t2.InverseTransformPoint(IKLookDirection)));

            Quaternion rotation = Quaternion.LookRotation( t2.InverseTransformDirection(-IKLookDirection), Vector3.up);
            _animator.SetBoneLocalRotation(HumanBodyBones.Head, rotation);


            // Transform head = _animator.GetBoneTransform(HumanBodyBones.Head);
            // Vector3 forward = (IKLookPosition - head.position).normalized;
            // Vector3 up = Vector3.Cross(forward, t.right);
            // Quaternion rotation = Quaternion.Inverse(t.rotation) * Quaternion.LookRotation(forward, up);
            // _animator.SetBoneLocalRotation(HumanBodyBones.Head, rotation);
        }
    }

}