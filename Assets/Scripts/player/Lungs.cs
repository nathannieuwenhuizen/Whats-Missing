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

    private float chokeIndex = 0;
    private float chokeDuration = 30;
    private Coroutine chokeCoroutine; 
    private SFXInstance chokeSFX;

    private SFXInstance burnSFX;
    private SFXInstance playerVoiceBurnSFX;

    private float burnIndex = 0;
    private float burnDuration = 15f;
    private Coroutine burnCoroutine; 
    private bool burning = true;


    private VignetteData startVignette;
    private VignetteData currentVignette;
    private Vignette vignette;

    private bool tooCold = false;
    [SerializeField]
    private ParticleSystem breathParticleSystem;

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
        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "choke");
        chokeSFX = AudioHandler.Instance.Player3DSound(SFXFiles.player_choking, transform, 1f, 1f, true, false, 100f, false);
        player.Movement.CameraZRotationTilt = true;
        chokeCoroutine = StartCoroutine(Chocking());
        currentVignette.color = Color.black;
        currentVignette.smoothnes = 1;
    }
    private void StartBurining() {
        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "choke");
        burnSFX = AudioHandler.Instance.Player3DSound(SFXFiles.fire_spread_burning, transform, 1f, 1f, true, false, 100f, false);
        playerVoiceBurnSFX = AudioHandler.Instance.Player3DSound(SFXFiles.player_cough, transform, 1f, 1f, true, false, 100f, false);

        burning = true;
        if (burnIndex <= 0 || burnIndex >= burnDuration) {
            burnCoroutine = StartCoroutine(Burning());
        } 
        UpdateVignette();
    }

    private void StartColdBreathing() {
        tooCold = true;
        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "cold");
        breathParticleSystem.Play();
        StartCoroutine(ColdBreathing());
    }
    private void StopColdBreathing() {
        tooCold = false;
        breathParticleSystem.Stop();
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);

    }

    private IEnumerator ColdBreathing() {
        while(tooCold) {
            yield return new WaitForSeconds(2f);
            AudioHandler.Instance.PlaySound(SFXFiles.exhale, Random.Range(.1f, .3f), Random.Range(.8f, 1f));
        }
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
        player.Die(true);
        AudioHandler.Instance.PlaySound(SFXFiles.choke_die, .5f, .8f);
        EndChoking();
    }

    private IEnumerator Burning() {
        while (burnIndex <= burnDuration && burnIndex >= 0) {
            burnIndex += burning ? Time.deltaTime : -Time.deltaTime * 5f;
            playerVoiceBurnSFX.Volume = (.9f * (burnIndex / burnDuration));
            burnSFX.Volume = .1f + (.9f * (burnIndex / burnDuration));
            currentVignette.intensity = startVignette.intensity + (burnIndex / burnDuration) * .4f;
            currentVignette.color = Color.Lerp(startVignette.color, Color.red, (burnIndex / burnDuration));
            currentVignette.smoothnes = startVignette.smoothnes + (burnIndex / burnDuration) * 2f;

            UpdateVignette();
            yield return new WaitForEndOfFrame();
        }
        if (burnIndex > burnDuration) {
            player.Die(true);
            AudioHandler.Instance.PlaySound(SFXFiles.choke_die, .5f, .8f);
        }
        EndBurning();
    }

    private void EndBurning() {
        burnIndex = 0;
        if (burnSFX != null) burnSFX.AudioSource.Stop();
        if (playerVoiceBurnSFX != null) playerVoiceBurnSFX.AudioSource.Stop();
        if (burnCoroutine != null) {
            StopCoroutine(burnCoroutine);
        }
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);
        currentVignette = startVignette;
        UpdateVignette();
    }

    private void EndChoking() {
        player.Movement.CameraZRotationTilt = false;
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);


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

    public void OutOfFire() {
        burning = false;
    }


    private void OnEnable() {
        WarmthProperty.OnWarmthMissing += StartColdBreathing;
        WarmthProperty.OnWarmthAppearing += StopColdBreathing;
        FireSpread.OnFireSpreadEnter += StartBurining;
        FireSpread.OnFireSpreadExit += OutOfFire;
        AirProperty.OnAirMissing += StartChoking;
        AirProperty.OnAirAppearing += GaspingForAir;
    }
    private void OnDisable() {
        WarmthProperty.OnWarmthMissing -= StartColdBreathing;
        WarmthProperty.OnWarmthAppearing -= StopColdBreathing;
        FireSpread.OnFireSpreadEnter -= StartBurining;
        FireSpread.OnFireSpreadExit -= OutOfFire;

        AirProperty.OnAirMissing -= StartChoking;
        AirProperty.OnAirAppearing -= GaspingForAir;
    }
}

public struct VignetteData {
    public Color color;
    public float intensity;
    public float smoothnes;
}