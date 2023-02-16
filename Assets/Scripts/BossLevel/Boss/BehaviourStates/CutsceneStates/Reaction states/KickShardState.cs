using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// At this state, the boss will perform a magic attack to destroy the shield
    ///</summary>
    public class KickShardState : BossReactionState
    {
        public override void Start()
        {
            customMountainShape = true;
            base.Start();
            zoomValue = 25f;
            stateName = "Kick shard cutscene";
            Boss.Body.Arm.Toggle(true);
        }

        public override Vector3 ReactionPosition() {
            Debug.Log("reaction pos");
            return bossAI.ReactionPosition.position;
        }

        public override void DoReaction()
        {
            stateName = "Kick shard cutscene";
            base.DoReaction();

            bossAI.StartCoroutine(ShardUpdate());
            bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_KICK_SHARD, false, 9.5f, () => {
                OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
            }));
        }
        public IEnumerator ShardUpdate() {
            yield return new WaitForSeconds(1.5f);
            OnBossCutsceneTargetUpdate?.Invoke(Boss.Body.KickedShardRenderer.transform, new Vector2(-2, 5f), 2f);
            DialoguePlayer.Instance.PlayLine(BossLines.GetShardKickLine());
            AttachMirrorShardToBoss();
            yield return new WaitForSeconds(4f);
            OnBossCutsceneTargetUpdate?.Invoke(Boss.Eye.transform, new Vector2(0, 4f), 1f);
            yield return new WaitForSeconds(.5f);
            DetachMirrorShard(); 
            yield return new WaitForSeconds(1f);
            DialoguePlayer.Instance.PlayLine(BossLines.Laugh);
        }


        
        private void AttachMirrorShardToBoss() {
            Boss.BossMirror.Shards[3].transform.SetParent(Boss.Body.KickedShardRenderer.transform);
            Boss.BossMirror.Shards[3].transform.localPosition = Vector3.zero;
            Boss.BossMirror.Shards[3].OutlineEnabled = false;
            Boss.BossMirror.Shards[3].transform.localRotation = Quaternion.Euler(Vector3.zero);
            Boss.BossMirror.Shards[3].transform.localScale = Vector3.one * 0.005f;
        }
        private void DetachMirrorShard() {
            Boss.BossMirror.Shards[3].OutlineEnabled = true;
            Boss.BossMirror.Shards[3].transform.SetParent(null);
            Boss.BossMirror.Shards[3].SetToFallPosition();

        }
    }
}
