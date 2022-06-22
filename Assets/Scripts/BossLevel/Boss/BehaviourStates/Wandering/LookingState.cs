using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
///<summary>
/// In this state, the boss is wandering trying to find the player.
/// The boss will move arround the mountain and tries to find the player
///</summary>
public abstract class LookingState : BaseBossState, IState
{
    protected BossEye eye;

    public override void DrawDebug()
    {
        base.DrawDebug();

        if (eye == null) return;

        Color debugColor = Color.Lerp(Color.red, Color.green, eye.NoticingValue / eye.NoticingThreshold);
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(eye.transform.position, .5f * (eye.NoticingValue / eye.NoticingThreshold));
        Gizmos.DrawSphere(eye.transform.position, .5f);
    }
    public override void Start()
    {
        stateName = "Looking";
        eye = bossAI.BossEye;
    }

    public override void Exit()
    {

    }

    protected virtual void BeginChaseOnGround() {
        // bossAI.Behaviours.landingState.nextState = bossAI.Behaviours.crawlingChaseState;
        // OnStateSwitch?.Invoke(bossAI.Behaviours.landingState);
    }

    protected virtual void BeginChase() {

        //go into crawling chase phase
        if (Positioner.BodyMovementType == BodyMovementType.navMesh) {
            OnStateSwitch?.Invoke(bossAI.Behaviours.crawlingChaseState);
            return;
        }

        //checks if player is inside the mountain, if not, then it will go into the air chase state
        if (Positioner.BossMountain.InsideShape(bossAI.Boss.Player.transform.position)) 
            OnStateSwitch?.Invoke(bossAI.Behaviours.mountainAirChaseState);
        else OnStateSwitch?.Invoke(bossAI.Behaviours.airChaseState);
    }


    public override void Run()
    {
        bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
        if (bossAI.BossEye.NoticesPlayer) {
            BeginChase();
        }
    }

}
}