using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// This handles the bossy body mesh. 
    /// It (de)activates and handles he secondary physics, purely the astethics
    ///</summary>
    public class BossBody : MonoBehaviour
    {
        //metamorphose values
        private bool hasMetamorphosed = false;
        private float metamorphoseDuration = 2f;
        private float metamorphoseDelay = 0f;

        //shader keys
        private readonly string growKey = "_grow";
        public static readonly string shineKey = "_shine_power";
        private readonly string glowKey = "_glow_power";
        private readonly string dissolveKey = "Dissolve";
        private readonly string disolveEdgeWidthKey = "EdgeWidth";

        //renders
        [SerializeField]
        private Renderer[] bodyRenders;
        [SerializeField]
        private Renderer[] tentacleRenderers;

        [SerializeField]
        private BossArms armRenders;
        public BossArms Arm {
            get { return armRenders;}
        }

        [SerializeField]
        private BossHitBox[] hitBoxes;

        private BossAnimator bossAnimator;
        public BossAnimator BossAnimator {
            get { return bossAnimator;}
        }

        [SerializeField]
        private SkinnedMeshToMesh[] spores;
        

        [SerializeField]
        private BossWiggleBone[] mainBossWiggleBones;
        [SerializeField]
        private BossWiggleBone[] metamorphosedBones;
        
        [SerializeField]
        private IKBossPass ikBossPass;
        public IKBossPass IKPass {
            get { return ikBossPass;}
        }
        
        [SerializeField]
        private MeshRenderer kickedShardRenderer;
        public MeshRenderer KickedShardRenderer {
            get { return kickedShardRenderer;}
        }

        [HideInInspector]
        public Boss boss;

        private void Start() {
            bossAnimator = new BossAnimator(boss, IKPass);
            ToggleDeathColliders(false);
            TentacleGrowth = 0;
            Arm.Toggle(false);
            kickedShardRenderer.enabled = false;
        }

        private void Update() {
            // if (Input.GetKeyDown(KeyCode.L)) {
            //     Arm.Toggle(true);
            //     TentacleGrowth = 1;
            // }
        }

        private void OnEnable() {
            BossRoom.OnRespawn += ResetBody;
        }

        private void OnDisable() {
            BossRoom.OnRespawn -= ResetBody;
        }


        ///<summary>
        /// Toggles all the renderers of the boss body making it invisible
        ///</summary>
        public void ToggleBody(bool _visible) {
            foreach (Renderer renderer in bodyRenders) renderer.enabled = _visible;
            ToggleTentacles(_visible);
            foreach(Renderer mesh in armRenders.currentArm.mesh) mesh.enabled = _visible;
            // if (_visible) ToggleSyth(hasMetamorphosed);
            // if (!_visible) ToggleSyth(false);
        }
        public void ToggleTentacles(bool _visible) {
            foreach (Renderer renderer in tentacleRenderers) renderer.enabled = _visible;

        }


        ///<summary>
        /// Toggles all hitboxes of the 
        ///</summary>
        public void ToggleDeathColliders(bool _enabled) {
            foreach (BossHitBox hitbox in hitBoxes) hitbox.Coll.enabled = _enabled;
        }

        ///<summary>
        /// Toggles/updates the anebling of the bones for secundairephysics
        ///</summary>
        public void ToggleWiggleBones(bool _active) {
            foreach (BossWiggleBone bone in mainBossWiggleBones) bone.enabled = _active;
            foreach (BossWiggleBone bone in metamorphosedBones) bone.enabled = _active && hasMetamorphosed;
            if (_active) armRenders.Toggle(hasMetamorphosed);
        }

        ///<summary>
        /// Time to transform!
        ///</summary>
        public void Metamorphose() {
            if (hasMetamorphosed) return;
            hasMetamorphosed = true;
            StartCoroutine(Matemorphosing());
        }


        #region Shader values
        
        private float tentacleGrowth;
        ///<summary>
        /// How much the tentacle is growing from 0 (invisible) to 1 (fully grown)
        ///</summary>
        public float TentacleGrowth {
            get { return tentacleGrowth;}
            set { 
                tentacleGrowth = value; 
                foreach(Renderer tentacle in tentacleRenderers) {
                    tentacle.material.SetFloat(growKey, value);
                }
                //update spore vfx
                foreach(SkinnedMeshToMesh spore in spores) {
                    if (value < .5f) spore.StopVFX();
                    else  spore.StartVFX();
                }
            }
        }

        private float glow;
        ///<summary>
        /// The glow value of the boss, the colored emission map
        ///</summary>
        public float Glow {
            get { return glow;}
            set { 
                glow = value; 
                foreach(Renderer renderers in bodyRenders) {
                    renderers.material.SetFloat(glowKey, value);
                }
            }
        }

        private float dissolve = 0;
        ///<summary>
        /// The glow value of the boss, the colored emission map
        ///</summary>
        public float Dissolve {
            get { return dissolve;}
            set { 
                glow = value; 
                foreach(Renderer renderers in bodyRenders) {
                    renderers.material.SetFloat(dissolveKey, value);
                    renderers.material.SetFloat(disolveEdgeWidthKey, (value > 0 && value < 1) ? .02f : 0);
                }
                foreach(Renderer mesh in armRenders.SytheArm.mesh) {
                    mesh.material.SetFloat(dissolveKey, value);
                    mesh.material.SetFloat(disolveEdgeWidthKey, (value > 0 && value < 1) ? .02f : 0);
                }
                foreach(Renderer mesh in armRenders.HumanArm.mesh) {
                    mesh.material.SetFloat(dissolveKey, value);
                    mesh.material.SetFloat(disolveEdgeWidthKey, (value > 0 && value < 1) ? .02f : 0);
                }
            }
        }

        #endregion

        //does the coroutine showing all the extra tentacles
        private IEnumerator Matemorphosing() {
            float index = 0;
            TentacleGrowth = 0f;
            ToggleTentacles(true);

            while (index < 10f) {
                UpdateBodyShaders();

                //make sythe
                if (index > 4.8f) {
                    Arm.Toggle(true);
                }

                index += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            armRenders.Shine = 0;
            TentacleGrowth = 1f;
            Glow = 1;
        }

        public void UpdateBodyShaders() {
            Glow = bossAnimator.Animator.GetFloat(glowKey);
            armRenders.Shine = bossAnimator.Animator.GetFloat(shineKey);
            if (TentacleGrowth < .99f) {
                TentacleGrowth = bossAnimator.Animator.GetFloat(growKey);
            }
        }

        ///<summary>
        /// Fired when the boss dies. It disovles the body
        ///</summary>
        public IEnumerator UpdatingDisolve(float _duration) {
            float index = 0;
            while (index < _duration) {
                index += Time.deltaTime;
                Dissolve = bossAnimator.Animator.GetFloat(dissolveKey);
                TentacleGrowth = bossAnimator.Animator.GetFloat(growKey);
                yield return new WaitForEndOfFrame();
                
            }
        }

        public void ResetBody(bool withColor) {
            bossAnimator.Attacking = false;
        }
    }

}