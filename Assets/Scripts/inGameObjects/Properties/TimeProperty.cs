using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeProperty : Property
{

    public static OnPropertyToggle onTimeMissing;
    public static OnPropertyToggle onTimeAppearing;

    [SerializeField]
    private Room room;

    private Vignette vignette;
    public override void OnMissing()
    {
        base.OnMissing();
        Room.TimeScale = 0f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.DeactivateRigidBody();
        }

        onTimeMissing?.Invoke();
        AudioHandler.Instance.pitchMultiplier = .5f;

    }
    public override void OnAppearing()
    {
        base.OnAppearing();
        Room.TimeScale = 1f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.RigidBodyInfo.UseGravity = true;
            obj.ActivateRigidBody();
        }

        onTimeAppearing?.Invoke();
        AudioHandler.Instance.pitchMultiplier = 1f;
    }
    private void Reset() {
        Word = "time";
        AlternativeWords = new string[] { "duration" };
    }

}
