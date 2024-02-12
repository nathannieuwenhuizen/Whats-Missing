using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : RoomObject
{
    ///<summary>
    /// Player cant be hit by the hitbox used for the bossai to determine if the palyer is inside a shield
    ///</summary>
    public static bool INVINCIBLE = false;

    public delegate void DieEvent(bool withAnimation, bool toPreviousLevel);
    public static event DieEvent OnDie;
    public delegate void RespawnEvent();
    public static RespawnEvent OnRespawn;

    private bool dead = false;

    [SerializeField]
    private SkinnedMeshRenderer mirrorHeadModel;

    private FPCamera fpCamera;
    public FPCamera FPCamera {
        get { 
            if (fpCamera == null) {
                fpCamera = fpCamera = new FPCamera(GetComponent<FPMovement>());
            }
            return fpCamera;
        }
    }

    [SerializeField]
    private Hands hands;

    private CharacterAnimationPlayer characterAnimationPlayer;
    public CharacterAnimationPlayer CharacterAnimationPlayer {
        get { return characterAnimationPlayer;}
    }

    [Header("animators")]
    [SerializeField]
    private PlayerAnimator idleAnimator;
    [SerializeField]
    private PlayerAnimator level2End;
    [SerializeField]
    public PlayerAnimator finalLevelEnd;

    [Header("IK pass")]

    [SerializeField]
    private IKPass IKPass;
    public IKPass IKPASS {
        get { return IKPass;}
    }

    [SerializeField]
    private Transform handsPosition;
    public Transform HandsPosition {
        get { return handsPosition;}
    }

    public override float CurrentScale { get { return base.CurrentScale; } 
        set {
            base.CurrentScale = value;
            hands.MassThreshhold = value;
            movement.RB.mass = value;
            Camera.nearClipPlane = .1f * value;
        }  
    }

    public delegate void Playerevent();
    public static event Playerevent OnPlayerMissing;
    public static event Playerevent OnPlayerAppear;
    public static event Playerevent OnPlayerShrink;
    public static event Playerevent OnPlayerUnShrink;

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

    [SerializeField] private Collider small_collider;


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
        small_collider.enabled = false;
        ApplyCameraSettings(Settings.GetSettings());
        characterAnimationPlayer = new CharacterAnimationPlayer(this, idleAnimator.animator, idleAnimator.cameraView, IKPass);
    }

    public Player() {
        largeScale = 1.75f;
        normalScale = 1f;
        shrinkScale = .15f;
        animationDuration  = 1f;
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

    public Transform test;
    private void Reset() {
        Word = "me";
        AlternativeWords = new string[]{ "myself", "i", "player", "gregory", "you", "yourself", "self"};
    }
    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) Die(true, false);
        
        if (Input.GetKeyDown(KeyCode.F) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) fpCamera.ShowAimCutscene(test, 5f, 60f, 2f);
#endif
    }

    private void OnEnable() {
        PerspectiveProperty.onPerspectiveAppearing += HideHead;
        PerspectiveProperty.onPerspectiveMissing += ShowHead;
        SettingPanel.OnSave += ApplyCameraSettings;
        CharacterAnimationPlayer?.OnEnable();
    }
    private void OnDisable() {
        PerspectiveProperty.onPerspectiveAppearing -= HideHead;
        PerspectiveProperty.onPerspectiveMissing -= ShowHead;
        SettingPanel.OnSave -= ApplyCameraSettings;
        CharacterAnimationPlayer?.OnDisable();

    }

    ///<summary>
    /// Thep play dies and falls to the gorund
    ///</summary>
    public void Die(bool withAnimation = true, bool toPreviousLevel = false) {
        if (dead) return;
        dead = true;
        if (withAnimation) {
            characterAnimationPlayer.SetBool("dead", true);
            StartCoroutine(PlayDeadFallSound());
            characterAnimationPlayer.PlayCutSceneAnimation("dying");
        } else {
            movement.EnableRotation = false;
            movement.EnableWalk = false;
            AudioHandler.Instance.PlaySound(SFXFiles.player_fall_death);
        }
        hands.DetachObject();
        OnDie?.Invoke(withAnimation, toPreviousLevel);
    }
    public void DieWithoutAnimation() {
        Die(false);
    }

    private IEnumerator PlayDeadFallSound() {
        yield return new WaitForSeconds(.6f);
        AudioHandler.Instance.PlaySound(SFXFiles.player_hits_ground, 1f);
    }


    //TODO: maybe move this to the fpmovement class
    private void OnCollisionStay(Collision other) {
        if (other.transform.GetComponentInParent<ICollissionArea>() != null) {
            other.transform.GetComponentInParent<ICollissionArea>().OnColliderStay(this);
        }
        
        if (other.rigidbody != null) 
            if (other.rigidbody.mass > hands.MassThreshhold) {
                other.rigidbody.velocity = Vector3.zero;
                movement.RB.mass = 0.01f;
            }
    }
    private void OnCollisionEnter(Collision other) {
        if (other.transform.GetComponentInParent<ICollissionArea>() != null) {
            other.transform.GetComponentInParent<ICollissionArea>().OnColliderEnter(this);
        }
        if (other.rigidbody != null) 
            if (other.rigidbody.mass > hands.MassThreshhold) {
                other.rigidbody.velocity = Vector3.zero;
                movement.RB.mass = 0.01f;
            }
    }
    private void OnCollisionExit(Collision other) {
        if (other.transform.GetComponentInParent<ICollissionArea>() != null) {
            other.transform.GetComponentInParent<ICollissionArea>().OnColliderExit(this);
        }
        if (other.rigidbody != null) 
            if (other.rigidbody.mass > hands.MassThreshhold) 
                movement.RB.mass = CurrentScale;
    }

    public void ShowHead() {
        
        headLayer = mirrorHeadModel.gameObject.layer;
        mirrorHeadModel.gameObject.layer = 0;
    }
    public void HideHead() {
        if (headLayer == 0) headLayer = mirrorHeadModel.gameObject.layer;
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
        //no base call!
        Debug.Log("should be appearing" + Animated);
        IsMissing = false;
        OnPlayerAppear?.Invoke();
        foreach(SkinnedMeshRenderer mr in meshObjects) {
            mr.enabled = true;
        }        
        if (Animated) {
            StartCoroutine(AnimateAppearing());
        } else {
            OnAppearingFinish();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<IPortal>() != null) {
            other.GetComponent<IPortal>().OnPortalEnter(this);
        }
        if (other.GetComponentInParent<ITriggerArea>() != null) {
            other.GetComponentInParent<ITriggerArea>().OnAreaEnter(this);
        }
        
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<IPortal>() != null) {
            other.GetComponent<IPortal>().OnPortalLeave();
        }
        if (other.GetComponentInParent<ITriggerArea>() != null) {
            other.GetComponentInParent<ITriggerArea>().OnAreaExit(this);
        }
    }

    public override void OnShrinking()
    {
        IsShrinked = true;
        OnPlayerShrink?.Invoke();
        StartCoroutine(AnimateShrinking());
    }
    public override void OnShrinkingFinish()
    {
        small_collider.enabled = true;
        base.OnShrinkingFinish();
    }

    public override void OnShrinkRevert()
    {
        IsShrinked = false;
        small_collider.enabled = false;
        OnPlayerUnShrink?.Invoke();
        StartCoroutine(AnimateShrinkRevert());
    }


    public override void OnRoomEnter()
    {
        //no base call!
    }

    ///<summary>
    /// Enables the movement and sets the camera animation to false.
    ///</summary>
    public void Respawn() {
        OnRespawn?.Invoke();
        dead = false;
        Movement.RB.velocity = Vector3.zero;
        characterAnimationPlayer.SetBool("dead", false);
        characterAnimationPlayer.PlayCutSceneAnimation("standingUp", true);
        StartCoroutine(characterAnimationPlayer.StandingUp());
    }

    public void SetLevel2EndAnimation() {
        characterAnimationPlayer.SetAnimator(level2End);
        characterAnimationPlayer.PlayCutSceneAnimation("level2end", true);
    }
    public void SetFinalLevelAnimation() {
        characterAnimationPlayer.SetAnimator(finalLevelEnd);
        characterAnimationPlayer.PlayCutSceneAnimation("acceptance", true);
    }
    private void OnDrawGizmosSelected() {
        if (Application.isPlaying) fpCamera.DrawGizmo();
    }

    [System.Serializable]
    public class PlayerAnimator {
        public Animator animator;
        public Transform cameraView;
        public Transform[] props;
    }
}
