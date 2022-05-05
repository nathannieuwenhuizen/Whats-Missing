using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

    public class TakeOffState : BossCutsceneState
    {
        private Coroutine takeOffCoroutine;
        
        public IState nextState {
            get; set;
        }

        public override void Start()
        {
            base.Start();
            stateName = "Take off";

            bossAI.Boss.BossPositioner.BodyOrientation = BodyOrientation.toShape;
            bossAI.Boss.BossPositioner.MovementEnabled = true;
            bossAI.Boss.BossPositioner.BodyMovementType = BodyMovementType.airSteering;

            if (bossAI.Boss.BossPositioner.InAir == false) {
                BossPositioner.SetDestinationPath(bossAI.transform.position, bossAI.transform.position);
                takeOffCoroutine = BossPositioner.StartCoroutine(BossPositioner.TakeOff(() => {
                    OnStateSwitch?.Invoke(nextState);
                }));
            } else {
                OnStateSwitch?.Invoke(nextState);
            }
            
        }
    }
}