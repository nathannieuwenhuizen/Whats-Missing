//    Copyright (C) 2020 Ned Makes Games

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections;
using System.Linq;
using Boss;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShockwaveController : MonoBehaviour {

    [SerializeField] private ForwardRendererData rendererData = null;
    [SerializeField] private string featureName = null;


    private BlitMaterialFeature blitFeature;
    private Material material;

    private void Awake() {
        if(TryGetFeature(out var feature)) {
            blitFeature = feature as BlitMaterialFeature;
            blitFeature.SetActive(false);
            material = blitFeature.Material;
        }
    }

    private void OnDestroy() {
        EndShockwave();
    }

    private bool TryGetFeature(out ScriptableRendererFeature feature) {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        return feature != null;
    }


    private void EndShockwave() {
        if(TryGetFeature(out var feature)) {
            rendererData.SetDirty();
            feature.SetActive(false);
        }
    }

    public void StartShockwave(Transform origin) {
        StopAllCoroutines();
        startRadius = 0f;
        Speed = 1f;
        Magnitude = 0.05f;

        StartCoroutine(AnimatingShockwave(origin));
    }

    public void StartSmallShockwave(Transform origin) {
        StopAllCoroutines();
        startRadius = 0;
        Speed = .3f;
        Magnitude = 0.1f;
        StartCoroutine(AnimatingShockwave(origin, false));
    }

    private Vector2 ShockwaveScreenPos {
        get => material.GetVector("_FocalPoint");
        set => material.SetVector("_FocalPoint", value);
    }

    private float Radius {
        get => material.GetFloat("_Radius");
        set => material.SetFloat("_Radius", value);
    }
    public float Speed {
        get => material.GetFloat("_Speed");
        set => material.SetFloat("_Speed", value);
    }
    private float Magnitude {
        get => material.GetFloat("_Magnification");
        set => material.SetFloat("_Magnification", value);
    }

    private void OnEnable() {
        AlchemyItem.OnPulsing += StartSmallShockwave;
        
        Property.onShockwave += StartShockwave;
        BossChangesHandler.OnShockwave += StartShockwave;
    }

    private void OnDisable() {
        AlchemyItem.OnPulsing -= StartSmallShockwave;

        Property.onShockwave -= StartShockwave;
        BossChangesHandler.OnShockwave -= StartShockwave;
    }

    public float startRadius = 0f;
    public float startMagnitute = .1f;


    private bool Backfaced(Transform _origin) {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 toOther = (Camera.main.transform.position - _origin.position).normalized;
        return Vector3.Dot(forward, toOther) > 0;
    }

    private IEnumerator AnimatingShockwave(Transform origin, bool decreasingMagnitude = false) {
        if (!Backfaced(origin)) {
            if(TryGetFeature(out var feature)) 
                feature.SetActive(true);

            Radius = startRadius;
            Magnitude = startMagnitute;

            while(Radius * Speed < 1) {
                yield return new WaitForEndOfFrame();
                Radius += Time.deltaTime;
                Vector2 screenPos = Camera.main.WorldToViewportPoint(origin.position);
                ShockwaveScreenPos = screenPos;

                if (decreasingMagnitude) {
                    Magnitude = Mathf.Max(0, Magnitude - Time.deltaTime * 2f);
                } 
            }
            EndShockwave();
        }

    }
}