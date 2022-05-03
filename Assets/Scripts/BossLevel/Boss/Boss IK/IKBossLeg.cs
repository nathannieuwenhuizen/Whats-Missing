using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// The IK of the boss's legs
    ///</summary>
    [System.Serializable]
    public class IKBossLeg : IKBossLimb
    {
        public IKBossLeg(Transform _transform, IKBossPass _ikPass, AvatarIKGoal _ikGoal) : base(_transform, _ikPass, _ikGoal)
        {
            transform = _transform;
            ikGoal = _ikGoal;
        }

        public override IKLimbInfo LimbInfo => new IKLimbInfo() {RayDistance = 5f, limbOffset = 1.8f, distanceBetweenContacts = 3f};

        public override Vector3 RayCastDirection() => Vector3.down;
        
    }
}
