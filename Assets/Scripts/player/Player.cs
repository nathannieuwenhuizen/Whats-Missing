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


    private FPMovement movement;
    public FPMovement Movement { get=> movement; }
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<FPMovement>();

        //set motionblur to settings.
        volume.profile.TryGet<MotionBlur>(out motionBlur);
        motionBlur.active = Settings.GetSettings().cameraSettings.Motion_blur_enabled;
    }

    private void Reset() {
        Word = "me";
        AlternativeWords = new string[]{ "myself", "i", "player", "edward"};
    }

    public void Die() {
        OnDie?.Invoke();
    }
}
