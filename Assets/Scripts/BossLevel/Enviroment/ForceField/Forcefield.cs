using System;
using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;
using UnityEngine.AI;

namespace ForcefieldDemo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Forcefield : RoomObject, ITriggerArea
    {    
        public delegate void ForcefieldEvent();
        public static ForcefieldEvent OnForceFieldEnter;
        public static ForcefieldEvent OnForceFieldExit;
        public static ForcefieldEvent OnForceFieldImpact;
        private NavMeshObstacle navMeshObstacle;

        private Rigidbody rb;

        [SerializeField] 
        private Collider[] sphereColliders;
        [SerializeField]
        private bool isOn = false;
        private Coroutine disolveCoroutine;
        [SerializeField]
        private int burstAmmount = 600;
        

        private Coroutine dampenCoroutine;

        // time until impact ripple dissipates
        [Range(0.1f, 5f)]
        [SerializeField] private float dampenTime = 1.5f;


        //filling values
        private float fillMax = .2f;
        private float fillIdle;

        // maximum displacement on impact
        [Range(0.002f,3f)]
        [SerializeField] private float impactRippleAmplitude = 0.005f;
        [Range(0.05f, 100f)]
        [SerializeField] private float impactRippleMaxRadius = 0.35f;

        // allow mouse clicks for testing 
        [SerializeField] private bool clickToImpact;

        //// slight delay between clicks to prevent spamming
        private const float coolDownMax = 0.25f;
        private float coolDownWindow;

        // main camera
        private Camera cam;

        // reference to this MeshRenderer
        [SerializeField]
        private MeshRenderer meshRenderer;
        [SerializeField]
        private MeshRenderer mirrorMeshRenderer;
        [SerializeField]
        private ParticleSystem burstParticle;
        [SerializeField]
        private ParticleSystem ringsParticle;


        protected override void Awake() {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            mirrorMeshRenderer.material.SetInt("_enableIntersection", 0);
            StartCoroutine(Cooldown());
            
            meshRenderer.enabled = false;
            ToggleColliders(false);
            Dissolve = 1;

            Radius = meshRenderer.transform.lossyScale.y * .5f;
            fillIdle = Fill;

            if (cam == null && Camera.main != null)
            {
                cam = Camera.main;
            }
            coolDownWindow = 0;
        }
        void Start()
        {

        }
        private void FixedUpdate() {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }

        ///<summary>
        /// If the IsOn Is true, the forcefield will be active, rendering them eshes and activating its colliders.
        ///</summary>
        public bool IsOn {
            get { return isOn;}
            set { 
                isOn = value; 
                Debug.Log("on deactivating shield" + isOn);

                ToggleColliders(true); // umst be true during animation
                if (navMeshObstacle) navMeshObstacle.enabled = value;
                if (disolveCoroutine != null) StopCoroutine(disolveCoroutine);
                disolveCoroutine = StartCoroutine(Dissolving(value));
            }
        }  

        private IEnumerator Dissolving(bool turningOn) {
            meshRenderer.enabled = true;
            yield return StartCoroutine(meshRenderer.material.AnimatingDissolveMaterial(Dissolve, turningOn ? 0 : 1, AnimationCurve.EaseInOut(0,0,1,1), 2f));
            ToggleColliders(IsOn);

            if (IsOn) ringsParticle.Play();
            else ringsParticle.Stop();
            meshRenderer.enabled = isOn;
        }   

        public void ToggleColliders(bool _value) {
            foreach(Collider col in sphereColliders) col.enabled = _value;
        }


        #region DIAGNOSTIC 
        private void UpdateMouse()
        {
            coolDownWindow -= Time.deltaTime;
            if (coolDownWindow <= 0)
                if (Input.GetMouseButtonDown(0))
                    ClickToImpact();
        }

        // allow mouse clicks to test forcefield - useful for diagnostic
        private void ClickToImpact()
        {
            if (cam == null)
                return;

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);//  cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Transform hitXform = hit.transform;

                if (hitXform == meshRenderer.transform)
                {
                    coolDownWindow = coolDownMax;
                    ApplyImpact(hit.point, hit.normal);
                }

            }
        }
        #endregion

        public void EnableRipple(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableRipple", onOff);
            mirrorMeshRenderer?.material.SetFloat("_enableRipple", onOff);
        }

        public void EnableRimGlow(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableRimGlow", onOff);
            mirrorMeshRenderer?.material.SetFloat("_enableRimGlow", onOff);
        }

        public void EnableScanLine(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableScanLine", onOff);
            mirrorMeshRenderer?.material.SetFloat("_enableScanLine", onOff);
        }

        public void EnableFillTexture(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableFillTexture", onOff);
            mirrorMeshRenderer?.material.SetFloat("_enableFillTexture", onOff);
        }

        public void EnableIntersection(bool state = false)
        {
            int onOff = (state) ? 1 : 0;
            meshRenderer?.material.SetFloat("_enableIntersection", onOff);
        }

        public float ImapctAmplitude {
            get { return meshRenderer.material.GetFloat("_impactRippleAmplitude");}
            set { meshRenderer.material.SetFloat("_impactRippleAmplitude", value); }
        }
        public float Fill {
            get { return meshRenderer.material.GetFloat("_fill");}
            set { meshRenderer.material.SetFloat("_fill", value); }
        }
        public float Radius {
            get { return meshRenderer.material.GetFloat("_radius");}
            set { meshRenderer.material.SetFloat("_radius", value); }
        }
        public float Dissolve {
            get { return meshRenderer.material.GetFloat("Dissolve");}
            set { 
                meshRenderer.material.SetFloat("Dissolve", value); 
                mirrorMeshRenderer.material.SetFloat("Dissolve", value); 
            }
        }

        public bool InsideArea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        // impact Forcefield, passing in hit point and direction
        public void ApplyImpact(Vector3 position, Vector3 direction)
        {
            if (meshRenderer != null)
            {
                AudioHandler.Instance?.PlaySound(SFXFiles.magic_shield_block);
                OnForceFieldImpact?.Invoke();

                EnableRipple(true);
                meshRenderer.material.SetFloat("_impactRippleMaxRadius", impactRippleMaxRadius);
                ImapctAmplitude = impactRippleAmplitude;
                Fill = fillMax;
                // meshRenderer.material.SetFloat("_impactRippleAmplitude", impactRippleAmplitude);
                meshRenderer.material.SetVector("_impactRippleDirection", direction);
                meshRenderer.material.SetVector("_impactPoint", position);
                burstParticle.Emit(burstAmmount);

                if (dampenCoroutine != null) StopCoroutine(dampenCoroutine);
                dampenCoroutine = StartCoroutine(Dampen());
            }
        }

        // impact Forcefield, passing in RaycastHit
        public void ApplyImpact(RaycastHit hit)
        {
            ApplyImpact(hit.point, hit.normal);
        }

        // gradually slow ripple motion 
        private IEnumerator Dampen()
        {
            yield return new WaitForFixedUpdate();
            while(ImapctAmplitude > 0) {
                ImapctAmplitude -= (impactRippleAmplitude * Time.deltaTime / dampenTime);
                Fill -= ((fillMax - fillIdle) * Time.deltaTime / dampenTime);
                yield return new WaitForEndOfFrame();
            }
            ImapctAmplitude = 0;
            Fill = fillIdle;
            EnableRipple(false);
        }
        public override void OnDestroy() {
            base.OnDestroy();
            Dissolve = 0;
        }

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.L))
            //         if (!IsOn) IsOn = true;

#if UNITY_EDITOR
            if (clickToImpact)
                UpdateMouse();
#endif
        }


        private bool coolDowned = true;
        public IEnumerator Cooldown() {
            coolDowned = false;
            yield return new WaitForSeconds(2f);
            coolDowned = true;
        }
        private void OnTriggerEnter(Collider other) {
            // boss collides with forcefield

            if (other.GetComponent<Boss.BossHitBox>() == null)  return;
            if (!other.isTrigger) 
                return;
            

            if (!coolDowned) return;
            StartCoroutine(Cooldown());

            Vector3 position = other.ClosestPoint(transform.position);
            Vector3 normal = (position - transform.position).normalized;
            // Debug.Log("something triggered me: " + other.name);
            ApplyImpact(position, normal);
        }

        public void OnAreaEnter(Player player)
        {
            OnForceFieldEnter?.Invoke();
        }

        public void OnAreaExit(Player player)
        {
            OnForceFieldExit?.Invoke();
        }

        private void OnEnable() {
            StartCoroutine(Cooldown()); // prevent apply impact bug
            BaseChaseState.AttemptingToHitShield += ActiavateForceField;
        }

        private void OnDisable() {
            BaseChaseState.AttemptingToHitShield -= ActiavateForceField;
            
        }

        public void ActiavateForceField() {
            if (IsOn) return;
            IsOn = true;
        }

        public override void OnMissing()
        {
            base.OnMissing();
            Player.INVINCIBLE = false;
            IsOn = false;
        }
        public override void OnMissingFinish()
        {
            base.OnMissingFinish();
            Player.INVINCIBLE = false;
            gameObject.SetActive(false);
        }


        ///<summary>
        /// Returns the position that lies on the edge of the forcefield directing to the _from parameter.
        ///</summary>
        public Vector3 EdgePosition(Vector3 _from, float _offset = 1f) {
            Vector3 result = transform.position;

            Vector3 delta = _from - transform.position;
            delta.y = 0;
            result = delta.normalized * (meshRenderer.transform.lossyScale.y * .5f + _offset);

            return transform.position + result;
        }
    }
}