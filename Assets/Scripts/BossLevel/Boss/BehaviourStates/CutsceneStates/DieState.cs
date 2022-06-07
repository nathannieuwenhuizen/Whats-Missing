using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// The boss dies and does an die animation
///</summary>
namespace Boss {

    public class DieState : BossCutsceneState
    {
    public override void Start()
        {
            stateName = "Die cutscene";
            base.Start();
            bossAI.StartCoroutine(TestAnimation());
            Boss.Head.LookAtPlayer = false;

            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_DEATH, true, 10f, () => {
                // OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
            }));

        }
        public IEnumerator TestAnimation() {
            yield return new WaitForEndOfFrame();
            Vector3 start = bossAI.Boss.transform.position;
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,50,0), AnimationCurve.EaseInOut(0,0,1,1), 1.5f);

            OnStateSwitch?.Invoke(bossAI.Behaviours.idleState);
        }
    }
}
