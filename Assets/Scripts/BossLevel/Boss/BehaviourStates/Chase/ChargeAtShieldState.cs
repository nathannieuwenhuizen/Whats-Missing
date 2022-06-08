using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {

public class ChargeAtShieldState : BaseChaseState
    {
        private BossPositioner positioner;
        private Vector3 chragePos;

        public override void DrawDebug()
        {
            base.DrawDebug();
        }

        public override void Start()
        {
            stateName = "Charge at shield";

            base.Start();
            positioner = bossAI.Boss.BossPositioner;
            positioner.BodyOrientation = BodyOrientation.toPath;

            chragePos = Boss.Forcefield.EdgePosition(Boss.Player.transform.position);
            positioner.SetDestinationPath(chragePos, Boss.transform.position);


            bossAI.StartCoroutine(Extensions.AnimateCallBack(1f, 3f, AnimationCurve.EaseInOut(0,0,1,1), (float _val) => {
                positioner.SpeedScale = _val;
            }, 2f));
        }

        public override void Run()
        {
            // base.Run();
            if (positioner.AtPosition(Boss.BOSS_ATTACK_SHIELD_RANGE)) MeleeAttack();

        }

        public override void Exit()
        {
            positioner.SpeedScale = 1f;
            base.Exit();
        }

        protected override IEnumerator Attacking()
        {
            yield return base.Attacking();
            StopChase();
        }
    }
}