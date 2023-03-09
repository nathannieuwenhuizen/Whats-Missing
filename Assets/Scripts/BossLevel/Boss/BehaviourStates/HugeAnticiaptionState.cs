using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

    public class HugeAnticipationState : BaseBossState, IState
    {
        public override void DrawDebug()
        {
            base.DrawDebug();
        }
        public override void Start()
        {
            stateName = "Huge Anticipation";

        }

        public override void Run()
        {
        }

        public override void Exit()
        {
        }
    }

}