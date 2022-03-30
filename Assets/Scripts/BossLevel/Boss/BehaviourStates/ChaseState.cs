using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void DrawDebug()
    {

    }
    private float oldViewAngle;
    private float sharpViewAngle = 10;
    public void Start()
    {
        bossAI.BossHead.SteeringBehaviour.MaxForce *= 10f;
        oldViewAngle = bossAI.BossEye.ViewAngle;
        bossAI.BossEye.ViewAngle = sharpViewAngle;;
    }

    public void Run()
    {
        bossAI.BossHead.SetAim(bossAI.Boss.Player.Camera.transform.position, Vector2.zero);
        bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
        if (bossAI.BossEye.DoesntNoticesPlayer) {
            OnStateSwitch?.Invoke(bossAI.Behaviours.lookingState);
        }
    }

    public void Exit()
    {
        bossAI.BossHead.SteeringBehaviour.MaxForce /= 10f;
        bossAI.BossEye.ViewAngle = oldViewAngle;;

    }

}
