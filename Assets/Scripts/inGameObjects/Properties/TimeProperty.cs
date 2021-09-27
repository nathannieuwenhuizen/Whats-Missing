using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeProperty : Property
{

    [SerializeField]
    private Room room;

    private Vignette vignette;
    public override void onMissing()
    {
        base.onMissing();
        Room.TimeScale = 0f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.DeactivateRigidBody();
        }

        AudioHandler.Instance.MusicSource.volume = 1f;
        AudioHandler.Instance.MusicSource.pitch = .2f;
        // room.Player.Volume.profile.TryGet<Vignette>(out vignette);
        // vignette.enabled.value = true;

    }
    public override void onAppearing()
    {
        base.onAppearing();
        Room.TimeScale = 1f;
        foreach(PickableRoomObject obj in room.GetAllObjectsInRoom<PickableRoomObject>()) {
            obj.RigidBodyInfo.UseGravity = true;
            obj.ActivateRigidBody();
        }

        Time.timeScale = 1;
        AudioHandler.Instance.MusicSource.volume = 1f;
        AudioHandler.Instance.MusicSource.pitch = AudioHandler.Instance.MusicVolume;
        // room.Player.Volume.profile.TryGet<Vignette>(out vignette);
        // vignette.enabled.value = true;
    }
    private void Reset() {
        Word = "time";
    }

}
