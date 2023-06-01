using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    ///<summary>
    /// Chase state for the boss when it wants ot free float to the player
    ///</summary>
    public class AirChaseState : BaseChaseState
    {
        private Vector3 startChasePos;
        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        public override void Start()
        {
            base.Start();
            Positioner.BodyOrientation = BodyOrientation.toPath;
            Positioner.BodyMovementType = BodyMovementType.freeFloat;
            stateName = "Air Chase Chase";
            Positioner.MovementEnabled = true;
            Positioner.InAir = true;

            //make the boss a bit slower.
            Positioner.SpeedScale = .5f;
            
            UpdateBossChasePath(true);

        }

        ///<summary>
        /// Calculates for the boss, if the reset bool is true, the begin pos will be again set to the boss current position.
        ///</summary>
        private void UpdateBossChasePath(bool _resetBeginPos = false) {
            if (_resetBeginPos) startChasePos = bossAI.transform.position +  Vector3.up * (Boss.BOSS_GROUND_OFFSET);

            Positioner.SetDestinationPath(bossAI.Boss.Player.transform, startChasePos);
        }

        public override void Run()
        {
            base.Run();

            //dont move when the boss is too close to the player
            float distanceToPlayer = DistnaceToPlayer();
            Positioner.MovementEnabled = distanceToPlayer > Boss.BOSS_MIN_DISTANCE_TO_PLAYER;

            //update the boss destination path.
            UpdateBossChasePath(false);            
        }


        public override void Exit()
        {
            Positioner.SpeedScale = 1f;
            base.Exit();
        }
    }
}