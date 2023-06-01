using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {

    public class CrawlingChaseState : BaseChaseState
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
            Positioner.BodyMovementType = BodyMovementType.navMesh;
            stateName = "Crawling Chase";
            Positioner.MovementEnabled = true;
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
            Positioner.MovementEnabled = distanceToPlayer > Boss.BOSS_MIN_CRAWLING_DISTANCE_TO_PLAYER;
            
            //update the boss destination path.
            UpdateBossChasePath(false);
        }
    }
}