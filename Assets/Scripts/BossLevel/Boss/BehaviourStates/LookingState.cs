using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
///<summary>
/// In this state, the boss is wandering trying to find the player.
/// The boss will move arround the mountain and tries to find the player
///</summary>
public class LookingState : IBaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public BossAI bossAI { get; set; }

    protected BossEye eye;
    private Player player;

    public virtual void DrawDebug()
    {
        if (eye == null) return;

        Color debugColor = Color.Lerp(Color.red, Color.green, eye.NoticingValue / eye.NoticingThreshold);
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(eye.transform.position, .5f * (eye.NoticingValue / eye.NoticingThreshold));
        Gizmos.DrawSphere(eye.transform.position, .5f);
    }
    public virtual void Start()
    {
        eye = bossAI.BossEye;
    }

    public virtual void Exit()
    {

    }

    public virtual void Run()
    {
        bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
        if (bossAI.BossEye.NoticesPlayer) {
            OnStateSwitch?.Invoke(bossAI.Behaviours.chaseState);
        }
    }

}
}