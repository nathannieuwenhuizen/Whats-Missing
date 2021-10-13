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
        chokeSFX = AudioHandler.Instance.Player3DSound(SFXFiles.player_choking, transform, 1f, 1f, true, false);
        chokeCoroutine = StartCoroutine(Chocking());
        currentVignette.color = Color.red;
        currentVignette.smoothnes = 1;
    }

    private IEnumerator Chocking() {
        chokeIndex = 0;
        while (chokeIndex < chokeDuration) {
            chokeIndex += Time.deltaTime;
            chokeSFX.AudioSource.volume = .5f + (.5f * (chokeIndex / chokeDuration));

            float speed = Mathf.PI *.5f + (chokeIndex / chokeDuration) * 5f;
            float amplitude = .1f;
            currentVignette.intensity = startVignette.intensity + (Mathf.Sin(chokeIndex * speed) * amplitude);
            UpdateVignette();
            yield return new WaitForEndOfFrame();
        }
        player.Die();
        EndChoking();
    }

    private void EndChoking() {

        if (chokeSFX != null) chokeSFX.AudioSource.Stop();
        if (chokeCoroutine != null) {
            StopCoroutine(chokeCoroutine);
        }
    }
    private void GaspingForAir() {
        EndChoking();
        currentVignette = startVignette;
        UpdateVignette();
        AudioHandler.Instance.PlaySound(SFXFiles.player_relief_gasp, 1f);
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