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
            Boss.Head.LookAtPlayer = false;

            if (bossAI.Boss.BossPositioner.InAir == false) {
                Positioner.SetDestinationPath(bossAI.transform.position, bossAI.transform.position);
                takeOffCoroutine = Positioner.StartCoroutine(Positioner.TakeOff(() => {
                    OnStateSwitch?.Invoke(nextState);
                }));
            } else {
                OnStateSwitch?.Invoke(nextState);
            }
            
        }
    }
}