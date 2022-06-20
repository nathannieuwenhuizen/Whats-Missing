using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// Responsible for the arm particles and toggling of the mesh.
    ///</summary>
    public class BossArm : MonoBehaviour
    {
        //mesh renderers
        public Renderer arm;
        public Renderer sythe;

        //trail renderers
        [SerializeField]
        private TrailRenderer slashTrailSythe;
        private float slashTrailTimeSythe;
        [SerializeField]
        private TrailRenderer slashTrailArm;
        private float slashTrailTimeArm;

        //particles
        [SerializeField]
        private ParticleSystem slashParticles;


        private bool isSythe = false;

        private void Awake() {
            
        }
        public void Toggle(bool _toSythe) {
            arm.gameObject.SetActive(!_toSythe);
            sythe.gameObject.SetActive(_toSythe);
            isSythe = _toSythe;
        }

        public void UpdateArmFX(BossAnimator bossAnimator) {
            float attackWeight = bossAnimator.Animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
            
            if (isSythe) {
                slashTrailSythe.time = attackWeight * slashTrailTimeSythe;
                slashParticles.enableEmission = attackWeight > 0.1f;
            } else {
                slashTrailArm.time = attackWeight * slashTrailTimeArm;
            }
        }
    }
}