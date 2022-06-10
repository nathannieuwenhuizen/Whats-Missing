using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class BossCutsceneState : BaseBossState, IState
    {
        public delegate void BossCutSceneDelegate(Boss boss, float zoomValue = 50f);
        public static BossCutSceneDelegate OnBossCutsceneStart;
        public static BossCutSceneDelegate OnBossCutsceneEnd;
        public float zoomValue = 50f;
        public bool withCutscene {
                get; set;
            } = true;
        public override void DrawDebug()
        {
            base.DrawDebug();
        }
        public override void Start()
        {
            stateName = "Cutscene";
            if (withCutscene) OnBossCutsceneStart?.Invoke(bossAI.Boss, zoomValue);
            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
            bossAI.Boss.BossPositioner.MovementEnabled = false;
        }

        public override void Run()
        {

        }

        public override void Exit()
        {
            bossAI.Boss.BossPositioner.MovementEnabled = true;
            if (withCutscene) OnBossCutsceneEnd?.Invoke(bossAI.Boss);
        }
    }
}