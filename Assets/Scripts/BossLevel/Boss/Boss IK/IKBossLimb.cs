using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Boss {

    public struct IKLimbInfo {
        public float RayDistance;
        public float limbOffset;
        public float distanceBetweenContacts;
    }

    public interface IIKBossLimb {
        public Vector3 RayCastDirection();
        public Vector3 RayCastPosition();
        public RaycastHit ContactInfo {get;}
        public IKLimbInfo LimbInfo {get;}
    }

    [System.Serializable]
    public abstract class IKBossLimb : IKBossBone, IIKBossLimb
    {
        private RaycastHit contactInfo;

        private bool hasContact = false;

        private bool enableRayCast = false;
        public bool EnableRayCast {
            get { return enableRayCast;}
            set {
                enableRayCast = value;
            }
        }

        public RaycastHit ContactInfo => contactInfo;


        public virtual IKLimbInfo LimbInfo => new IKLimbInfo() {RayDistance = 5f, limbOffset = 1.8f, distanceBetweenContacts = 3f};
        

        public IKBossLimb(Transform _transform, IKBossPass _ikPass, AvatarIKGoal _ikGoal) : base(_transform, _ikPass)
        {
            transform = _transform;
            ikGoal = _ikGoal;
            boneTransform = SetBoneTransform(_ikPass.Animator); 
        }


        public override void UpdateIK(Animator _animator)
        {
            if (EnableRayCast) {
                hasContact = CastRay();
                if (hasContact) {
                    if (Vector3.Distance(IKPosition, GetContactPosition()) > LimbInfo.distanceBetweenContacts) {
                        IKPosition = GetContactPosition();
                        IKLookDirection = contactInfo.normal;
                    }
                }
            }
            base.UpdateIK(_animator);
        }

        private Vector3 GetContactPosition() {
            return contactInfo.point + contactInfo.normal * LimbInfo.limbOffset;
        }

        public bool CastRay() {
            int layer = 11;
            int layerMask = 1 << layer;

            if (Physics.Raycast(RayCastPosition(), RayCastDirection(), out contactInfo, LimbInfo.RayDistance)) {
                return true;
            }
            contactInfo = default(RaycastHit);
            return false;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (EnableRayCast) {
                if (hasContact) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(RayCastPosition(), .1f);
                    Gizmos.DrawSphere(GetContactPosition(), .1f);
                    Debug.DrawLine(RayCastPosition(), GetContactPosition(), Color.green);
                    // Debug.DrawLine(contactInfo.point, GetContactPosition(), Color.green);
                }
                else {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(RayCastPosition(), .1f);
                    Debug.DrawLine(RayCastPosition(), RayCastPosition() + RayCastDirection().normalized * LimbInfo.RayDistance, Color.red);
                }
            }
        }

        public virtual Vector3 RayCastPosition()
        {
            if (boneTransform != null)
            return boneTransform.position;

            return Vector3.zero;
        }

        public virtual Vector3 RayCastDirection()
        {
            return Vector3.down;
        }

    }
}