using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenRoom : AreaTrigger
{
    public override void OnAreaEnter(Player player)
    {
        if (InsideArea) return;
        base.OnAreaEnter(player);
        AudioHandler.Instance.FadeMusic(MusicFiles.planetarium_hidden_room, 1f);
    }
    public override void OnAreaExit(Player player)
    {
        if (!InsideArea) return;
        base.OnAreaExit(player);
        AudioHandler.Instance.FadeMusic(MusicFiles.planetarium, .5f);

    }
}
