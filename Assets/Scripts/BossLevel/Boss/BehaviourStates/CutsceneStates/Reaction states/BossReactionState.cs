using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// Base class for a boss reaction state. The boss goes first in the boss reaction position and then does its reaction to the player.
    ///</summary>
    public abstract class BossReactionState : BossCutsceneState
    {
        //the old maxforce of the steringbehaviour. Needed to restore the movement steering force.
        private float oldMaxForce;

        ///<summary>
        /// If set to true, the boss mountain will recalculated ot get out of the bounds of the mountain shape.
        ///</summary>
        protected bool customMountainShape = false; 

        public override void Start()
        {
            stateName = "Boss Reaction cutscene";
            base.Start();
            bossAI.StartCoroutine(GoToReactionPose());
            oldMaxForce = bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce;
            bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce *= 4f;
        }

        ///<summary>
        /// Returns the reaction pose the boss would go to.
        ///</summary>
        public virtual Vector3 ReactionPosition() {
            return bossAI.ReactionPosition.position;
        }

        ///<summary>
        /// Boss goes to the reaction pose in the air
        ///</summary>
        public virtual IEnumerator GoToReactionPose() {
            //reshape the boss mountain to make the arc more fitting
            if (customMountainShape) Positioner.BossMountain.MakeMountainFit(bossAI.transform.position, ReactionPosition() + Vector3.up * 3f);
            
            //go to air position of landing pos
            Positioner.BodyOrientation = BodyOrientation.toPlayer;
            Positioner.BodyMovementType = BodyMovementType.airSteeringAtMountain;
            Positioner.RotationEnabled = true;
            Positioner.MovementEnabled = true;
            Positioner.SetDestinationPath(ReactionPosition() + Vector3.up * 3f, bossAI.transform.position, true, 5f);
            Positioner.SpeedScale = 2f;
            Positioner.SteeringBehaviour.MaxForce *= 5f;

            while(!Positioner.AtPosition(2f)) {
                yield return new WaitForFixedUpdate();
            }
            Positioner.SteeringBehaviour.MaxForce /= 5f;


            yield return bossAI.StartCoroutine(LandOnReactionPosition());

            //resotre the boss mountain
            if (customMountainShape) Positioner.BossMountain.RestoreShape();
            
            DoReaction();
        }

        ///<summary>
        /// The boss now lands on the reaction position
        ///</summary>
        public virtual IEnumerator LandOnReactionPosition() {
            //now land on the reaction pose
            yield return bossAI.StartCoroutine(Positioner.Landing(ReactionPosition()));
            Positioner.SpeedScale = 1f;

        }

        ///<summary>
        /// The boss does the reaction
        ///</summary>
        public virtual void DoReaction() {
            // Debug.Log("do reaction");
        }

        public override void Exit()
        {
            bossAI.Boss.BossPositioner.SteeringBehaviour.MaxForce = oldMaxForce;
            base.Exit();
        }
    }
}