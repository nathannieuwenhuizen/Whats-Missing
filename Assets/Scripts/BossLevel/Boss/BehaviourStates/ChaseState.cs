using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseBossState, IState
{
        public BossAI bossAI { get; set; }

    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void DrawDebug()
    {

    }
    private float oldViewAngle;
    private float sharpViewAngle = 8;
    public void Start()
    {
        bossAI.BossHead.SteeringBehaviour.MaxForce *= 20f;
        oldViewAngle = bossAI.BossEye.ViewAngle;

        bossAI.BossEye.AnimateViewAngle(sharpViewAngle);
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
        bossAI.BossHead.SteeringBehaviour.MaxForce /= 20f;
        bossAI.BossEye.AnimateViewAngle(oldViewAngle);
    }

}
