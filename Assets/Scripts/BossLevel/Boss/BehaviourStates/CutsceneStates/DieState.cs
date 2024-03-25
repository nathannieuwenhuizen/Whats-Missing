using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// The boss dies and does an die animation
///</summary>
namespace Boss {

    public class DieState : BossCutsceneState
    {
        public delegate void BossEvent();
        public static BossEvent OnBossDieStart;
        public static BossEvent OnBossDie;

        public override void Start()
            {
            base.Start();
            stateName = "Die cutscene";
            DialoguePlayer.Instance.PlayLine(BossLines.Boss_Die);
            AudioHandler.Instance.PlaySound(SFXFiles.boss_dying);

            // Positioner.MovementEnabled = false;
            // Positioner.RotationEnabled = false;

            //transport the boss to the end position
            // Vector3 aimDelta = (bossAI.ShieldDestroyPosition.position - bossAI.DiePosition.position).normalized;
            // bossAI.transform.position = bossAI.DiePosition.position + aimDelta * 5f;
            // Quaternion aim =  Quaternion.LookRotation(-aimDelta, Vector3.up);
            // bossAI.transform.rotation = aim;

            // Positioner.BodyMovementType = BodyMovementType.navMesh;
            // Positioner.BodyOrientation = BodyOrientation.toPath;

            // Positioner.SetDestinationPath(bossAI.DiePosition, bossAI.transform.position, false, 0);
            bossAI.StartCoroutine(bossAI.Boss.Body.UpdatingDisolve(11f));
            bossAI.Boss.AnimationDuration = 11f;
            bossAI.Boss.ShowDisovleParticles(false);
            // bossAI.StartCoroutine(Extensions.AnimateCallBack(0, 1f, AnimationCurve.EaseInOut))
            bossAI.StartCoroutine(DissolveParticlesUpdate());

            OnBossDieStart?.Invoke();
            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_DEATH, true, 11f, () => {
                OnBossDie?.Invoke();
                OnStateSwitch.Invoke(bossAI.Behaviours.idleState);
                SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.Anger);

            }));
        }

        private  IEnumerator DissolveParticlesUpdate(float duration = 6f) {
            yield return new WaitForSeconds (2f);
            // bossAI.DissolveParticles.transform.position = bossAI.transform.position + Vector3.down * 5f;
            bossAI.DissolveParticles.Play();
            float timePassed = 0f;
            while (timePassed < duration) {

                timePassed += Time.deltaTime;
                bossAI.DissolveParticles.emissionRate = (timePassed/duration) * 300;
               yield return new WaitForEndOfFrame();

           }
            bossAI.DissolveParticles.emissionRate = 0;
        }

        
    }
}
