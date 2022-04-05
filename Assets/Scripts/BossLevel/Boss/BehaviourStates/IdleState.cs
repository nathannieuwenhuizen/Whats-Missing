using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void DrawDebug()
    {

    }
    public void Start()
    {
        bossAI.BossEye.LightIsOn = false;
    }

    public void Run()
    {
    }

    public void Exit()
    {

    }
}
