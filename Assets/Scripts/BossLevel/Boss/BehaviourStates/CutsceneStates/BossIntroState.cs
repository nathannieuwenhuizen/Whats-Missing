using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class BossIntroState : BossCutsceneState
    {


        public override void Start()
        {
            zoomValue = 70f;
            stateName = "Intro cutscene";
            base.Start();
            bossAI.StartCoroutine(MovingBoss());

            Positioner.InAir = true;
            Positioner.BodyMovementType = BodyMovementType.freeFloat;
            Body.Arm.Toggle(false);
            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_INTRO, true, 10f, () => {
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