using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class BossIntroState : BossCutsceneState
    {

        public delegate void BossIntroDelegate();
        public static BossIntroDelegate OnBossIntroStart;

        public override void Start()
        {
            zoomValue = 70f;
            base.Start();
            stateName = "Intro cutscene";
            bossAI.StartCoroutine(MovingBoss());
            OnBossIntroStart?.Invoke();

            Positioner.InAir = true;
            Positioner.BodyMovementType = BodyMovementType.freeFloat;
            Body.Arm.Toggle(false);
            bossAI.StartCoroutine(Talking());
            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_INTRO, true, 10f, () => {
                // DialoguePlayer.Instance.PlayLine(BossLines.Intro_3);
                OnStateSwitch?.Invoke(bossAI.Behaviours.chargeAtShieldState);
            }));
        }
        public IEnumerator MovingBoss() {
            yield return new WaitForSeconds(2f);
            Boss.Head.LookAtPlayer = true;
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.RotationEnabled = true;
            Vector3 start = bossAI.Boss.transform.position;
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(15,15f,10), AnimationCurve.EaseInOut(0,0,1,1), 6f);
            // yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,10f,-20), AnimationCurve.EaseInOut(0,0,1,1), 6f);
        }

        public IEnumerator Talking() {
            yield return new WaitForSeconds(2f);
            DialoguePlayer.Instance.PlayLine(BossLines.Intro_2, true, SFXFiles.boss_general_talking);
            yield return new WaitForSeconds(5f);
            DialoguePlayer.Instance.PlayLine(BossLines.Intro_3, true, SFXFiles.boss_general_talking);
            yield return new WaitForSeconds(5f);
            DialoguePlayer.Instance.PlayLine(BossLines.Intro_shield, true, SFXFiles.boss_general_talking);
            yield return new WaitForSeconds(5f);
            DialoguePlayer.Instance.PlayLine(BossLines.Intro_shield_2, true, SFXFiles.boss_general_talking);
        }




        ///<summary>
        /// Reason for this is so that the boss first does an charge at shield attack before the end of the cutscene.
        ///</summary>
        public override void Exit()
        {
            bossAI.StartCoroutine(Exitting());
        }
        public IEnumerator Exitting() {
            yield return new WaitForSeconds(5f);
            base.Exit();
        }
    }
}