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
            bossAI.StartCoroutine(TestAnimation());

            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_INTRO, true, 10f, () => {
                OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
            }));
        }
        public IEnumerator TestAnimation() {
            yield return new WaitForSeconds(2f);
            Boss.Head.LookAtPlayer = true;
            Vector3 start = bossAI.Boss.transform.position;
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,10,-20), AnimationCurve.EaseInOut(0,0,1,1), 6f);
            // yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,40,-20), AnimationCurve.EaseInOut(0,0,1,1), 3f);
            // yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(-30,30,0), AnimationCurve.EaseInOut(0,0,1,1), 4f);

            // OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}