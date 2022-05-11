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
    private Animator animator;
    private IKPass IKPass;
    private Transform animationView;
    private Player player;
    private Transform cameraParent;

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
    public void PlayCutSceneAnimation(string trigger, bool applyRootAnimation = false, Action callback = null) {
        OnCutsceneStart?.Invoke();

        SetTorsoAnimation(false);
        if (trigger != "") inAnimationCutscene = true;

        IKPass.IKActive = false;
        player.Movement.EnableRotation = false;
        player.Movement.EnableWalk = false;
        player.Movement.RB.velocity = Vector3.zero;

        if (player.Camera.transform.parent != animationView) cameraParent = player.Camera.transform.parent;
        player.Camera.transform.SetParent(animationView);
        player.Camera.transform.localPosition = animationView.localPosition;
        player.Camera.transform.localRotation = animationView.localRotation;
        player.StartCoroutine(player.Camera.AnimatingFieldOfView(80, AnimationCurve.EaseInOut(0,0,1,1), 2f));
        animator.SetTrigger(trigger);
        animator.applyRootMotion = applyRootAnimation;
    }

    public void SetAnimator(Animator _animator, Transform _animationView) {
        animator.gameObject.SetActive(false); //may cause errors?
        _animator.gameObject.SetActive(true);        
        animator = _animator;
        animationView = _animationView;
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
        Debug.Log("in boss cutscene!");
        inBossCutscene = true;
        yield return new WaitForSeconds( bossMirror.ShakeDuration - .5f);
        Debug.Log("in boss cutscene for realz");
        PlayCutSceneAnimation("boss_intro_1", false);
        yield return new WaitForSeconds(2f);
        PlayCutSceneAnimation("boss_intro_2", false);
        yield return new WaitForSeconds(.5f);
        PlayCutSceneAnimation("boss_intro_3", false);
        inBossCutscene = false;
    }
    private void OnBossMirrorExplode(BossMirror bossMirror) {

    }

    public void OnBossCutsceneStart(Boss.Boss boss) {
        if (inBossCutscene) return;
        PlayCutSceneAnimation("", false);
    }
    public void OnBossCutsceneEnd(Boss.Boss boss) {
        EndOfCutSceneAnimation();
    }

    ///<summary>
    /// Called when the cutscene animation has ended. Disabling the cutscene ui and enabling the ingame ui.
    ///</summary>
    public void EndOfCutSceneAnimation() {

        OnCutsceneEnd?.Invoke();
        inAnimationCutscene = false;
        player.Camera.transform.SetParent(cameraParent);
        player.Movement.EnableRotation = true;
        player.Movement.EnableWalk = true;
        player.StartCoroutine(player.Camera.AnimatingFieldOfView(60, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        player.Movement.CameraPivot.localPosition = new Vector3(0,player.Movement.CameraPivot.transform.localPosition.y,0);
        animator.applyRootMotion = false;
        animator.transform.localPosition = Vector3.zero;
        IKPass.IKActive = true;

    }

    public bool CameraIsInModel() {
        return player.Camera.transform.parent == animationView;
    }
}
