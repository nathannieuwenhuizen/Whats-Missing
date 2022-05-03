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
            stateName = "Take off";
            
            base.Start();
            if (bossAI.Boss.BossPositioner.InAir == false) {
                takeOffCoroutine = BossPositioner.StartCoroutine(BossPositioner.TakeOff(() => {
                    OnStateSwitch?.Invoke(nextState);
                }));
            } else {
                OnStateSwitch?.Invoke(nextState);
            }
            
        }
    }
}