using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class HugeAnticipationState : BaseBossState, IState
    {
        public Coroutine animationCoroutine;
        public float totalDuration = 5f;

    public override void DrawDebug()
        {
            base.DrawDebug();
        }
        public override void Start()
        {
            stateName = "Huge Anticipation";
            animationCoroutine = bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_HUGE_ANTICIPATION, false, 9.5f, () => {
                // OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
            }));
            bossAI.Boss.BossPositioner.MovementEnabled = false;
            bossAI.Boss.BossPositioner.RotationEnabled = false;

            bossAI.StartCoroutine(AnticipationTImeFrame());
        }

        public IEnumerator AnticipationTImeFrame() {
            yield return new WaitForSeconds(4f);
            BossCutsceneState.OnBossCutsceneEnd?.Invoke(bossAI.Boss);

            float index = 0;
            while(index < totalDuration) {
                index += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            DoHugeAttackWhileTImeStops();
        }

        public void KillPlayer() {
            DoAttackAnimation();
        }
        public IEnumerator ActivateTimeStopColliders() {
            // BossCutsceneState.OnBossCutsceneStart?.Invoke(bossAI.Boss);
            yield return new WaitForSeconds(5f);
            bossAI.BossTimeStopCollider.SetActive(true);
            // BossCutsceneState.OnBossCutsceneEnd?.Invoke(bossAI.Boss);

        }

        public void DoHugeAttackWhileTImeStops() {
            DoAttackAnimation();
            bossAI.StartCoroutine(ActivateTimeStopColliders());
        }
        
        public void DoAttackAnimation() {
            Body.BossAnimator.SetTrigger(BossAnimatorParam.TRIGGER_ATTACK, true);

            //setting boss pos
            bossAI.Boss.BossPositioner.MovementEnabled = true;
            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.toPlayer;
            bossAI.Boss.BossPositioner.BodyMovementType = BodyMovementType.freeFloat;

            Positioner.SpeedScale = 1f;
            Positioner.SetDestinationPath(bossAI.HugeAttackPos , bossAI.transform.position, true, 5f);

        }

        public override void Exit()
        {
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);
        }
    }

}