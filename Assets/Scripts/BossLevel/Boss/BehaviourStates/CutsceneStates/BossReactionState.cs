using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// Bases class for a boss reaction state. The boss goes first in the boss reaction position and then does its reaction to the player.
    ///</summary>
    public abstract class BossReactionState : BossCutsceneState
    {
        public override void Start()
        {
            stateName = "Boss Reaction cutscene";
            base.Start();
            Positioner.MovementEnabled = true;
            Positioner.BodyOrientation = BodyOrientation.toShape;
            Positioner.BodyMovementType = BodyMovementType.airSteering;
            Positioner.SetDestinationPath(bossAI.ReactionPosition.position, bossAI.transform.position);
            bossAI.StartCoroutine(GoToReactionPose());
            Positioner.SpeedScale = 5f;
        }
        public IEnumerator GoToReactionPose() {
            while(Positioner.AtPosition(5f)) {
                yield return new WaitForFixedUpdate();
            }
            DoReaction();
        }
        public virtual void DoReaction() {
            Positioner.SpeedScale = 1f;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}