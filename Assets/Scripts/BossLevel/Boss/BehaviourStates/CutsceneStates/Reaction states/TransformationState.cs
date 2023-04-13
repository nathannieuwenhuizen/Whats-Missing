using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class TransformationState : BossReactionState
    {
        public override void Start()
        {
            zoomValue = 45f;
            Debug.Log("before start");
            base.Start();
            Debug.Log("after start");
            stateName = "Transformation cutscene";
        }

        public override Vector3 ReactionPosition() {
            Debug.Log("correct reaction pos");

            return bossAI.ReactionPosition.position;
        }

        public override IEnumerator GoToReactionPose() {
            yield return base.GoToReactionPose();
        }
        public Coroutine animationCoroutine;

        public override void DoReaction()
        {
            stateName = "Transformation cutscene start";
            Body.Metamorphose();

            base.DoReaction();

            AudioHandler.Instance?.Play3DSound(SFXFiles.boss_transformation, bossAI.BossHead.transform);
            bossAI.Boss.BossPositioner.BodyMovementType = BodyMovementType.freeFloat;
            animationCoroutine = bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_TRANSFORM, true, 6f, () => {
                bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                    bossAI.Boss.BossChangesHandler.CreateChange("fire", ChangeType.tooBig);
                }, () => {
                    OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
                }));
            }));
        }
        public override void Exit()
        {
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);
            base.Exit();
        }
    }
}