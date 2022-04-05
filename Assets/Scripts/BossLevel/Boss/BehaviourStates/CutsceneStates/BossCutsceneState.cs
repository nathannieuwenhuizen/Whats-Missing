using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCutsceneState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public delegate void BossCutSceneDelegate(Boss boss);
    public static BossCutSceneDelegate OnBossCutsceneStart;
    public static BossCutSceneDelegate OnBossCutsceneEnd;
    
    public virtual void DrawDebug()
    {

    }
    public virtual void Start()
    {
        OnBossCutsceneStart?.Invoke(bossAI.Boss);
    }

    public virtual void Run()
    {

    }

    public virtual void Exit()
    {
        OnBossCutsceneEnd?.Invoke(bossAI.Boss);
    }

}
