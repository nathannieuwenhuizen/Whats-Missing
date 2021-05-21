using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeProperty : Property
{

    [SerializeField]
    private Room room;

    public override void onMissing()
    {
        base.onMissing();
        Time.timeScale = 0f;
        MusicObject.INSTANCE.Volume = 1f;
        MusicObject.INSTANCE.Pitch = .2f;
        room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().enabled.value = true;

    }
    public override void onAppearing()
    {
        base.onAppearing();
        Time.timeScale = 1;
        MusicObject.INSTANCE.Volume = MusicObject.INSTANCE.startVolume;
        MusicObject.INSTANCE.Pitch = 1f;
        room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().enabled.value = false;


    }
}
