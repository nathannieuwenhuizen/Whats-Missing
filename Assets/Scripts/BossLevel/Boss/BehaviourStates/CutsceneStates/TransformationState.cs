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
            bossAI.StartCoroutine(Transforming());
            AudioHandler.Instance?.Play3DSound(SFXFiles.boss_transformation, bossAI.BossHead.transform);
            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_TRANSFORM, true, 6f, () => {
                bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                    bossAI.Boss.BossChangesHandler.CreateChange("fire", ChangeType.tooBig);
                }, () => {
                    OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
                }));
            }));
        }
        ///<summary>
        /// Boss does the transformation, applying the fire change and transforming their body to get extra tentacles.
        ///</summary>
        private IEnumerator Transforming() {
            yield return new WaitForSeconds(4.2f);
            Boss.Body.Arm.Toggle(true);
            // OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}