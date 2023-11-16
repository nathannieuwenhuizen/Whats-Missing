using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms.Impl;

///<summary>
/// Handles the choking mechanism when the air is missing. And when it is burning.
/// Further it handles the player hp in the boss battle
///</summary>
public class Lungs : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private float health = 1f;
    public float Health {
        get { return health;}
        set { 
            health = value;
            float precentage = 1 - value;
            currentVignette.intensity = startVignette.intensity + (precentage) * .4f;
            currentVignette.color = Color.Lerp(startVignette.color, Color.red, precentage);
            currentVignette.smoothnes = startVignette.smoothnes + (precentage) * 2f;
            UpdateVignette();
        }
    }


    private Coroutine healthRecorverCoroutine;
    private float recoverTime = 5f;

    //choke
    private float chokeIndex = 0;
    private float chokeDuration = 30f;
    private Coroutine chokeCoroutine; 
    private SFXInstance chokeSFX;

    //burning
    private SFXInstance burnSFX;
    private SFXInstance playerVoiceBurnSFX;
    private float burnIndex = 0;
    private float burnDuration = 3f;
    private Coroutine burnCoroutine; 
    private bool burning = false;
    private List<FireSpread> fireSpreads = new List<FireSpread>();

    //cold breathing
    [SerializeField]
    private ParticleSystem breathParticleSystem;
    private bool tooCold = false;

    ///<summary>
    /// Start vigentte of the camera
    ///</summary>
    private VignetteData startVignette;
    private VignetteData currentVignette;
    private Vignette vignette;


    ///<summary>
    /// Updates the vignette of the camera
    ///</summary>
    private void UpdateVignette() {
        vignette.intensity.value = currentVignette.intensity;
        vignette.color.value = currentVignette.color;
        vignette.smoothness.value = currentVignette.smoothnes;
    }

    private void Awake() {
        //define the start values
        player.Volume.profile.TryGet<Vignette>(out vignette);
        startVignette = new VignetteData() { 
            intensity = vignette.intensity.value,
            smoothnes = vignette.smoothness.value,
            color = vignette.color.value };
    }

    private void LateUpdate() {
        //pure for the boss battle so the player odesnt die inside the sihled of burning
        if (fireSpreads.Count != 0) {
            if (Player.INVINCIBLE && burning) {
                burning = false;
            }
            if (!Player.INVINCIBLE && !burning) {
                StartBurining(fireSpreads[0]);
            }
        }
    }


#region cold breathing

    private void StartColdBreathing() {
        tooCold = true;
        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "cold");
        breathParticleSystem.Play();
        StartCoroutine(ColdBreathing());
    }

    private IEnumerator ColdBreathing() {
        while(tooCold) {
            yield return new WaitForSeconds(2f);
            AudioHandler.Instance.PlaySound(SFXFiles.exhale, Random.Range(.1f, .3f), Random.Range(.8f, 1f));
        }
    }

    private void StopColdBreathing() {
        tooCold = false;
        breathParticleSystem.Stop();
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);

    }
#endregion

#region burning
    private bool slowFire = true;
    private void StartBurining(FireSpread spread) {

        if (!fireSpreads.Contains(spread)) fireSpreads.Add(spread);
        if (burning) return;
        slowFire = spread.SlowFire;

        burning = true;

        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "choke");
        burnSFX = AudioHandler.Instance.Play3DSound(SFXFiles.fire_spread_burning, transform, 1f, 1f, true, false, 100f, false);
        playerVoiceBurnSFX = AudioHandler.Instance.Play3DSound(SFXFiles.player_cough, transform, 1f, 1f, true, false, 100f, false);

        if (burnIndex <= 0 || burnIndex >= burnDuration) {
            burnCoroutine = StartCoroutine(Burning());
        } 
        UpdateVignette();
    }

    private float BurnDuration() {
        return slowFire ? burnDuration : 1f;
    }

    private IEnumerator Burning() {
        while (burnIndex <= (slowFire ? BurnDuration() : 1f) && burnIndex >= 0) {
            burnIndex += burning ? Time.deltaTime : -Time.deltaTime * 5f;
            playerVoiceBurnSFX.Volume = (.9f * (burnIndex / BurnDuration() ));
            burnSFX.Volume = .1f + (.9f * (burnIndex / BurnDuration() ));
            currentVignette.intensity = startVignette.intensity + (burnIndex / BurnDuration() ) * .4f;
            currentVignette.color = Color.Lerp(startVignette.color, Color.red, (burnIndex / BurnDuration() ));
            currentVignette.smoothnes = startVignette.smoothnes + (burnIndex / BurnDuration() ) * 2f;

            UpdateVignette();
            yield return new WaitForEndOfFrame();
        }
        if (burnIndex > BurnDuration() ) {
            if (player.Room.Area.AreaIndex == 1) {
                Debug.Log("let it burn achievement");
                SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.LetItBurn);
            }
            player.Die(true);
            AudioHandler.Instance.PlaySound(SFXFiles.choke_die, .5f, .8f);
        }
        EndBurning();
    }

    private void OutOfFire(FireSpread spread) {
        fireSpreads.Remove(spread);
        if (fireSpreads.Count == 0) burning = false;
    }

    private void EndBurning() {
        burnIndex = 0;
        if (burnSFX != null) burnSFX.Stop();
        if (playerVoiceBurnSFX != null) playerVoiceBurnSFX.Stop();
        if (burnCoroutine != null) {
            StopCoroutine(burnCoroutine);
        }
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);
        currentVignette = startVignette;
        UpdateVignette();
    }

#endregion

#region air/choking

    private void StartChoking() {
        player.CharacterAnimationPlayer.SetTorsoAnimation(true, "choke");
        chokeSFX = AudioHandler.Instance.Play3DSound(SFXFiles.player_choking, transform, 1f, 1f, true, false, 100f, false);
        player.Movement.FPCamera.CameraZRotationTilt = true;
        chokeCoroutine = StartCoroutine(Chocking());
        currentVignette.color = Color.black;
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
        player.Die(true, true);
        if(player.Room.Area.AreaIndex == 0) SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.StealMyBreathAway);

        AudioHandler.Instance.PlaySound(SFXFiles.choke_die, .5f, .8f);
        EndChoking();
    }

    ///<summary>
    /// End of thechoking resetting the torso animation and camera tilt.
    ///</summary>
    private void EndChoking() {
        player.Movement.FPCamera.CameraZRotationTilt = false;
        player.CharacterAnimationPlayer.SetTorsoAnimation(false);


        if (chokeSFX != null) chokeSFX.Stop();
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


    ///<summary>
    /// Player just has air now!
    ///</summary>
    private IEnumerator GaspingAir() {
        yield return new WaitForSeconds(1f);
        AudioHandler.Instance.PlaySound(SFXFiles.player_relief_gasp, 1f, 1.2f);
    }

#endregion

#region  health

    private bool coolDown = false;
    public void Damage(float _val) {
        if (coolDown) return;
        coolDown = true;

        Health -= _val;
        if (health <= 0) player.Die(true);
        else if (health < 1) {
            if (healthRecorverCoroutine != null) StopCoroutine(healthRecorverCoroutine);
            healthRecorverCoroutine = StartCoroutine(RecoveringHealth());
        }
    }
    private void ResetHealth() {
        Health = 1;
    }

    public IEnumerator RecoveringHealth() {
        yield return new WaitForSeconds(1f);
        coolDown = false;
        while(Health < 1) {
            Health += Time.deltaTime / recoverTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Health = 1;
    }

#endregion

    private void OnEnable() {
        WarmthProperty.OnWarmthMissing += StartColdBreathing;
        WarmthProperty.OnWarmthAppearing += StopColdBreathing;

        FireSpread.OnFireSpreadEnter += StartBurining;
        FireSpread.OnFireSpreadExit += OutOfFire;

        AirProperty.OnAirMissing += StartChoking;
        AirProperty.OnAirAppearing += GaspingForAir;

        Boss.BossHitBox.OnHit += Damage;
        Player.OnRespawn += ResetHealth;
    }

    private void OnDisable() {
        WarmthProperty.OnWarmthMissing -= StartColdBreathing;
        WarmthProperty.OnWarmthAppearing -= StopColdBreathing;

        FireSpread.OnFireSpreadEnter -= StartBurining;
        FireSpread.OnFireSpreadExit -= OutOfFire;

        AirProperty.OnAirMissing -= StartChoking;
        AirProperty.OnAirAppearing -= GaspingForAir;

        Boss.BossHitBox.OnHit -= Damage;
        Player.OnRespawn -= ResetHealth;

        if (chokeSFX != null) chokeSFX.Stop(true);
        if (burnSFX != null) burnSFX.Stop(true);
        if (playerVoiceBurnSFX != null) playerVoiceBurnSFX.Stop(true);
    }
}

///<summary>
/// Vignette data for the post processing of the camera
///</summary>
public struct VignetteData {
    public Color color;
    public float intensity;
    public float smoothnes;
}