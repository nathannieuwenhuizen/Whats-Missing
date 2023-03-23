using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {


    public struct ParentCoord {
        public Transform startParent;
        public Vector3 startPos;
        public Quaternion startRot;
    }
    ///<summary>
    /// An individual arm
    ///</summary>
    [System.Serializable]
    public class ArmRender{
        public Renderer[] mesh;
        public TrailRenderer armTrail;
        [HideInInspector]
        public float startTrailTime;

        public ParentCoord trailCoord;

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
        public ArmRender HumanArm {
            get { return humanArm;}
        }
        [SerializeField]
        private ArmRender sytheArm;
        public ArmRender SytheArm {
            get { return sytheArm;}
        }


        private bool isSythe = false;
        public ArmRender currentArm {
            get => isSythe ? sytheArm : humanArm;
        }

        private float armShine = 0;
        public float Shine {
            get { return armShine;}
            set { 
                armShine = value; 
                foreach(Renderer mesh in humanArm.mesh) mesh.material.SetFloat(BossBody.shineKey, value);
                foreach(Renderer mesh in sytheArm.mesh)  mesh.material.SetFloat(BossBody.shineKey, value);
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
            foreach(Renderer mesh in humanArm.mesh) mesh.gameObject.SetActive(!_toSythe);
            foreach(Renderer mesh in sytheArm.mesh) mesh.gameObject.SetActive(_toSythe);
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
 
        public IEnumerator UpdatingArmFXWithTimeStop(BossAnimator _animator) {
            deattachTrail = false;
            attackWeightIsAtMax = false;

            currentArm.ReattachTrail();
            currentArm.armTrail.enabled = true;
            StartCoroutine(Extensions.AnimateCallBack(.5f, 0f, AnimationCurve.Linear(0,0,1,1), (float v) => {
                Debug.Log("change arm speed " + v);
                currentArm.armTrail.material.SetVector("_MainTexSpeed", new Vector4(v,0,0,0));
                currentArm.armTrail.materials[0].SetVector("_MainTexSpeed", new Vector4(v,0,0,0));
            }, 2f));

            while (_animator.Attacking) {
                float attackWeight = _animator.Animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
                currentArm.armTrail.time = Mathf.Infinity;
                currentArm.armTrail.emitting = true;
                currentArm.armParitcles.enableEmission = true;// attackWeight > 0.1f;
                yield return new WaitForEndOfFrame();
            }
            currentArm.armTrail.enabled = false;
        }
 

        public void UpdateArmFX(BossAnimator bossAnimator) {
            float attackWeight = bossAnimator.Animator.GetFloat(BossAnimatorParam.FLOAT_ATTACKWEIGHT);
            currentArm.armTrail.time = attackWeight * currentArm.startTrailTime;
            currentArm.armTrail.enabled = attackWeight > 0;
            currentArm.armParitcles.enableEmission = attackWeight > 0.1f;
        }
    }
}