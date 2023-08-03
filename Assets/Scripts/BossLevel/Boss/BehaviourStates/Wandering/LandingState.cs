using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    public class LandingState : BossCutsceneState
    {

        public Transform landingPos;
        private Coroutine landingCoroutine;
        
        public IState nextState {
            get; set;
        }

        public override void Start()
        {
            base.Start();
            stateName = "Landing";
            // bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.toPath;
            // bossAI.Boss.BossPositioner.MovementEnabled = true;
            Boss.Head.LookAtPlayer = true;

            if (bossAI.Boss.BossPositioner.InAir) {
                Positioner.SetDestinationPath(bossAI.Boss.Player.transform, landingPos != null ? landingPos.position : bossAI.transform.position +  Vector3.up * (Boss.BOSS_GROUND_OFFSET));
                landingCoroutine = Positioner.StartCoroutine(Positioner.Landing( bossAI.CurrentWanderingPath.LandingPos.position,
                () => {
                    OnStateSwitch?.Invoke(nextState);
                }));
            }
            else {
                OnStateSwitch?.Invoke(nextState);
            }
        }
    }
}