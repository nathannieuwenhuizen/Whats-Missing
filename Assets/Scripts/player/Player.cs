using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : RoomObject
{

    public delegate void DieEvent();
    public static event DieEvent OnDie;
    public static event DieEvent Onrespawn;

    [SerializeField]
    private Transform headModel;
    [SerializeField]
    private Transform animationView;
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

    [SerializeField]
    private SkinnedMeshRenderer[] meshObjects;


    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected void Awake()
    {
        movement = GetComponent<FPMovement>();

        ApplyCameraSettings(Settings.GetSettings());
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
        AlternativeWords = new string[]{ "myself", "i", "player", "gregory"};
    }
    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) {
            Die();
        }
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
    public void Die() {
        Movement.CharacterAnimator.SetBool("dead", true);
        PlayCutSceneAnimation("dying");
        OnDie?.Invoke();
    }

    public void PlayCutSceneAnimation(string trigger, bool applyRoonAnimation = false, Action callback = null) {
        OnCutsceneStart?.Invoke();
        Movement.EnableRotation = false;
        Movement.EnableWalk = false;
        Movement.RB.velocity = Vector3.zero;

        playerCamera.transform.SetParent(headModel);
        playerCamera.transform.localPosition = animationView.localPosition;
        playerCamera.transform.localRotation = animationView.localRotation;
        StartCoroutine(playerCamera.AnimatingFieldOfView(80, AnimationCurve.EaseInOut(0,0,1,1), 2f));
        Movement.CharacterAnimator.SetTrigger(trigger);
        Movement.CharacterAnimator.applyRootMotion = applyRoonAnimation;

    }

    private int headLayer;
    public void ShowHead() {
        headLayer = meshObjects[1].gameObject.layer;
        meshObjects[1].gameObject.layer = 0;
    }
    public void HideHead() {
        meshObjects[1].gameObject.layer = headLayer;
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
        if (other.GetComponent<AreaTrigger>() != null) {
            other.GetComponent<AreaTrigger>().OnAreaEnter();
        }
        if (other.GetComponent<AreaTextMeshFader>() != null){
            other.GetComponent<AreaTextMeshFader>().FadeIn();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Portal>() != null) {
            other.GetComponent<Portal>().OnPortalLeave();
        }
        if (other.GetComponent<AreaTrigger>() != null) {
            other.GetComponent<AreaTrigger>().OnAreaExit();
        }
    }

    public void EndOfCutSceneAnimation() {
        OnCutsceneEnd?.Invoke();
        playerCamera.transform.SetParent(transform);
        Movement.EnableRotation = true;
        Movement.EnableWalk = true;
        StartCoroutine(playerCamera.AnimatingFieldOfView(60, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        Movement.CameraPivot.localPosition = new Vector3(0,Movement.CameraPivot.transform.localPosition.y,0);
        Movement.CharacterAnimator.applyRootMotion = false;
        Movement.CharacterAnimator.transform.localPosition = Vector3.zero;
    }

    ///<summary>
    /// Enables the movement and sets the camera animation to false.
    ///</summary>
    public void Respawn() {
        Onrespawn?.Invoke();

        // EndOfCutSceneAnimation();
        Movement.CharacterAnimator.SetBool("dead", false);
        PlayCutSceneAnimation("standingUp", true);
        StartCoroutine(DelayEndOfAnimation(5f));
    }

    private IEnumerator DelayEndOfAnimation(float sec) {
        yield return new WaitForSeconds(sec);
        EndOfCutSceneAnimation();
    }
}
