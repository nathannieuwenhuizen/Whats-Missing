using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

public class WanderState : LookingState, IState
{
    private int currentPoseIndex = 0;
    private int CurrentPoseIndex {
        get => currentPoseIndex;
        set {
            currentPoseIndex = value;
            UpdateDestination();
        }
    }

    public Coroutine wanderingCoroutine;
    public Coroutine takeOffCoroutine;
    private float minLookDuration = 1f;

    private BossHead bossHead;
    private BossPositioner positioner;


    public override void DrawDebug()
    {
        base.DrawDebug();
    }

        protected override void BeginChaseOnGround()
        {
            bossAI.Behaviours.landingState.landingPos = bossAI.CurrentWanderingPath.LandingPos;
            base.BeginChaseOnGround();
        }

        public override void Start()
    {
        base.Start();
        
        stateName = "Wandering: " + bossAI.CurrentWanderingPath.name;
        bossAI.CurrentWanderingPath.showGizmo = true;
        positioner = bossAI.Boss.BossPositioner;
        bossHead = bossAI.Boss.Head;
        MirrorShard.OnPickedUp += ShardHasBeenPickedUp;

        wanderingCoroutine = bossAI.StartCoroutine(Wandering());
        CurrentPoseIndex = 0;
        Boss.Body.IKPass.SetLimbsActive(false);
        Boss.Head.SteeringEnabled = true;
        Boss.Head.LookAtPlayer = false;
        positioner.CurrentMovementBehaviour.bodyOrientation = BodyOrientation.toShape;
        positioner.BodyMovementType = BodyMovementType.airSteering;
        positioner.InAir = true;

    }
    
    public void ShardHasBeenPickedUp(MirrorShard _shard) {
        BeginChaseOnGround();
    }



    public override void Exit()
    {
        base.Exit();
        bossAI.CurrentWanderingPath.showGizmo = false;
        bossAI.StopCoroutine(wanderingCoroutine);
        MirrorShard.OnPickedUp -= ShardHasBeenPickedUp;

    }

    public override void Run()
    {
        base.Run();
    }

    public IEnumerator Wandering() {
        while (true) {
            while (IsAtPose(bossAI.CurrentWanderingPath.poses[currentPoseIndex]) == false) { 
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(minLookDuration);
            if (IsAtPose(bossAI.CurrentWanderingPath.poses[currentPoseIndex]))
                GoToNextPose();
        }
    }

    private void GoToNextPose() {
        CurrentPoseIndex = (CurrentPoseIndex + 1) % bossAI.CurrentWanderingPath.poses.Length;
    }

    private void UpdateDestination() {
        positioner.SetDestinationPath(bossAI.CurrentWanderingPath.poses[currentPoseIndex].position);
        if (bossAI.CurrentWanderingPath.poses[currentPoseIndex].aimPosition != null) bossHead.SetAim(bossAI.CurrentWanderingPath.poses[currentPoseIndex].aimPosition.position, Vector2.zero);
        //if no aim psoition is set, then let the boss look forward
        else bossHead.SetAim(bossHead.transform.position + Boss.transform.forward * 10f, Vector2.zero, true);
    }

    ///<summary>
    /// Returns true if the boss body is at the desired pose
    ///</summary>
    private bool IsAtPose(WanderPose pose) {
        if (positioner.AtPosition(.1f) == false) return false;
        if (bossHead.IsAtPosition(.1f, .5f) == false) return false;
        return true;
    }
}
}