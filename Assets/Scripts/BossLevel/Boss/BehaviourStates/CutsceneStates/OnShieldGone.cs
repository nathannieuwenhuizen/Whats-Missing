using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class OnShieldGone : BossCutsceneState
    {
        public override void Start()
        {
            stateName = "Shield gone cutscene";
            base.Start();
            bossAI.StartCoroutine(TestAnimation());
            Boss.Head.LookAtPlayer = true;
        }
        public IEnumerator TestAnimation() {
            yield return new WaitForEndOfFrame();
            Vector3 start = bossAI.Boss.transform.position;
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,50,0), AnimationCurve.EaseInOut(0,0,1,1), 1.5f);

            OnStateSwitch?.Invoke(bossAI.Behaviours.crawlingChaseState);
        }
    }
}
