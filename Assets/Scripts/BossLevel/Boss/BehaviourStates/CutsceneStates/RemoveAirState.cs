using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss{
    public class RemoveAirState : BossCutsceneState
    {
        public override void Start()
        {
            stateName = "Remove air cutscene";
            base.Start();
            bossAI.StartCoroutine(RemovingAir());
        }
        ///<summary>
        /// Boss does the transformation, applying the fire change and transforming their body to get extra tentacles.
        ///</summary>
        private IEnumerator RemovingAir() {
            Body.Metamorphose();
            yield return new WaitForSeconds(3f);
            bossAI.Boss.BossChangesHandler.CreateChange("air", ChangeType.missing);
            yield return new WaitForSeconds(3f);
            OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
        }
    }
}
