using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss{

    public class RemoveLightState : BossReactionState
    {
        public override void DoReaction()
        {
            stateName = "Remove light cutscene";
            base.DoReaction();
            bossAI.StartCoroutine(RemoveLight());
        }
        ///<summary>
        /// Boss does the transformation, applying the fire change and transforming their body to get extra tentacles.
        ///</summary>
        private IEnumerator RemoveLight() {
            Body.Metamorphose();
            yield return new WaitForSeconds(3f);
            bossAI.Boss.BossChangesHandler.CreateChange("light", ChangeType.missing);
            yield return new WaitForSeconds(3f);
            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}