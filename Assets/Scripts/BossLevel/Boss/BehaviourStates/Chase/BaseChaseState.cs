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
        bossAI.BossHead.SteeringEnabled = false;
        Boss.Head.LookAtPlayer = true;
        oldViewAngle = bossAI.BossEye.ViewAngle;
        oldViewAlpha = bossAI.BossEye.ViewAlpha;

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

    protected void StopChase() {
        Debug.Log("stop chase");
        bossAI.Behaviours.takeoffState.withCutscene = false;
        bossAI.Behaviours.takeoffState.nextState = bossAI.Behaviours.wanderState;
        OnStateSwitch?.Invoke(bossAI.Behaviours.takeoffState);
    }

    public override void Exit()
    {
        bossAI.BossHead.SteeringEnabled = true;

        bossAI.BossEye.AnimateViewAngle(oldViewAngle);
        bossAI.BossEye.noticingValue = 0;
    }
        private bool isAttacking = false;
        protected void Attack() {
            if (isAttacking) return;
            bossAI.StartCoroutine(Attacking());
        }

        protected virtual IEnumerator Attacking() {
            isAttacking = true;
            Boss.BossPositioner.MovementEnabled = false;
            // bossAI.Boss.Body.ToggleDeathColliders(true);
            yield return bossAI.StartCoroutine(Boss.Body.BossAnimator.DoAttackAnimation());
            // bossAI.Boss.Body.ToggleDeathColliders(false);
            isAttacking = false;
            Boss.BossPositioner.MovementEnabled = true;
        }
    }

}