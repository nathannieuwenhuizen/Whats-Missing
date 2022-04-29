using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    public class IKBossHead : IKBossBone
    {

        public IKBossHead(Transform _transform) : base(_transform)
        {
            transform = _transform;
        }


        public override void UpdatePositionIK(Animator _animator)
        {
            // base.UpdatePositionIK(_animator);
        }
        public override void UpdateRotationIK(Animator _animator)
        {
            
            _animator.SetLookAtWeight(1);
            Transform t = _animator.transform;// _animator.GetBoneTransform(HumanBodyBones.Head);
            Transform t2 = _animator.GetBoneTransform(HumanBodyBones.Head);
            _animator.SetLookAtPosition(Vector3.zero);


            // Transform head = _animator.GetBoneTransform(HumanBodyBones.Head);
            // Vector3 forward = (IKLookPosition - head.position).normalized;
            // Vector3 up = Vector3.Cross(forward, t.right);
            // Quaternion rotation = Quaternion.Inverse(t.rotation) * Quaternion.LookRotation(forward, up);
            // _animator.SetBoneLocalRotation(HumanBodyBones.Head, rotation);
        }
    }

}