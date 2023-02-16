using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// The boss dies and does an die animation
///</summary>
namespace Boss {

    public class DieState : BossCutsceneState
    {
        public delegate void BossEvent();
        public static BossEvent OnBossDie;

        public override void Start()
            {
            base.Start();
            stateName = "Die cutscene";

            Positioner.MovementEnabled = false;
            Positioner.RotationEnabled = false;

            //transport the boss to the end position
            Vector3 aimDelta = (bossAI.ShieldDestroyPosition.position - bossAI.DiePosition.position).normalized;
            bossAI.transform.position = bossAI.DiePosition.position + aimDelta * 5f;
            Quaternion aim =  Quaternion.LookRotation(-aimDelta, Vector3.up);
            bossAI.transform.rotation = aim;

            // Positioner.BodyMovementType = BodyMovementType.navMesh;
            // Positioner.BodyOrientation = BodyOrientation.toPath;

            Positioner.SetDestinationPath(bossAI.DiePosition, bossAI.transform.position, false, 0);
            bossAI.StartCoroutine(bossAI.Boss.Body.UpdatingDisolve(10f));

            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_DEATH, true, 10f, () => {
                OnBossDie?.Invoke();
                OnStateSwitch.Invoke(bossAI.Behaviours.idleState);
            }));
        }
        
    }
}
