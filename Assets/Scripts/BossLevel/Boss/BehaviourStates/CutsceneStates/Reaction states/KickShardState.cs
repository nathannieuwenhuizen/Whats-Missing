using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// At this state, the boss will perform a magic attack to destroy the shield
    ///</summary>
    public class KickShardState : BossReactionState
    {
        public Coroutine animationCoroutine;

        public override void Start()
        {
            customMountainShape = true;
            base.Start();
            zoomValue = 25f;
            stateName = "Kick shard cutscene";
            Boss.Body.Arm.Toggle(true); // purely for testing
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
            animationCoroutine = bossAI.StartCoroutine(Body.BossAnimator.DoTriggerAnimation(BossAnimatorParam.TRIGGER_KICK_SHARD, false, 9.5f, () => {
                OnStateSwitch?.Invoke(bossAI.Behaviours.wanderState);
            }));
        }
        public IEnumerator ShardUpdate() {
            yield return new WaitForSeconds(.5f);
            //AudioHandler.Instance?.Play3DSound(SFXFiles.boss_shard_kick_noise, Boss.transform);
            yield return new WaitForSeconds(1f);
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
            MirrorShard shard = Boss.BossMirror.Shards[3];
            shard.lettersVisible = false;
            shard.transform.SetParent(Boss.Body.KickedShardRenderer.transform);
            shard.transform.localPosition = Vector3.zero;
            shard.OutlineEnabled = false;
            shard.transform.localRotation = Quaternion.Euler(Vector3.zero);
            shard.transform.localScale = Vector3.one;// * 0.005f;
        }

        private void DetachMirrorShard() {
            MirrorShard shard = Boss.BossMirror.Shards[3];
            shard.lettersVisible = true;
            shard.OutlineEnabled = true;
            shard.transform.SetParent(null);
            shard.SetToFallPosition();
        }

        public override void Exit()
        {
            if(animationCoroutine != null) bossAI.StopCoroutine(animationCoroutine);
            base.Exit();
        }
    }
}
