using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;


namespace Boss {

public class CrawlingChaseState : BaseChaseState
{
    private BossPositioner positioner;
    private Coroutine landingCoroutine;
    private Vector3 startChasePos;

    public override void DrawDebug()
    {
        base.DrawDebug();
    }

    public override void Start()
    {
        base.Start();
        positioner = bossAI.Boss.BossPositioner;
        positioner.BodyOrientation = BodyOrientation.toPath;

        UpdateBossChasePath(true);

        positioner.BodyMovementType = BodyMovementType.navMesh;
        positioner.MovementEnabled = false;
        Debug.Log("start crawling");
        landingCoroutine = positioner.StartCoroutine(positioner.Landing(positioner.GetClosestMountainCoordOfBoss(), () => {
            positioner.MovementEnabled = true;
        }));

    }

    ///<summary>
    /// Calculates for the boss, if the reset bool is true, the begin pos will be again set to the boss current position.
    ///</summary>
    private void UpdateBossChasePath(bool _resetBeginPos = false) {
        if (_resetBeginPos) startChasePos = bossAI.transform.position +  Vector3.up * (Boss.BOSS_GROUND_OFFSET);

        positioner.SetDestinationPath(bossAI.Boss.Player.transform.position + Vector3.up * (Boss.BOSS_GROUND_OFFSET), startChasePos);
    }

    public override void Run()
    {
        base.Run();
        if (positioner.MovementEnabled) {
            UpdateBossChasePath(false);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // positioner.BodyMovementType = BodyMovementType.none;
        positioner.BodyOrientation = BodyOrientation.toShape;

    }
}
}