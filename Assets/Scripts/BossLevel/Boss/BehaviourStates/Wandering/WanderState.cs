using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

public class WanderState : LookingState, IState
{
    public WanderingPath wanderingPath;

    private int currentPoseIndex = 0;
    private int CurrentPoseIndex {
        get => currentPoseIndex;
        set {
            currentPoseIndex = value;
            UpdateDestination();
        }
    }

    public Coroutine wanderingCoroutine;
    private float minLookDuration = 1f;

    private BossHead bossHead;
    private BossPositioner positioner;


    public override void DrawDebug()
    {
        base.DrawDebug();
        
    }

    public override void Start()
    {
        base.Start();
        positioner = bossAI.Boss.BossPositioner;
        positioner.UseSteering = true;
        bossHead = bossAI.Boss.Head;

        CurrentPoseIndex = 0;
        wanderingCoroutine = bossAI.StartCoroutine(Wandering());

    }

    public override void Exit()
    {
        base.Exit();
        bossAI.StopCoroutine(wanderingCoroutine);
        positioner.UseSteering = false;

    }

    public override void Run()
    {
        base.Run();
    }
    public IEnumerator Wandering() {
        while (true) {
            while (IsAtPose(wanderingPath.poses[currentPoseIndex]) == false) { 
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(minLookDuration);
            GoToNextPose();
        }
    }

    private void GoToNextPose() {
        CurrentPoseIndex = (CurrentPoseIndex + 1) % wanderingPath.poses.Length;
    }

    private void UpdateDestination() {
        positioner.SetDestinationPath(wanderingPath.poses[currentPoseIndex].position);
        if (wanderingPath.poses[currentPoseIndex].aimPosition != null) bossHead.SetAim(wanderingPath.poses[currentPoseIndex].aimPosition.position, Vector2.zero);
        //if no aim psoition is set, then let the boss look forward
        else bossHead.SetAim(bossHead.transform.position + -bossHead.transform.forward * 5f, Vector2.zero, true);
    }

    ///<summary>
    /// Returns true if the boss body is at the desired pose
    ///</summary>
    private bool IsAtPose(WanderPose pose) {
        Debug.Log("positioner at pose " + positioner.isAtPosition(.1f));
        Debug.Log("head at pose " + bossHead.IsAtPosition(.1f));
        if (positioner.isAtPosition(.1f, 1f) == false) return false;
        if (bossHead.IsAtPosition(.1f, 1f) == false) return false;
        return true;
    }
}
}