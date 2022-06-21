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
    [SerializeField]
    private float minLookDuration = 1f;



    public override void DrawDebug()
    {
        base.DrawDebug();
        if (bossAI.CurrentWanderingPath != null) {
        }
    }

    protected override void BeginChaseOnGround()
    {
        bossAI.Behaviours.landingState.landingPos = bossAI.CurrentWanderingPath.LandingPos;
        base.BeginChaseOnGround();
    }

    public override void Start()
    {
        base.Start();
        
        bossAI.CurrentWanderingPath.showGizmo = true;
        Boss.Head.PlayBossVoice();
        MirrorShard.OnPickedUp += ShardHasBeenPickedUp;


        Boss.Head.LookAtPlayer = false;

        //begin with air wandering
        if (Positioner.InAir) {
            BeginAirWandering();
        } else {
            //first take off from the ground before air wandering.
            bossAI.StartCoroutine(Positioner.TakingOff(() => {
                BeginAirWandering();
            }));
        }
    }



    private void BeginAirWandering() {
        CurrentPoseIndex = 0;
        wanderingCoroutine = bossAI.StartCoroutine(Wandering());
        Boss.Body.IKPass.SetLimbsActive(false);
        Boss.Head.SteeringEnabled = true;
        Positioner.BodyOrientation = BodyOrientation.toShape;
        Positioner.BodyMovementType = bossAI.CurrentWanderingPath.BossMovementType;
        bossAI.BossEye.AnimateViewAlpha(1f);

    }
    
    public void ShardHasBeenPickedUp(MirrorShard _shard) {
        BeginChase();
    }



    public override void Exit()
    {
        base.Exit();
        bossAI.CurrentWanderingPath.showGizmo = false;

        Boss.Head.StopBossVoice();

        if (wanderingCoroutine != null) bossAI.StopCoroutine(wanderingCoroutine);
        MirrorShard.OnPickedUp -= ShardHasBeenPickedUp;
        bossAI.BossEye.AnimateViewAlpha(0f);

    }

    public override void Run()
    {
        base.Run();
        stateName = "Wandering: " + bossAI.CurrentWanderingPath.name;
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
        //if it is from in air to navmesh
        Debug.Log("next pose");
        if ( Positioner.BodyMovementType != bossAI.CurrentWanderingPath.BossMovementType && 
            bossAI.CurrentWanderingPath.BossMovementType == BodyMovementType.navMesh) {

            CurrentPoseIndex = (CurrentPoseIndex + 1) % bossAI.CurrentWanderingPath.poses.Length;
            
            Debug.Log("next pose for real");
            Positioner.StartCoroutine(
                Positioner.Landing(bossAI.CurrentWanderingPath.poses[currentPoseIndex].position.position, () => {
                    SetNextPos();
                })
            );
        } else {
            SetNextPos();
        }
    }
    private void SetNextPos () {
        //just go to next position
        Positioner.BodyMovementType = bossAI.CurrentWanderingPath.BossMovementType;
        Positioner.BodyOrientation = bossAI.CurrentWanderingPath.BossOrientationType;
        CurrentPoseIndex = (CurrentPoseIndex + 1) % bossAI.CurrentWanderingPath.poses.Length;
    }

    private void UpdateDestination() {
        Positioner.SetDestinationPath(bossAI.CurrentWanderingPath.poses[currentPoseIndex].position);
        if (bossAI.CurrentWanderingPath.poses[currentPoseIndex].aimPosition != null) Boss.Head.SetAim(bossAI.CurrentWanderingPath.poses[currentPoseIndex].aimPosition.position, Vector2.zero);
        //if no aim psoition is set, then let the boss look forward
        else Boss.Head.SetAim(Boss.Head.transform.position + Boss.transform.forward * 10f, Vector2.zero, true);
    }

    ///<summary>
    /// Returns true if the boss body is at the desired pose
    ///</summary>
    private bool IsAtPose(WanderPose pose) {
        if (Positioner.AtPosition(.1f) == false) return false;
        if (Boss.Head.IsAtPosition(.1f, .5f) == false) return false;
        return true;
    }
}
}