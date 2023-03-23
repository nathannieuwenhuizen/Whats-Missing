using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class HugeAnticipationState : BaseBossState, IState
    {
        public Coroutine animationCoroutine;
        public float totalDuration = 33f;
        private bool timeStops = false;

        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        private void OnTimeMissing() {
            DoHugeAttackWhileTImeStops();
        }

        public override void Start()
        {
            TimeProperty.onTimeMissing += OnTimeMissing;
            BossRoom.OnRespawn += ResetState;


            stateName = "Huge Anticipation";

            StartHugeAnticipationAnimation();
        }
        private void StartHugeAnticipationAnimation() {
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);

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
            bossAI.StartCoroutine(KillPlayer());
        }

        public IEnumerator KillPlayer() {
            DoAttackAnimation();
            yield return new WaitForSeconds(1f);
            if (!timeStops) bossAI.Boss.Player.Die(false);
        }

        public IEnumerator ActivateTimeStopColliders() {
            // BossCutsceneState.OnBossCutsceneStart?.Invoke(bossAI.Boss);
            // yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(.9f);
            bossAI.TimeStopDebrees.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            bossAI.BossTimeStopCollider.SetActive(true);
            // BossCutsceneState.OnBossCutsceneEnd?.Invoke(bossAI.Boss);

        }

        public void DoHugeAttackWhileTImeStops() {
            timeStops = true;
            DoAttackAnimation();
        }
        
        public void DoAttackAnimation() {
            bossAI.StartCoroutine(ActivateTimeStopColliders());
            Body.BossAnimator.SetTrigger(BossAnimatorParam.TRIGGER_ATTACK, true);

            //setting boss pos
            bossAI.Boss.BossPositioner.MovementEnabled = true;
            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.toPlayer;
            bossAI.Boss.BossPositioner.BodyMovementType = BodyMovementType.freeFloat;

            Positioner.SpeedScale = 1f;
            Positioner.SetDestinationPath(bossAI.HugeAttackPos , bossAI.transform.position, true, 5f);
            Boss.Body.BossAnimator.DoHugeSlashArmFX();

        }

        public void ResetState(bool wthColor) {
            bossAI.TimeStopDebrees.gameObject.SetActive(false);
            bossAI.BossTimeStopCollider.SetActive(true);
            StartHugeAnticipationAnimation();
        }

        public override void Exit()
        {
            TimeProperty.onTimeMissing -= OnTimeMissing;
            BossRoom.OnRespawn -= ResetState;
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);
        }
    }

}