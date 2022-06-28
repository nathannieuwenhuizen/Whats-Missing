using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class TransformationState : BossReactionState
    {
        public override void Start()
        {
            zoomValue = 45f;
            stateName = "Transformation cutscene";
            base.Start();
        }

        public override void DoReaction()
        {
            stateName = "Transformation cutscene start";
            Body.Metamorphose();

            base.DoReaction();

            AudioHandler.Instance?.Play3DSound(SFXFiles.boss_transformation, bossAI.BossHead.transform);

            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_TRANSFORM, true, 6f, () => {
                bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                    bossAI.Boss.BossChangesHandler.CreateChange("fire", ChangeType.tooBig);
                }, () => {
                    OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
                }));
            }));
        }
    }
}