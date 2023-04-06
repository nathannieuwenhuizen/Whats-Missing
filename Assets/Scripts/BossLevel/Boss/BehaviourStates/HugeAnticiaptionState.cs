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

            bossAI.StartCoroutine(AnticipationTimeFrame());
        }

        public IEnumerator AnticipationTimeFrame() {
            yield return new WaitForSeconds(4f);
            BossCutsceneState.OnBossCutsceneEnd?.Invoke(bossAI.Boss);
            yield return new WaitForSeconds(1f);
            bossAI.BossChargeParticle.gameObject.SetActive(true);
            bossAI.BossChargeParticle.PlayEmmission();

            float index = 0;
            while(index < totalDuration && !timeStops) {
                index += Time.deltaTime;
                bossAI.BossChargeParticle.Interpolate(index / totalDuration);
                bossAI.Boss.Body.Glow = 1 + 2f - 2f * Mathf.Abs(Mathf.Cos(Mathf.PI *  index * (.5f + index * .1f)));
                yield return new WaitForEndOfFrame();
            }
            if (!timeStops) bossAI.StartCoroutine(KillPlayer());
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
            bossAI.BossChargeParticle.StopTime();
            yield return new WaitForSeconds(2f);
            bossAI.BossTimeStopCollider.SetActive(true);
            // BossCutsceneState.OnBossCutsceneEnd?.Invoke(bossAI.Boss);

        }

        public void DoHugeAttackWhileTImeStops() {
            timeStops = true;
            DoAttackAnimation();
        }
        
        public void DoAttackAnimation() {
            bossAI.Boss.Body.Glow = 5;

            // bossAI.BossChargeParticle.StopEmmission();//.SetActive(false);

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
            bossAI.BossChargeParticle.StopEmmission();
            bossAI.TimeStopDebrees.gameObject.SetActive(false);
            bossAI.BossTimeStopCollider.SetActive(true);
            StartHugeAnticipationAnimation();
        }

        public override void Exit()
        {
            TimeProperty.onTimeMissing -= OnTimeMissing;
            BossRoom.OnRespawn -= ResetState;
            if (bossAI.TimeStopDebrees.isActiveAndEnabled) bossAI.TimeStopDebrees.TimeScale = 2f;
            bossAI.BossChargeParticle.TimeScale = 4f;
            bossAI.BossChargeParticle.StopEmmission();

            bossAI.BossTimeStopCollider.SetActive(false);
            Boss.Body.BossAnimator.Attacking = false;
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);
        }
    }

}