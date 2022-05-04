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
    private float oldViewAlpha;
    private float sharpViewAngle = 15f;
    private bool showCutsceneBeforeChase = false;
    public override void Start()
    {
        // bossAI.BossHead.SteeringBehaviour.MaxForce *= 20f;
        bossAI.BossHead.SteeringEnabled = false;
        oldViewAngle = bossAI.BossEye.ViewAngle;
        oldViewAlpha = bossAI.BossEye.ViewAlpha;

        bossAI.BossEye.AnimateViewAngle(sharpViewAngle);
        bossAI.BossEye.AnimateViewAlpha(0);
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
        // bossAI.BossHead.SteeringBehaviour.MaxForce /= 20f;
        bossAI.BossHead.SteeringEnabled = true;

        bossAI.BossEye.AnimateViewAngle(oldViewAngle);
        bossAI.BossEye.AnimateViewAlpha(oldViewAlpha);
        bossAI.BossEye.noticingValue = 0;
    }
        private bool isAttacking = false;
        protected void Attack() {
            if (isAttacking) return;
            bossAI.StartCoroutine(Attacking());
        }

        protected IEnumerator Attacking() {
            isAttacking = true;
            Boss.BossPositioner.MovementEnabled = false;
            bossAI.Boss.Body.ToggleDeathColliders(true);
            yield return bossAI.StartCoroutine(Boss.Body.BossAnimator.DoAttackAnimation());
            bossAI.Boss.Body.ToggleDeathColliders(false);
            isAttacking = false;
            Boss.BossPositioner.MovementEnabled = true;
        }
}

}