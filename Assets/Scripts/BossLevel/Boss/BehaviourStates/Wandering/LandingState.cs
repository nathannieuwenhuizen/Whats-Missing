using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    public class LandingState : BossCutsceneState
    {
        private Coroutine landingCoroutine;
        
        public IState nextState {
            get; set;
        }

        public override void Start()
        {
            base.Start();
            stateName = "Landing";
            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.toPath;
            bossAI.Boss.BossPositioner.MovementEnabled = true;

            if (bossAI.Boss.BossPositioner.InAir) {
                BossPositioner.SetDestinationPath(bossAI.Boss.Player.transform, bossAI.transform.position +  Vector3.up * (Boss.BOSS_GROUND_OFFSET));
                Debug.Log("landing on a position");
                landingCoroutine = BossPositioner.StartCoroutine(BossPositioner.Landing(() => {
                    OnStateSwitch?.Invoke(nextState);
                }));
            }
            else {
                OnStateSwitch?.Invoke(nextState);
            }
        }
    }
}