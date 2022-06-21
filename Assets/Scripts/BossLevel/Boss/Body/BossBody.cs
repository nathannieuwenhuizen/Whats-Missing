using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    // [System.Serializable]
    // public struct ArmRenders {
    //     [SerializeField]
    //     public Renderer arm;
    //     [SerializeField]
    //     public Renderer sythe;

    //     public void Toggle(bool _toSythe) {
    //         arm.gameObject.SetActive(!_toSythe);
    //         sythe.gameObject.SetActive(_toSythe);
    //     }
    // }

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
        private readonly string growKey = "_grow";
        public static readonly string shineKey = "_shine_power";
        private readonly string glowKey = "_glow_power";


        [SerializeField]
        private AnimationCurve metamorphoseCurve = AnimationCurve.EaseInOut(0,0,1,1);

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
        private BossWiggleBone[] mainBossWiggleBones;
        [SerializeField]
        private BossWiggleBone[] metamorphosedBones;
        
        [SerializeField]
        private IKBossPass ikBossPass;
        public IKBossPass IKPass {
            get { return ikBossPass;}
        }
        

        [HideInInspector]
        public Boss boss;

        private void Start() {
            bossAnimator = new BossAnimator(boss, IKPass);
            ToggleDeathColliders(false);
            Grow = 0;
            Arm.Toggle(true);
        }

        ///<summary>
        /// Toggles all the renderers of the boss body making it invisible
        ///</summary>
        public void ToggleBody(bool _visible) {
            foreach (Renderer renderer in bodyRenders) renderer.enabled = _visible;
            foreach (Renderer renderer in tentacleRenderers) renderer.enabled = _visible && hasMetamorphosed;
            // if (_visible) ToggleSyth(hasMetamorphosed);
            // if (!_visible) ToggleSyth(false);
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

        
        private float grow;
        public float Grow {
            get { return grow;}
            set { 
                grow = value; 
                foreach(Renderer tentacle in tentacleRenderers) {
                    tentacle.material.SetFloat(growKey, value);
                }
            }
        }
        private float glow;
        public float Glow {
            get { return glow;}
            set { 
                glow = value; 
                foreach(Renderer renderers in bodyRenders) {
                    renderers.material.SetFloat(glowKey, value);
                }
            }
        }


        //does the coroutine showing all the extra tentacles
        private IEnumerator Matemorphosing() {
            float index = 0;
            Grow = 0f;

            while (index < 10f) {
                Glow = bossAnimator.Animator.GetFloat(glowKey);
                armRenders.Shine = bossAnimator.Animator.GetFloat(shineKey);
                if (Grow < .99f) {
                    Grow = bossAnimator.Animator.GetFloat(growKey);
                }

                index += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            armRenders.Shine = 0;
            Grow = 1f;
            Glow = 1;
        }


    }
}