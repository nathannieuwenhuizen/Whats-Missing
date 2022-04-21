using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public class BossCutsceneState : IBaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public BossAI bossAI { get; set; }

    public delegate void BossCutSceneDelegate(Boss boss);
    public static BossCutSceneDelegate OnBossCutsceneStart;
    public static BossCutSceneDelegate OnBossCutsceneEnd;
    
    public virtual void DrawDebug()
    {

    }
    public virtual void Start()
    {
        OnBossCutsceneStart?.Invoke(bossAI.Boss);
        bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
        bossAI.Boss.BossPositioner.MovementEnabled = false;
    }

    public virtual void Run()
    {

    }

    public virtual void Exit()
    {
        bossAI.Boss.BossPositioner.MovementEnabled = true;
        OnBossCutsceneEnd?.Invoke(bossAI.Boss);
    }

}
}