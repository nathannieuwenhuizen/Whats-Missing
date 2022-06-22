using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss{
    public class RemoveAirState : BossCutsceneState
    {
        public override void Start()
        {
            stateName = "Remove air cutscene";
            base.Start();
            //set the rotation to player and also on
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            bossAI.Boss.BossPositioner.RotationEnabled = true;

            bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                bossAI.Boss.BossChangesHandler.CreateChange("air", ChangeType.missing);
            }, () => {
                OnStateSwitch?.Invoke(bossAI.Behaviours.crawlingChaseState);
            }));

        }
    }
}
