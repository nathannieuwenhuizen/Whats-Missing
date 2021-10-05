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
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShockwaveController : MonoBehaviour {

    [SerializeField] private ForwardRendererData rendererData = null;
    [SerializeField] private string featureName = null;


    private BlitMaterialFeature blitFeature;
    private Material material;

    private void Update() {
        // if(Input.GetKeyDown(KeyCode.L)) {
        //     StartShockwave(transform);
        // }
    }

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
        Vector2 screenPos = Camera.main.WorldToViewportPoint(origin.position);
        Debug.Log("screenpos: "+ screenPos);
        StopAllCoroutines();
        StartCoroutine(AnimatingShockwave(origin));
    }

    private Vector2 ShockwaveScreenPos {
        get => material.GetVector("_FocalPoint");
        set => material.SetVector("_FocalPoint", value);
    }

    private float Radius {
        get => material.GetFloat("_Radius");
        set => material.SetFloat("_Radius", value);
    }
    private float Speed {
        get => material.GetFloat("_Speed");
        set => material.SetFloat("_Speed", value);
    }

    private void OnEnable() {
        Property.onShockwave += StartShockwave;
    }

    private void OnDisable() {
        Property.onShockwave -= StartShockwave;
    }

    private IEnumerator AnimatingShockwave(Transform origin) {
        if(TryGetFeature(out var feature)) 
            feature.SetActive(true);

        Radius = 0;
        while(Radius * Speed < 1) {
            yield return new WaitForEndOfFrame();
            Radius += Time.deltaTime;
            Vector2 screenPos = Camera.main.WorldToViewportPoint(origin.position);
            ShockwaveScreenPos = screenPos;
        }
        EndShockwave();
    }
}