using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// At this state, the boss will perform a magic attack to destroy the shield
    ///</summary>
    public class EnlargeState : BossReactionState
    {
        private float enlargeDuration = 2f;
        public override void Start()
        {
            customMountainShape = true;
            includeLanding = false;
            zoomValue = 40f;
            base.Start();
            stateName = "Enlarge cutscene";
            Boss.Body.Arm.Toggle(true); // purely for testing
            DialoguePlayer.Instance.PlayLine(BossLines.BeforeGrowth);

        }

        public override Vector3 ReactionPosition() {
            return bossAI.EnlargePosition.position;
        }

        public override void DoReaction()
        {
            stateName = "Enlarge cutscene";
            base.DoReaction();


            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_ENLARGE_START, false, 5f, () => {
                // OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
                bossAI.BossHead.LookAtPlayer = true;
                bossAI.BossHead.SteeringEnabled = true;
                bossAI.StartCoroutine(AfterGrwonig());
                DialoguePlayer.Instance.PlayLine(BossLines.AfterGrowth);
            }));
            bossAI.StartCoroutine(UpdateBody());
            bossAI.StartCoroutine(Enlarging());
        }
        public IEnumerator UpdateBody() {
            float index = 0;
            while (index < 5f ) {
                bossAI.Boss.Body.UpdateBodyShaders();
                // Debug.Log("update shader = " + bossAI.Boss.Body.Glow);
                index += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            // bossAI.Boss.Body.Glow = 5f;
        }
        public IEnumerator Enlarging() {
            yield return new WaitForSeconds(1f);
            Boss.AnimationDuration = enlargeDuration;
            Boss.AddChange(new MirrorChange() {
                word = "spirit",
                changeType = ChangeType.tooBig,
                active = true
            });

            //setting boss pos
            Positioner.SpeedScale = 1f;
            Positioner.SteeringBehaviour.MaxForce *= 1f;
            Positioner.SetDestinationPath(ReactionPosition() + Vector3.up * -20f, bossAI.transform.position, true, 5f);

            //camera zoom
            bossAI.Boss.Player.CharacterAnimationPlayer.ZoomDuration = enlargeDuration;
            bossAI.Boss.Player.CharacterAnimationPlayer.CameraZoom = 90f;
            bossAI.Boss.Player.CharacterAnimationPlayer.ZoomDuration = .5f;

            //camera new target
            yield return new WaitForSeconds(.5f);
            OnBossCutsceneTargetUpdate?.Invoke(Boss.Eye.transform, new Vector2(0, 20f), 1f);
        }
        public override void Exit()
        {
            Positioner.BossMountain.RestoreShape();
            base.Exit();
        }

        public IEnumerator AfterGrwonig() {
            yield return new WaitForSeconds(2f);
            OnStateSwitch(bossAI.Behaviours.hugeAnticiaptionState);
        }

    }
}
