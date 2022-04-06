using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroState : BossCutsceneState
{
        public BossAI bossAI { get; set; }

    public override void Start()
    {
        base.Start();
        bossAI.StartCoroutine(TestAnimation());
    }
    public IEnumerator TestAnimation() {
        yield return new WaitForEndOfFrame();
        Vector3 start = bossAI.Boss.transform.position;
        yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(0,50,0), AnimationCurve.EaseInOut(0,0,1,1), 1.5f);
        yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(-20,40,0), AnimationCurve.EaseInOut(0,0,1,1), 3f);
        yield return bossAI.Boss.transform.AnimatingPos(start +  new Vector3(-30,30,0), AnimationCurve.EaseInOut(0,0,1,1), 4f);

        OnStateSwitch?.Invoke(bossAI.Behaviours.lookingState);
    }
}