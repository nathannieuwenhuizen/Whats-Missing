using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class BossIntroState : BossCutsceneState
    {
        public override void Start()
        {
            stateName = "Intro cutscene";
            base.Start();
            bossAI.StartCoroutine(TestAnimation());
            Boss.Head.LookAtPlayer = true;

            
        }
        public IEnumerator TestAnimation() {
            yield return new WaitForEndOfFrame();
            Vector3 start = bossAI.Boss.transform.position;
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,50,0), AnimationCurve.EaseInOut(0,0,1,1), 1.5f);
            yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,40,-20), AnimationCurve.EaseInOut(0,0,1,1), 3f);
            // yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(-30,30,0), AnimationCurve.EaseInOut(0,0,1,1), 4f);

            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}