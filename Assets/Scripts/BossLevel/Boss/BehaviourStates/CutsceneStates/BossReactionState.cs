using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// Bases class for a boss reaction state. The boss goes first in the boss reaction position and then does its reaction to the player.
    ///</summary>
    public abstract class BossReactionState : BossCutsceneState
    {
        private float oldMaxForce;
        public override void Start()
        {
            stateName = "Boss Reaction cutscene";
            base.Start();
            bossAI.StartCoroutine(GoToReactionPose());
            oldMaxForce = bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce;
            bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce *= 4f;
        }


        public IEnumerator GoToReactionPose() {
            //go to air position of landing pos
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            Positioner.RotationEnabled = true;
            Positioner.MovementEnabled = true;
            Positioner.SetDestinationPath(bossAI.ReactionPosition.position, bossAI.transform.position, true, 10f);
            Positioner.SpeedScale = 2f;
            Positioner.SteeringBehaviour.MaxForce *= 5f;

            while(!Positioner.AtPosition(3f)) {
                yield return new WaitForFixedUpdate();
            }
            Positioner.SteeringBehaviour.MaxForce /= 5f;

            //now land on the reaction pose
            yield return bossAI.StartCoroutine(Positioner.Landing(bossAI.ReactionPosition));
            Positioner.SpeedScale = 1f;

            DoReaction();
        }
        public virtual void DoReaction() {
        }

        public override void Exit()
        {
            bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce = oldMaxForce;
            base.Exit();
        }
    }
}