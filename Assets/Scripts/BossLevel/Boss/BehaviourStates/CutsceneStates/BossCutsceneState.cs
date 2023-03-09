using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// At this state the boss will perform a cutscene animation. 
    ///The players own movement is set to zero as the player aims the camera to that of the boss.
    ///</summary>
    public class BossCutsceneState : BaseBossState, IState
    {
        public delegate void BossCutSceneDelegate(Boss boss, float zoomValue = 50f);
        public static BossCutSceneDelegate OnBossCutsceneStart;
        public static BossCutSceneDelegate OnBossCutsceneEnd;

        public delegate void BossCutSceneUpdateDelegate(Transform _target, Vector2 _offset, float speed);
        public static BossCutSceneUpdateDelegate OnBossCutsceneTargetUpdate;
        ///<summary>
        /// The zoom size of th camera when it goes into the cutscene animation.
        ///</summary>
        public float zoomValue = 50f;
        public bool withCutscene {
                get; set;
            } = true;

        public override void Start()
        {
            stateName = "Start cutscene";
            Boss.Head.StopBossVoice();
            if (withCutscene) OnBossCutsceneStart?.Invoke(bossAI.Boss, zoomValue);
            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.none;
            bossAI.Boss.BossPositioner.MovementEnabled = false;
            bossAI.Boss.BossPositioner.RotationEnabled = false;
        }

        public override void Exit()
        {
            // Boss.Head.PlayBossVoice();
            bossAI.Boss.BossPositioner.MovementEnabled = true;
            bossAI.Boss.BossPositioner.RotationEnabled = true;
            if (withCutscene) OnBossCutsceneEnd?.Invoke(bossAI.Boss);
        }
    }
}