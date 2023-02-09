using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Habdles all the calls to the animator of the player.
///</summary>
public class CharacterAnimationPlayer
{
    public delegate void CutSceneAction();
    public static event CutSceneAction OnCutsceneStart;
    public static event CutSceneAction OnCutsceneEnd;

    private Coroutine torsoCoroutine;
    private float cameraZoom = 60f;
    private Coroutine zoomCoroutine;
    public float CameraZoom {
        get { return cameraZoom;}
        set { 
            cameraZoom = value; 
            if (zoomCoroutine != null) player.StopCoroutine(zoomCoroutine);
            zoomCoroutine = player.StartCoroutine(player.Camera.AnimatingFieldOfView(cameraZoom, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        }
    }
    private Animator animator;
    private IKPass IKPass;
    private Transform animationView;
    private Player player;

    private bool inAnimationCutscene = false;
    public bool InCutScene {
        get { return inAnimationCutscene;}
    }

    public CharacterAnimationPlayer(Player _player, Animator _animator, Transform _animationView, IKPass _IKPass) {
        animator = _animator;
        animationView = _animationView;
        player = _player;
        IKPass =_IKPass;
    }

    ///<summary>
    /// Set the walk values for the palyer. .5f is walking and 1f is running.
    ///</summary>
    public void SetWalkValues(Vector2 value) {
        animator.SetFloat("deltaX", value.x);
        animator.SetFloat("deltaY", value.y);
    }

    ///<summary>
    /// Set if the player is in a jump/in air animation or not.
    ///</summary>
    public void SetInAir(bool val) {
        animator.SetBool("inAir", val);
    }

    ///<summary>
    /// Set if the player is in a water animation or not.
    ///</summary>
    public void SetInWater(bool val) {
        animator.SetBool("inWater", val);
    }

    ///<summary>
    /// Sets the weight of the torso animation, used for if the player chokes or is cold.
    ///</summary>
    public void SetTorsoAnimation(bool on, string trigger = "") {
        if (torsoCoroutine != null) player.StopCoroutine(torsoCoroutine);
        torsoCoroutine = player.StartCoroutine(AnimatingTorsoWeight(on ? 1 : 0));
        if (on) animator.SetTrigger(trigger);
    }

    private IEnumerator AnimatingTorsoWeight(float end) {
        float index = 0; 
        float start = animator.GetLayerWeight(1);
        while (index < 1f) {
            index += Time.deltaTime;
            animator.SetLayerWeight(1, Mathf.Lerp(start, end, index / 1f));
            yield return new WaitForEndOfFrame();
        }
        animator.SetLayerWeight(1, end);
    }

    ///<summary>
    /// Sets a trigger of the animator
    ///</summary>
    public void SetTrigger(string triggerkey) {
        animator.SetTrigger(triggerkey);
    }
    ///<summary>
    /// Sets the boolean of the animator
    ///</summary>
    public void SetBool(string key, bool val) {
        animator.SetBool(key, val);
    }

    ///<summary>
    /// Plays an animation by triggering it while also calling the cutscene ui.
    /// TODO: make sure the callbacks works after the animation has finished playing.
    ///</summary>
    public void PlayCutSceneAnimation(string _trigger, bool _applyRootAnimation = false, Action _callback = null, float _zoom = 80f, bool _cameraToAnimationView = true) {
        OnCutsceneStart?.Invoke();

        SetTorsoAnimation(false);
        if (_trigger != "") inAnimationCutscene = true;

        IKPass.IKActive = false;
        player.Movement.EnableRotation = false;
        player.Movement.EnableWalk = false;
        player.Movement.RB.velocity = Vector3.zero;

        if (_cameraToAnimationView) player.FPCamera.SetParentToAnimation(animationView);
        CameraZoom = _zoom;

        //activates the trigger
        animator.SetTrigger(_trigger);
        animator.applyRootMotion = _applyRootAnimation;
    }

    public void SetAnimator(Player.PlayerAnimator playerAnimator) {
        animator.gameObject.SetActive(false); //may cause errors?
        playerAnimator.animator.gameObject.SetActive(true);        
        animator = playerAnimator.animator;
        animationView = playerAnimator.cameraView;
    }

    public void OnEnable() {
        BossMirror.OnMirrorShake += OnBossMirrorShake;
        BossMirror.OnMirrorExplode += OnBossMirrorExplode;
        Boss.BossCutsceneState.OnBossCutsceneStart += OnBossCutsceneStart;
        Boss.BossCutsceneState.OnBossCutsceneEnd += OnBossCutsceneEnd;

    }
    public void OnDisable() {
        BossMirror.OnMirrorShake -= OnBossMirrorShake;
        BossMirror.OnMirrorExplode -= OnBossMirrorExplode;
        Boss.BossCutsceneState.OnBossCutsceneStart -= OnBossCutsceneStart;
        Boss.BossCutsceneState.OnBossCutsceneEnd -= OnBossCutsceneEnd;
    }

    private void OnBossMirrorShake(BossMirror bossMirror) {
        PlayCutSceneAnimation("", false);
        bossMirror.StartCoroutine(InBossCutScene(bossMirror));
    }
    private bool inBossCutscene = false;
    private IEnumerator InBossCutScene(BossMirror bossMirror) {
        inBossCutscene = true;
        yield return new WaitForSeconds( bossMirror.ShakeDuration - .5f);
        Debug.Log("in boss cutscene for realz");
        PlayCutSceneAnimation("boss_intro_1", false);
        yield return new WaitForSeconds(3f);
        //no call playcutsceneanimation, as it flickers the camera
        animator.SetTrigger("boss_intro_2");
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("boss_intro_3");
        inBossCutscene = false;
    }
    private void OnBossMirrorExplode(BossMirror bossMirror) {

    }

    public void OnBossCutsceneStart(Boss.Boss boss, float zoom) {
        if (inBossCutscene) return;
        PlayCutSceneAnimation("", false);
        CameraZoom = zoom;
    }
    public void OnBossCutsceneEnd(Boss.Boss boss, float zoom) {
        EndOfCutSceneAnimation();
    }

    ///<summary>
    /// Called when the cutscene animation has ended. Disabling the cutscene ui and enabling the ingame ui.
    ///</summary>
    public void EndOfCutSceneAnimation() {

        OnCutsceneEnd?.Invoke();
        inAnimationCutscene = false;
        player.FPCamera.ResetParent();

        CameraZoom = 60f;
        player.StartCoroutine(AnimateEndOfCutscene(() => {
            player.Movement.EnableRotation = true;
            player.Movement.EnableWalk = true;
            animator.applyRootMotion = false;
            IKPass.IKActive = true;
        }));
    }
    
    public IEnumerator AnimateEndOfCutscene(Action _callback) {
        player.StartCoroutine(player.Movement.CameraPivot.AnimatingLocalPos(new Vector3(0,player.Movement.CameraPivot.transform.localPosition.y,0), AnimationCurve.EaseInOut(0,0,1,1)));
        player.StartCoroutine(player.Movement.CameraPivot.AnimatingLocalRotation(Quaternion.Euler(player.Movement.CameraPivot.localRotation.eulerAngles.x,0,0), AnimationCurve.EaseInOut(0,0,1,1)));
        player.StartCoroutine(animator.transform.AnimatingLocalPos(Vector3.zero, AnimationCurve.EaseInOut(0,0,1,1)));

        yield return new WaitForSeconds(.5f);
        _callback.Invoke();

    }
    ///<summary>
    /// Plays the standing up animation with the sounds
    ///</summary>
    public IEnumerator StandingUp() {
        yield return new WaitForSeconds(2.2f);
        AudioHandler.Instance?.PlaySound( SFXFiles.player_footstep_normal, .1f);
        yield return new WaitForSeconds(.5f);
        AudioHandler.Instance?.PlaySound( SFXFiles.player_footstep_normal, .1f);
        yield return new WaitForSeconds(2.3f);
        EndOfCutSceneAnimation();
    }

    public bool CameraIsInModel() {
        return player.Camera.transform.parent == animationView;
    }
}
