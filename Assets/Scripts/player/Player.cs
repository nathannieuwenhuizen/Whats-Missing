using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : RoomObject
{

    public delegate void DieEvent(bool withAnimation);
    public static event DieEvent OnDie;
    public static event DieEvent Onrespawn;

    private bool dead = false;

    [SerializeField]
    private SkinnedMeshRenderer mirrorHeadModel;

    [SerializeField]
    private Transform animationView;

    private CharacterAnimationPlayer characterAnimationPlayer;
    public CharacterAnimationPlayer CharacterAnimationPlayer {
        get { return characterAnimationPlayer;}
    }
    [SerializeField]
    private Animator animator;

    public Transform AnimationView {
        get { return animationView;}
    }
    [SerializeField]
    private Transform handsPosition;
    public Transform HandsPosition {
        get { return handsPosition;}
    }

    public delegate void Playerevent();
    public static event Playerevent OnPlayerMissing;
    public static event Playerevent OnPlayerAppear;

    public delegate void CutSceneAction();
    public static event CutSceneAction OnCutsceneStart;
    public static event CutSceneAction OnCutsceneEnd;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private Volume volume;

    public Volume Volume { get => volume; }
    private MotionBlur motionBlur;

    public Camera Camera { get => playerCamera;}
    private int headLayer;

    [SerializeField]
    private SkinnedMeshRenderer[] meshObjects;


    private FPMovement movement;
    public FPMovement Movement { 
        get {
        if (movement == null) movement = GetComponent<FPMovement>();
        return  movement;
        } 
    }
    protected void Awake()
    {
        movement = GetComponent<FPMovement>();
        ApplyCameraSettings(Settings.GetSettings());
        characterAnimationPlayer = new CharacterAnimationPlayer(this, animator);
    }

    public Player() {
        largeScale = 4f;
        shrinkScale = .2f;
    }

    private void ApplyCameraSettings(Settings settings) {
        //set motionblur to settings.
        volume.profile.TryGet<MotionBlur>(out motionBlur);
        motionBlur.active = settings.cameraSettings.Motion_blur_enabled;

    }

    protected override Material[] getMaterials() {
        List<Material> materials = new List<Material>();
        foreach(SkinnedMeshRenderer mr in meshObjects) {
            materials.AddRange(mr.materials);
        }
        return materials.ToArray();
    }


    private void Reset() {
        Word = "me";
        AlternativeWords = new string[]{ "myself", "i", "player", "gregory", "you", "yourself", "self"};
    }
    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) Die(true);
#endif
    }

    private void OnEnable() {
        PerspectiveProperty.onPerspectiveAppearing += HideHead;
        PerspectiveProperty.onPerspectiveMissing += ShowHead;
        SettingPanel.OnSave += ApplyCameraSettings;
    }
    private void OnDisable() {
        PerspectiveProperty.onPerspectiveAppearing -= HideHead;
        PerspectiveProperty.onPerspectiveMissing -= ShowHead;
        SettingPanel.OnSave -= ApplyCameraSettings;
    }

    ///<summary>
    /// Thep play dies and falls to the gorund
    ///</summary>
    public void Die(bool withAnimation = true) {
        if (dead) return;
        dead = true;
        if (withAnimation) {
            characterAnimationPlayer.SetBool("dead", true);
            StartCoroutine(PlayDeadFallSound());
            characterAnimationPlayer.PlayCutSceneAnimation("dying");
        } else {
            movement.EnableRotation = false;
            movement.EnableWalk = false;
        }
        OnDie?.Invoke(withAnimation);
    }
    public void DieWithoutAnimation() {
        Die(false);
    }

    private IEnumerator PlayDeadFallSound() {
        yield return new WaitForSeconds(.6f);
        AudioHandler.Instance.PlaySound(SFXFiles.player_hits_ground, 1f);
    }

    public void ShowHead() {
        
        headLayer = mirrorHeadModel.gameObject.layer;
        mirrorHeadModel.gameObject.layer = 0;
    }
    public void HideHead() {
        mirrorHeadModel.gameObject.layer = headLayer;
    }

    public override void OnMissingFinish()
    {
        //no base call!
        OnPlayerMissing?.Invoke();
        foreach(SkinnedMeshRenderer mr in meshObjects) {
            mr.enabled = false;
        }
    }

    public override void OnAppearing()
    { 
        IsMissing = false;
        OnPlayerAppear?.Invoke();
        //no base call!
        foreach(SkinnedMeshRenderer mr in meshObjects) {
            mr.enabled = true;
        }        
        base.OnAppearing();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Portal>() != null) {
            other.GetComponent<Portal>().OnPortalEnter(this);
        }
        if (other.GetComponent<ITriggerArea>() != null) {
            other.GetComponent<ITriggerArea>().OnAreaEnter(this);
        }
        if (other.GetComponent<AreaTextMeshFader>() != null){
            other.GetComponent<AreaTextMeshFader>().FadeIn();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Portal>() != null) {
            other.GetComponent<Portal>().OnPortalLeave();
        }
        if (other.GetComponent<ITriggerArea>() != null) {
            other.GetComponent<ITriggerArea>().OnAreaExit(this);
        }
    }

    ///<summary>
    /// Enables the movement and sets the camera animation to false.
    ///</summary>
    public void Respawn() {
        dead = false;
        Movement.RB.velocity = Vector3.zero;
        Onrespawn?.Invoke(true);
        characterAnimationPlayer.SetBool("dead", false);
        characterAnimationPlayer.PlayCutSceneAnimation("standingUp", true);
        StartCoroutine(StandingUp());
    }

    ///<summary>
    /// Plays the standing up animation with the sounds
    ///</summary>
    private IEnumerator StandingUp() {
        yield return new WaitForSeconds(2.2f);
        AudioHandler.Instance?.PlaySound( SFXFiles.player_footstep, .1f);
        yield return new WaitForSeconds(.5f);
        AudioHandler.Instance?.PlaySound( SFXFiles.player_footstep, .1f);
        yield return new WaitForSeconds(2.3f);
        characterAnimationPlayer.EndOfCutSceneAnimation();
    }
}
