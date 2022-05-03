using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    [System.Serializable]
    public struct ArmRenders {
        [SerializeField]
        public Renderer arm;
        [SerializeField]
        public Renderer sythe;
    }

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
        private string metamorphoseKey = "index";
        [SerializeField]
        private AnimationCurve metamorphoseCurve = AnimationCurve.EaseInOut(0,0,1,1);

        [SerializeField]
        private Renderer[] bodyRenders;
        [SerializeField]
        private Renderer[] metamorphosedRenders;

        [SerializeField]
        private ArmRenders armRenders;
        

        [SerializeField]
        private BossWiggleBone[] mainBossWiggleBones;
        [SerializeField]
        private BossWiggleBone[] metamorphosedBones;
        
        [SerializeField]
        private IKBossPass ikBossPass;
        public IKBossPass IKPass {
            get { return ikBossPass;}
        }


        ///<summary>
        /// Toggles all the renderers of the boss body making it invisible
        ///</summary>
        public void ToggleBody(bool _visible) {
            foreach (Renderer renderer in bodyRenders) renderer.enabled = _visible;
            foreach (Renderer renderer in metamorphosedRenders) renderer.enabled = _visible && hasMetamorphosed;
            if (_visible) ToggleSyth(hasMetamorphosed);
        }

        ///<summary>
        /// Toggles/updates the anebling of the bones for secundairephysics
        ///</summary>
        public void ToggleWiggleBones(bool _active) {
            foreach (BossWiggleBone bone in mainBossWiggleBones) bone.enabled = _active;
            foreach (BossWiggleBone bone in metamorphosedBones) bone.enabled = _active && hasMetamorphosed;
            if (_active) ToggleSyth(hasMetamorphosed);
        }

        ///<summary>
        /// Toggles/updates the arm renders. Mainly used for the metamorphosedPhase
        ///</summary>
        public void ToggleSyth(bool _val) {
            armRenders.sythe.enabled = _val;
            armRenders.arm.enabled = !_val;
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.O)) {
                ToggleBody(true);
            }
            if (Input.GetKeyDown(KeyCode.P)) {
                ToggleBody(false);
            }
        }

        ///<summary>
        /// Time to transform!
        ///</summary>
        public void Metamorphose() {
            if (hasMetamorphosed) return;
            hasMetamorphosed = true;
            StartCoroutine(Matemorphosing());
        }

        //does the coroutine showing all the extra tentacles
        private IEnumerator Matemorphosing() {
            yield return new WaitForSeconds(metamorphoseDelay);
            foreach(Renderer renderer in metamorphosedRenders) {
                float randomDelay = Random.Range(0,.5f);
                StartCoroutine(renderer.material.AnimatingNumberPropertyMaterial(metamorphoseKey, 0, 1, metamorphoseCurve, 
                metamorphoseDuration - randomDelay * metamorphoseDuration , 
                randomDelay * metamorphoseDuration ));
            }
            yield return new WaitForSeconds(metamorphoseDuration);
        }
    }
}