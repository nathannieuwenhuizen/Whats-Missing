using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss{

    public class RemoveFloorState : BossReactionState
    {
        public override void DoReaction()
        {
            stateName = "Transformation cutscene";
            stateName = "Remove floor cutscene";
            base.DoReaction();
            bossAI.StartCoroutine(RemoveFloor());
        }

        ///<summary>
        /// Boss does the transformation, applying the fire change and transforming their body to get extra tentacles.
        ///</summary>
        private IEnumerator RemoveFloor() {
            Body.Metamorphose();
            yield return new WaitForSeconds(3f);
            bossAI.Boss.BossChangesHandler.CreateChange("floor", ChangeType.missing);
            yield return new WaitForSeconds(3f);
            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}