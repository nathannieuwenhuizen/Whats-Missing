using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    [System.Serializable]
    public class IKBossLeg : IKBossBone
    {
        

        private RaycastHit contactInfo;
        private const float feetRayDistance = 5f;
        private const float feetOffset = 1.8f;
        private bool legAboveGround = false;
        private Vector3 bonePos;
        public IKBossLeg(Transform _transform, AvatarIKGoal _ikGoal) : base(_transform)
        {
            transform = _transform;
            ikGoal = _ikGoal;
        }

        public override void UpdateIK(Animator _animator)
        {
            boneTransform = _animator.GetBoneTransform(ikGoal == AvatarIKGoal.LeftFoot ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
            bonePos = boneTransform.position;

            legAboveGround = CastRay();
            if (legAboveGround) {
                IKPosition = FeetPos();
                IKLookDirection = contactInfo.normal;
            }
            base.UpdateIK(_animator);
        }

        private Vector3 FeetPos() {
            return contactInfo.point + contactInfo.normal * feetOffset;
        }

        public bool CastRay() {
            int layer = 11;
            int layerMask = 1 << layer;

            if (Physics.Raycast(boneTransform.position, Vector3.down, out contactInfo, feetRayDistance)) {
                return true;
            }
            contactInfo = default(RaycastHit);
            return false;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (legAboveGround) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(bonePos, .1f);
                Gizmos.DrawSphere(FeetPos(), .1f);
                Debug.DrawLine(bonePos, FeetPos(), Color.green);
                Debug.DrawLine(contactInfo.point, FeetPos(), Color.green);
            }
            else {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(bonePos, .1f);
                Debug.DrawLine(bonePos, bonePos + Vector3.down * feetRayDistance, Color.red);

            }
        }
    }
}
