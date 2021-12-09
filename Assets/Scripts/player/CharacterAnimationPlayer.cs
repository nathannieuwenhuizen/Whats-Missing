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
    private Player player;

    public CharacterAnimationPlayer(Player _player, Animator _animator) {
        animator = _animator;
        player = _player;
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
    public void PlayCutSceneAnimation(string trigger, bool applyRoonAnimation = false, Action callback = null) {
        OnCutsceneStart?.Invoke();

        SetTorsoAnimation(false);
        player.Movement.EnableRotation = false;
        player.Movement.EnableWalk = false;
        player.Movement.RB.velocity = Vector3.zero;

        player.Camera.transform.SetParent(player.AnimationView);
        player.Camera.transform.localPosition = player.AnimationView.localPosition;
        player.Camera.transform.localRotation = player.AnimationView.localRotation;
        player.StartCoroutine(player.Camera.AnimatingFieldOfView(80, AnimationCurve.EaseInOut(0,0,1,1), 2f));
        animator.SetTrigger(trigger);
        animator.applyRootMotion = applyRoonAnimation;
    }

    ///<summary>
    /// Called when the cutscene animation has ended. Disabling the cutscene ui and enabling the ingame ui.
    ///</summary>

    public void EndOfCutSceneAnimation() {
        OnCutsceneEnd?.Invoke();
        player.Camera.transform.SetParent(player.transform);
        player.Movement.EnableRotation = true;
        player.Movement.EnableWalk = true;
        player.StartCoroutine(player.Camera.AnimatingFieldOfView(60, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        player.Movement.CameraPivot.localPosition = new Vector3(0,player.Movement.CameraPivot.transform.localPosition.y,0);
        animator.applyRootMotion = false;
        animator.transform.localPosition = Vector3.zero;
    }
}