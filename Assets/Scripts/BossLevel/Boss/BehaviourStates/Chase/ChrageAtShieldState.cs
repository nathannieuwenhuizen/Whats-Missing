using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {

public class ChrageAtShieldState : BaseChaseState
{
    private BossPositioner positioner;


    public override void DrawDebug()
    {
        base.DrawDebug();
    }

    public override void Start()
    {
        stateName = "Chase";

        base.Start();
        positioner = bossAI.Boss.BossPositioner;
        positioner.BodyOrientation = BodyOrientation.toPath;
        bossAI.StartCoroutine(Extensions.AnimateCallBack(1f, 10f, AnimationCurve.EaseInOut(0,0,1,1), (float _val) => {
            positioner.SpeedScale = _val;
        }, 2f));
    }

    public override void Run()
    {
        base.Run();

        if (positioner.isAtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) {
            Debug.Log("Attack!");
        }
    }

    public override void Exit()
    {
        positioner.SpeedScale = 1f;
        base.Exit();
    }
}
}