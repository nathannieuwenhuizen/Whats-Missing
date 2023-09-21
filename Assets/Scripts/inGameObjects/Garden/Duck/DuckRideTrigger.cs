using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckRideTrigger : AreaTrigger
{
    [SerializeField]
    private Duck duck;
    public override void OnAreaEnter (Player player) {
        base.OnAreaEnter(player);
        if (duck.IsEnlarged) {
            Debug.Log("quack!");
            SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.QUACK);
        }
    }

}
