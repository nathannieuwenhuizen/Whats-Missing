using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public class BossCutsceneState : BaseBossState, IState
{
    public delegate void BossCutSceneDelegate(Boss boss);
    public static BossCutSceneDelegate OnBossCutsceneStart;
    public static BossCutSceneDelegate OnBossCutsceneEnd;
    
    public override void DrawDebug()
    {
        base.DrawDebug();
    }
    public override void Start()
    {
        stateName = "Cutscene";

        OnBossCutsceneStart?.Invoke(bossAI.Boss);
        bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
        bossAI.Boss.BossPositioner.MovementEnabled = false;
    }

    public override void Run()
    {

    }

    public override void Exit()
    {
        bossAI.Boss.BossPositioner.MovementEnabled = true;
        OnBossCutsceneEnd?.Invoke(bossAI.Boss);
    }

}
}