using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBossState{
    public BossAI bossAI;
}


public class LookingState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void Exit()
    {
    }

    public void Run()
    {
    }

    void IState.Start()
    {
    }
}
