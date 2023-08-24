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
            DialoguePlayer.Instance.PlayLine(BossLines.Transform, true);

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
                DialoguePlayer.Instance.PlayLine(BossLines.NowBurn, true);
                bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                    AudioHandler.Instance?.Play3DSound(SFXFiles.boss_shockwave, bossAI.BossHead.transform);
                    bossAI.Boss.BossChangesHandler.CreateChange("fire", ChangeType.tooBig);
                }, () => {
                    OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
                    DialoguePlayer.Instance.PlayLine(BossLines.Hahaha, true);

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