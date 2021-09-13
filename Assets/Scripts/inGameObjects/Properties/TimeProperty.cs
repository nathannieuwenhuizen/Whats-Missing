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
        AudioHandler.Instance.MusicSource.volume = 1f;
        AudioHandler.Instance.MusicSource.pitch = .2f;
        room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().enabled.value = true;

    }
    public override void onAppearing()
    {
        base.onAppearing();
        Time.timeScale = 1;
        AudioHandler.Instance.MusicSource.volume = 1f;
        AudioHandler.Instance.MusicSource.pitch = AudioHandler.Instance.MusicVolume;
        room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().enabled.value = false;


    }
}
