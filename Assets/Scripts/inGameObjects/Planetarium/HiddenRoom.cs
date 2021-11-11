using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenRoom : AreaTrigger
{
    public override void OnAreaEnter()
    {
        base.OnAreaEnter();
        AudioHandler.Instance.FadeMusic(MusicFiles.planetarium_hidden_room, 1f);
    }
    public override void OnAreaExit()
    {
        base.OnAreaExit();
        AudioHandler.Instance.FadeMusic(MusicFiles.planetarium, 1f);

    }
}
