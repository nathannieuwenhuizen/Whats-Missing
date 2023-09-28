using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicDepthOfField : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    [SerializeField]
    private Volume volume;
    private DepthOfField depthOfField;

    private bool isHit = false;
    private float hitDistance;

    private float lerpSpeed = 100f;

    private bool perspectiveMissing = false;

    private void Start() {
        volume.profile.TryGet<DepthOfField>(out depthOfField);
        ApplyCameraSettings(Settings.GetSettings());
    }

    private void ApplyCameraSettings(Settings settings) {
        depthOfField.active = settings.cameraSettings.Depth_of_field_enabled && !perspectiveMissing;
    }
    
    private void OnEnable() {
        SettingPanel.OnSave += ApplyCameraSettings;
        PerspectiveProperty.onPerspectiveMissing += PerspectiveMissing;
        PerspectiveProperty.onPerspectiveAppearing += PerspectiveAppear;
    }
    private void OnDisable() {
        SettingPanel.OnSave -= ApplyCameraSettings;
        PerspectiveProperty.onPerspectiveMissing -= PerspectiveMissing;
        PerspectiveProperty.onPerspectiveAppearing -= PerspectiveAppear;
    }

    private void Update() {
        ray = new Ray(transform.position, transform.forward * 100);
        if (Physics.Raycast(ray, out hit, 100f))
        {    
            isHit = true;
            hitDistance = hit.distance;
            // Debug.Log("hit!: " + hit.distance);
        } else {
            if (hitDistance < 100f) {
                hitDistance++;
            }
        }
        SetFocus();
    }

    private void PerspectiveMissing() {
        perspectiveMissing = true;
        ApplyCameraSettings(Settings.GetSettings());
    }
    private void PerspectiveAppear() {
        perspectiveMissing = false;
        ApplyCameraSettings(Settings.GetSettings());
    }

    private void SetFocus() {
        depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.unscaledDeltaTime * hitDistance);
    }

    private void OnDrawGizmos() {
        if (isHit) {
            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(transform.position, hit.point));
        } else {
            Debug.DrawRay(transform.position, transform.forward * lerpSpeed);
        }
    }
}
