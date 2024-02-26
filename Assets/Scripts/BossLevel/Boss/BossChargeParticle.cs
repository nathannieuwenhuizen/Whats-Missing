using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class BossChargeParticle : MonoBehaviour
    {
        [SerializeField]
        private ChargeParticleInfo[] chargeParticles;

        private Vector3 startScale, endScale;
        
        private SFXInstance chargeSound;



        private void Awake() {
            endScale = transform.localScale;
            startScale = endScale * .3f;
            foreach(ChargeParticleInfo particle in chargeParticles) {
                particle.Setup();
            }
        }
        private void Start() {
            if (chargeSound == null) {
                chargeSound =  AudioHandler.Instance?.Play3DSound(SFXFiles.boss_charge_up_attack, transform, 1f, 1, true, true, 200f, false);
                chargeSound.Play();
                chargeSound.Volume = 0;

            }
        }

        float targetVolume = .5f;

        private void Update() {
            chargeSound.Volume = Mathf.Lerp(chargeSound.Volume, targetVolume, Time.deltaTime);
        }

        public void Interpolate(float percentage) {
            timeScale = Mathf.Lerp(1, 2, percentage);
            if (chargeSound == null) {
                chargeSound =  AudioHandler.Instance?.Play3DSound(SFXFiles.boss_charge_up_attack, transform, 1f, 1, true, true, 200f, false);
                chargeSound.Play();
                chargeSound.Volume = 0;
            }
            targetVolume = .5f + percentage * .5f;
            transform.localScale =  Vector3.Lerp(startScale, endScale, percentage);
            foreach(ChargeParticleInfo particle in chargeParticles) {
                particle.Interpolate(percentage);
            }
        }
        public void StopEmmission() {
            foreach(ChargeParticleInfo particle in chargeParticles) {
                particle.ps.Stop();
            }
        }
        public void ResetEmission() {
            TimeScale = 1f;
            foreach(ChargeParticleInfo particle in chargeParticles) {
                particle.Reset();
            }
        }
        public void PlayEmmission() {
            foreach(ChargeParticleInfo particle in chargeParticles) {
                particle.ps.Play();
            }
        }
        private float timeScale = 1f;

        public float TimeScale {
            get { return timeScale;}
            set { 
                timeScale = value;
                foreach(ChargeParticleInfo particle in chargeParticles) {
                    ParticleSystem.MainModule main = particle.ps.main;
                    particle.ps.GetComponent<ParticleSystemRenderer>().material.SetFloat("_Speed", value < 1 ? 0 : value);
                    particle.ps.GetComponent<ParticleSystemRenderer>().material.SetFloat("_Stretch", value < 1 ? 0 : value);
                    main.simulationSpeed = value;
                } 
            }
        }


        public void StopTime() {
            StartCoroutine( Extensions.AnimateCallBack(1,0, AnimationCurve.Linear(0,0,1,1), (float v) => {
                TimeScale = v;
                targetVolume = v;
            }, 2f));
        }
    }

    [System.Serializable]
    public class ChargeParticleInfo {
        public ParticleSystem ps;
        [HideInInspector]
        public float endEmmission;
        [HideInInspector]
        public float startEmission;

        public void Setup() {
            endEmmission = ps.emissionRate * 1.3f;
            startEmission = endEmmission * .8f;

        }
        public void Reset() {
            ps.emissionRate = startEmission;
        }

        public void Interpolate(float i) {
            ps.emissionRate = Mathf.Lerp(startEmission, endEmmission, i);
            if (ps.GetComponent<ParticleSystemRenderer>() != null)  
                if (ps.GetComponent<ParticleSystemRenderer>().trailMaterial != null)  
                    ps.GetComponent<ParticleSystemRenderer>().trailMaterial.SetFloat("_SimulationSpeed", i);
            // ps.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, i);
        }
    }
}