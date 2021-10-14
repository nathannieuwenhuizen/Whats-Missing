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
    private Camera playerCamera;

    [SerializeField]
    private Volume volume;

    public Volume Volume { get => volume; }
    private MotionBlur motionBlur;

    public Camera Camera { get => playerCamera;}

    [SerializeField]
    private Animator cameraAnimator;

    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<FPMovement>();
        cameraAnimator.enabled = false;

        //set motionblur to settings.
        volume.profile.TryGet<MotionBlur>(out motionBlur);
        motionBlur.active = Settings.GetSettings().cameraSettings.Motion_blur_enabled;
    }

    private void Reset() {
        Word = "me";
        AlternativeWords = new string[]{ "myself", "i", "player", "edward"};
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            Die();
        }
    }

    ///<summary>
    /// Thep play dies and falls to the gorund
    ///</summary>
    public void Die() {
        cameraAnimator.enabled = true;
        cameraAnimator.SetTrigger("Die");
        Movement.EnableRotation = false;
        Movement.RB.velocity = Vector3.zero;
        Movement.EnableWalk = false;
        OnDie?.Invoke();
    }
    ///<summary>
    /// Enables the movement and sets the camera animation to false.
    ///</summary>
    public void Respawn() {
        cameraAnimator.enabled = false;
        Movement.EnableRotation = true;
        Movement.EnableWalk = true;

    }
}
