using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {


    public struct StartCoord {
        public Transform startParent;
        public Vector3 startPos;
        public Quaternion startRot;
    }
    ///<summary>
    /// An induvidual arm
    ///</summary>
    [System.Serializable]
    public class ArmRender{
        public Renderer mesh;
        public TrailRenderer armTrail;
        [HideInInspector]
        public float startTrailTime;

        public StartCoord trailCoord;

        public ParticleSystem armParitcles;


        public void Setup() {
            startTrailTime = armTrail.time;
            armTrail.time = 0;
            armTrail.emitting = false;
            armParitcles.enableEmission = false;

            trailCoord.startParent = armTrail.transform.parent;
            trailCoord.startPos = armTrail.transform.localPosition;
            trailCoord.startRot = armTrail.transform.localRotation;
        }

        public void DeattachTrail(Transform newParent) {
            armTrail.transform.parent = newParent;
        }
        public void ReattachTrail() {
            armTrail.transform.parent = trailCoord.startParent;
            armTrail.transform.localPosition = trailCoord.startPos;
            armTrail.transform.localRotation = trailCoord.startRot;
        }

    }
    ///<summary>
    /// Responsible for the arm particles and toggling of the mesh.
    ///</summary>
    public class BossArms : MonoBehaviour
    {
        [SerializeField]
        private ArmRender humanArm;
        [SerializeField]
        private ArmRender sytheArm;

        private bool isSythe = false;
        public ArmRender currentArm {
            get => isSythe ? sytheArm : humanArm;
        }

        private float armShine = 0;
        public float Shine {
            get { return armShine;}
            set { 
                armShine = value; 
                humanArm.mesh.material.SetFloat(BossBody.shineKey, value);
                sytheArm.mesh.material.SetFloat(BossBody.shineKey, value);
            }
        }


        private void Awake() {
            humanArm.Setup();
            sytheArm.Setup();
        }

        ///<summary>
        /// Toggles/updates the arm renders. Mainly used for the metamorphosedPhase
        ///</summary>
        public void Toggle(bool _toSythe) {
            humanArm.mesh.gameObject.SetActive(!_toSythe);
            sytheArm.mesh.gameObject.SetActive(_toSythe);
            isSythe = _toSythe;
        }

        private bool deattachTrail = false;
        private bool attackWeightIsAtMax = false;

        public IEnumerator UpdatingArmFX(BossAnimator _animator) {
            deattachTrail = false;
            attackWeightIsAtMax = false;

            currentArm.ReattachTrail();

            while (_animator.Attacking) {
                float attackWeight = _animator.Animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
                if (attackWeight > .9f) attackWeightIsAtMax = true;
                
                if (attackWeightIsAtMax && attackWeight < .9f && !deattachTrail) {
                    deattachTrail = true;
                    currentArm.armTrail.time = currentArm.startTrailTime;
                    currentArm.armTrail.emitting = false;
                    currentArm.DeattachTrail(_animator.Boss.transform.parent);
                    Debug.Log("deattach");

                }

                if (!deattachTrail) {
                    currentArm.armTrail.time = attackWeight * currentArm.startTrailTime;
                    currentArm.armTrail.enabled = attackWeight > 0;
                    currentArm.armTrail.emitting = attackWeight > 0;

                }
                currentArm.armParitcles.enableEmission = attackWeight > 0.1f;

                yield return new WaitForEndOfFrame();
            }

        }

        public void UpdateArmFX(BossAnimator bossAnimator) {
            float attackWeight = bossAnimator.Animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
            currentArm.armTrail.time = attackWeight * currentArm.startTrailTime;
            currentArm.armTrail.enabled = attackWeight > 0;
            currentArm.armParitcles.enableEmission = attackWeight > 0.1f;
        }
    }
}