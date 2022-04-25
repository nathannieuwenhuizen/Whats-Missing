using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseBossState, IState
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
    }

    public void Run()
    {
    }

    public void Exit()
    {
        bossAI.Boss.Body.ToggleBody(true);
    }
}
