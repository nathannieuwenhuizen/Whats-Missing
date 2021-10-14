using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

///<summary>
/// Handles the choking mechanism when the air is missing.
///</summary>
public class Lungs : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private float chokeIndex = 0;
    private float chokeDuration = 30;
    private Coroutine chokeCoroutine; 
    private SFXInstance chokeSFX;

    private VignetteData startVignette;
    private VignetteData currentVignette;
    private Vignette vignette;

    private void UpdateVignette() {
        vignette.intensity.value = currentVignette.intensity;
        vignette.color.value = currentVignette.color;
        vignette.smoothness.value = currentVignette.smoothnes;
    }
    private void Awake() {
        player.Volume.profile.TryGet<Vignette>(out vignette);
        startVignette = new VignetteData() { 
            intensity = vignette.intensity.value,
            smoothnes = vignette.smoothness.value,
            color = vignette.color.value };
    }


    private void StartChoking() {
        chokeSFX = AudioHandler.Instance.Player3DSound(SFXFiles.player_choking, transform, 1f, 1f, true, false, 100f, false);
        player.Movement.CameraZRotationTilt = true;
        chokeCoroutine = StartCoroutine(Chocking());
        currentVignette.color = Color.red;
        currentVignette.smoothnes = 1;
    }

    private IEnumerator Chocking() {
        chokeIndex = 0;
        float ammountOfHeartPulse = 0;
        while (chokeIndex < chokeDuration) {
            chokeIndex += Time.deltaTime;
            chokeSFX.Volume = .1f + (.9f * (chokeIndex / chokeDuration));

            float speed = (Mathf.PI *.2f) + (chokeIndex / chokeDuration) * 5f;
            float amplitude = .1f;
            currentVignette.intensity = startVignette.intensity + (Mathf.Abs(Mathf.Sin(chokeIndex * speed)) * amplitude);
            if (chokeIndex * speed > ammountOfHeartPulse * Mathf.PI) {
                ammountOfHeartPulse++;
                AudioHandler.Instance.PlaySound(SFXFiles.heartbeat, .3f, 1f + speed * .4f);
            }
            UpdateVignette();
            yield return new WaitForEndOfFrame();
        }
        player.Die();
        AudioHandler.Instance.PlaySound(SFXFiles.choke_die, .5f, .8f);
        EndChoking();
    }

    private void EndChoking() {
        player.Movement.CameraZRotationTilt = false;

        if (chokeSFX != null) chokeSFX.AudioSource.Stop();
        if (chokeCoroutine != null) {
            StopCoroutine(chokeCoroutine);
        }
    }
    private void GaspingForAir() {
        EndChoking();
        currentVignette = startVignette;
        UpdateVignette();
        StartCoroutine(GaspingAir());
    }
    private IEnumerator GaspingAir() {
        yield return new WaitForSeconds(1f);
        AudioHandler.Instance.PlaySound(SFXFiles.player_relief_gasp, 1f, 1.2f);
    }


    private void OnEnable() {
        AirProperty.OnAirMissing += StartChoking;
        AirProperty.OnAirAppearing += GaspingForAir;
    }
    private void OnDisable() {
        AirProperty.OnAirMissing -= StartChoking;
        AirProperty.OnAirAppearing -= GaspingForAir;
    }
}

public struct VignetteData {
    public Color color;
    public float intensity;
    public float smoothnes;
}