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

    [SerializeField]
    private Transform headModel;
    [SerializeField]
    private Transform animationView;

    [SerializeField]
    private Camera playerCamera;
    private float cameraClipPlane;

    [SerializeField]
    private Volume volume;

    public Volume Volume { get => volume; }
    private MotionBlur motionBlur;

    public Camera Camera { get => playerCamera;}

    [SerializeField]
    private SkinnedMeshRenderer meshObject;


    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected void Awake()
    {
        movement = GetComponent<FPMovement>();
        cameraClipPlane = playerCamera.nearClipPlane;

        //set motionblur to settings.
        volume.profile.TryGet<MotionBlur>(out motionBlur);
        motionBlur.active = Settings.GetSettings().cameraSettings.Motion_blur_enabled;
    }

    protected override Material[] getMaterials() {
        return meshObject.materials;
    }


    private void Reset() {
        Word = "me";
        AlternativeWords = new string[]{ "myself", "i", "player", "gregory"};
    }
    // private void Update() {
    //     if (Input.GetKeyDown(KeyCode.D)) {
    //         // Die();
    //         PlaycharacterAnimation("takingItem");

    //     }
    // }

    ///<summary>
    /// Thep play dies and falls to the gorund
    ///</summary>
    public void Die() {
        Movement.CharacterAnimator.SetBool("dead", true);

        PlaycharacterAnimation("dying");

        OnDie?.Invoke();
    }

    public void PlaycharacterAnimation(string trigger) {
        Movement.EnableRotation = false;
        Movement.EnableWalk = false;
        Movement.RB.velocity = Vector3.zero;

        playerCamera.transform.SetParent(headModel);
        playerCamera.transform.localPosition = animationView.localPosition;
        playerCamera.transform.localRotation = animationView.localRotation;
        playerCamera.nearClipPlane = 0.01f;
        Movement.CharacterAnimator.SetTrigger(trigger);
    }


    public override void OnMissingFinish()
    {
        //no base call!
        meshObject.enabled = false;
    }

    public override void OnAppearing()
    { 
        //no base call!
        meshObject.enabled = true;
        base.OnAppearing();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Portal>() != null) {
            other.GetComponent<Portal>().OnPortalEnter(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Portal>() != null) {
            other.GetComponent<Portal>().OnPortalLeave();
        }
    }



    ///<summary>
    /// Enables the movement and sets the camera animation to false.
    ///</summary>
    public void Respawn() {
        playerCamera.transform.SetParent(transform);
        playerCamera.nearClipPlane = cameraClipPlane;


        Movement.EnableRotation = true;
        Movement.EnableWalk = true;
        Movement.CameraPivot.transform.localPosition = new Vector3(0,Movement.CameraPivot.transform.localPosition.y,0);
        Movement.CharacterAnimator.SetBool("dead", false);

    }
}
