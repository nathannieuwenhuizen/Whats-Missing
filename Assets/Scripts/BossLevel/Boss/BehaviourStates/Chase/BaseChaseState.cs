using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public class BaseChaseState : BaseBossState
{

    public override void DrawDebug()
    {
        base.DrawDebug();
    }
    private float oldViewAngle;
    private float sharpViewAngle = 8;
    public override void Start()
    {
        bossAI.BossHead.SteeringBehaviour.MaxForce *= 20f;
        oldViewAngle = bossAI.BossEye.ViewAngle;

        bossAI.BossEye.AnimateViewAngle(sharpViewAngle);
    }

    public override void Run()
    {
        bossAI.BossHead.SetAim(bossAI.Boss.Player.Camera.transform.position, Vector2.zero);
        bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
        if (bossAI.BossEye.DoesntNoticesPlayer) {
            // OnStateSwitch?.Invoke(bossAI.Behaviours.lookingState);
        }
    }

    public override void Exit()
    {
        bossAI.BossHead.SteeringBehaviour.MaxForce /= 20f;
        bossAI.BossEye.AnimateViewAngle(oldViewAngle);
    }

}

}