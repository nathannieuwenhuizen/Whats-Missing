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
            Positioner.MovementEnabled = true;
            Positioner.SetDestinationPath(bossAI.ReactionPosition.position, bossAI.transform.position, true, 10f);
            Positioner.SpeedScale = 2f;

            while(!Positioner.AtPosition(3f)) {
                yield return new WaitForFixedUpdate();
            }

            //now land on the reaction pose
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.BodyMovementType = BodyMovementType.freeFloat;
            Positioner.MovementEnabled = true;
            Positioner.SetDestinationPath(bossAI.ReactionPosition.position, bossAI.transform.position);
            Positioner.SpeedScale = 1f;
            Boss.Body.BossAnimator.SetBool(BossAnimatorParam.BOOL_INAIR, false);

            while(!Positioner.AtPosition(1f)) {
                yield return new WaitForFixedUpdate();
            }

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