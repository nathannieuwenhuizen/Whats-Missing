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
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            Positioner.SetDestinationPath(bossAI.ReactionPosition.position, bossAI.transform.position);
            bossAI.StartCoroutine(GoToReactionPose());
        }
        public IEnumerator GoToReactionPose() {
            Positioner.SpeedScale = 10f;
            while(Positioner.AtPosition(1f)) {
                yield return new WaitForFixedUpdate();
            }
            Positioner.SpeedScale = 1f;
            DoReaction();
        }
        public virtual void DoReaction() {
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}