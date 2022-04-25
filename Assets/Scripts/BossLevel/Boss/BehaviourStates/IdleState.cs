using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public class IdleState : BaseBossState, IState
{
    public override void DrawDebug()
    {
        base.DrawDebug();
    }
    public override void Start()
    {
        stateName = "Idle";

        bossAI.BossEye.LightIsOn = false;
        bossAI.Boss.Body.ToggleBody(false);
        bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
        bossAI.Boss.BossPositioner.MovementEnabled = false;
    }

    public override void Run()
    {
    }

    public override void Exit()
    {
        bossAI.Boss.BossPositioner.MovementEnabled = true;
        bossAI.Boss.Body.ToggleBody(true);
    }
}

}