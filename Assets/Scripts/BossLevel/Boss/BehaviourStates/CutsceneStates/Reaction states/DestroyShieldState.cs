using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// At this state, the boss will perform a magic attack to destroy the shield
    ///</summary>
    public class DestroyShieldState : BossReactionState
    {
        public override void Start()
        {
            zoomValue = 35f;
            customMountainShape = true;
            stateName = "Destroy shield cutscene";
            base.Start();
        }

        public override Vector3 ReactionPosition() {
            return bossAI.ShieldDestroyPosition.position;
        }

        public override void DoReaction()
        {
            stateName = "Destroy shield cutscene";
            base.DoReaction();

            bossAI.StartCoroutine(Body.BossAnimator.DoMirrorAttack(() => {
                bossAI.Boss.BossChangesHandler.CreateChange("shield", ChangeType.missing);
            }, () => {
                OnStateSwitch?.Invoke(bossAI.Behaviours.crawlingChaseState);
            }));
        }
    }
}
