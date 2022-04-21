using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public class IdleState : IBaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public BossAI bossAI { get; set; }

    public void DrawDebug()
    {

    }
    public void Start()
    {
        bossAI.BossEye.LightIsOn = false;
        bossAI.Boss.Body.ToggleBody(false);
        bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
        bossAI.Boss.BossPositioner.MovementEnabled = false;
    }

    public void Run()
    {
    }

    public void Exit()
    {
        bossAI.Boss.BossPositioner.MovementEnabled = true;
        bossAI.Boss.Body.ToggleBody(true);
    }
}

}