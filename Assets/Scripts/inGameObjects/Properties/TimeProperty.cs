using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeProperty : Property
{

    public static OnPropertyToggle onTimeMissing;
    public static OnPropertyToggle onTimeAppearing;

    [SerializeField]
    private Room room;

    private Vignette vignette;

    private void Awake() {
        Room.TimeScale = 1f;
    }
    public override void OnMissing()
    {
        base.OnMissing();
        Room.TimeScale = 0f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.RigidBody.isKinematic = true;
            // obj.DeactivateRigidBody();
        }

        onTimeMissing?.Invoke();
        AudioHandler.Instance.pitchMultiplier = .5f;

    }

    public TimeProperty() {
        largeScale = 10f;
    }
    public override void OnAppearing()
    {
        base.OnAppearing();
        Room.TimeScale = 1f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.RigidBodyInfo.IsKinematic = false;
            obj.RigidBodyInfo.UseGravity = true;
            obj.ActivateRigidBody();
        }

        onTimeAppearing?.Invoke();
        AudioHandler.Instance.pitchMultiplier = 1f;
    }
    private void OnDisable() {
        if (InSpace) Room.TimeScale = 1f;
    }

    public override void OnEnlarge()
    {
        Room.TimeScale = largeScale;
        onTimeAppearing?.Invoke();
        base.OnEnlarge();
    }

    public override void OnEnlargeRevert()
    {
        Room.TimeScale = 1f;
        onTimeAppearing?.Invoke();
        base.OnEnlargeRevert();
    }

    public override void OnShrinking()
    {
        Room.TimeScale = shrinkScale;
        base.OnShrinking();
    }

    public override void OnShrinkRevert()
    {
        Room.TimeScale = 1f;
        base.OnShrinkRevert();
    }

    private void Reset() {
        Word = "time";
        AlternativeWords = new string[] { "duration" };
    }

}
