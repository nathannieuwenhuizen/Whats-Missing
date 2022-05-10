using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    public class TransformationState : BossReactionState
    {
        public override void Start()
        {
            base.Start();
        }

        public override void DoReaction()
        {
            stateName = "Transformation cutscene";
            base.DoReaction();
            bossAI.StartCoroutine(Transforming());
        }
        ///<summary>
        /// Boss does the transformation, applying the fire change and transforming their body to get extra tentacles.
        ///</summary>
        private IEnumerator Transforming() {
            Body.Metamorphose();
            yield return new WaitForSeconds(3f);
            bossAI.Boss.BossChangesHandler.CreateChange("fire", ChangeType.tooBig);
            yield return new WaitForSeconds(3f);
            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}