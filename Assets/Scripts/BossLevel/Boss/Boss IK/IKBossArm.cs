using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// The ik of the boss's arms
    ///</summary>
    [System.Serializable]
    public class IKBossArm : IKBossLimb
    {
        private float armOffset = 1.5f;
        public IKBossArm(Transform _transform, AvatarIKGoal _ikGoal) : base(_transform, _ikGoal)
        {
            transform = _transform;
            ikGoal = _ikGoal;
        }
        private bool isSyth = true;

        public bool IsSyth {
            get { return isSyth;}
            set { isSyth = value; }
        }

        public override IKLimbInfo LimbInfo {
            get {
                if (isSyth) return new IKLimbInfo() {RayDistance = 15f, limbOffset = 3f, distanceBetweenContacts = 5f};
                else        return new IKLimbInfo() {RayDistance = 12f, limbOffset = .5f, distanceBetweenContacts = 3f};
            }
        } 
        
        public override Vector3 RayCastDirection() {
            if (transform != null) return transform.forward;
            return base.RayCastDirection();
        }

        public override Vector3 RayCastPosition()
        {
            if (boneTransform != null)
            return boneTransform.position + transform.right * (ikGoal == AvatarIKGoal.LeftHand ? armOffset : -armOffset);
            return Vector3.zero;
        }

    }
}
