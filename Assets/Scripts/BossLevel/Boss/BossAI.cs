using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Boss {
///<summary>
/// Main Ai for the boss holding all the statesand behaviour trees
///</summary>
public class BossAI : MonoBehaviour {

    public static bool PlayerIsInForceField = false;

    [SerializeField]
    private WanderingPaths paths;

    [HideInInspector]
    public WanderingPath CurrentWanderingPath;

    [SerializeField]
    private Transform reactionPosition;
    public Transform ReactionPosition {
        get { return reactionPosition;}
    }


    //states
    private FSM stateMachine;

    public FSM StateMachine {
        get { return stateMachine;}
    }

    private BossBehaviours behaviours;
    public BossBehaviours Behaviours {
        get { return behaviours;}
    }
    public BossEye BossEye {
        get { return boss.Eye;}
    }
    public BossHead BossHead {
        get { return boss.Head;}
    }

    private Boss boss;
    public Boss Boss {
        get { return boss;}
    }
    public void Setup(Boss _boss) {
        boss = _boss;
        behaviours = new BossBehaviours(this);
        CurrentWanderingPath = paths.firstShardPath;
    }
    public void InitializeStateMachine() {
        stateMachine = new FSM(behaviours.wanderState);
    }
    public void UpdateAI() {
        stateMachine?.Update();
        if (Input.GetKeyDown(KeyCode.T)) {
            stateMachine.SwitchState(behaviours.transformationState);
        }
    }

    private void OnDrawGizmos() {
        stateMachine?.Debug();
    }

    private void DoIntro(BossMirror mirror) {
        boss.transform.position = mirror.transform.position + new Vector3(0,-15,0);
        boss.transform.LookAt(boss.Player.transform.position, Vector3.up);
        boss.transform.rotation = Quaternion.Euler(0, boss.transform.rotation.eulerAngles.y,0);
        
        stateMachine.SwitchState(behaviours.bossIntro);
    }


    public void PlayerIsInsdeForceField() {
        PlayerIsInForceField = true;
    }

    public void PlayerIsOutsideForceField() {
        PlayerIsInForceField = false;
    }

    ///<summary>
    /// Boss reacts when the shards is getting collected.
    ///</summary>
    public void MirrorShardRecolectReaction(BossMirror _bossMirror) {
        int shardNumber = _bossMirror.AmmountOfShardsAttached();
        Debug.Log("shard recollect: " + shardNumber);
        switch(shardNumber) {
            case 0:
            CurrentWanderingPath = paths.firstShardPath;           
            break;
            case 1:
            CurrentWanderingPath = paths.secondShardPath;           
            break;
            case 2:
            CurrentWanderingPath = paths.thirdShardPath;           
            stateMachine.SwitchState(behaviours.transformationState);
            break;
            case 3:
            CurrentWanderingPath = paths.fourthShardPath;  
            stateMachine.SwitchState(behaviours.removeLightState);
            break;
            case 4:    
            CurrentWanderingPath = paths.fifthShardPath;   
            stateMachine.SwitchState(behaviours.removeLightState);
            break;        
            case 5:    
            //go instant into end chase mdoe
            break;
        }
    }
    public void MirrShardPickupEvent(MirrorShard _shard) {
        switch (_shard.ShardIndex) {
            case 4:
            stateMachine.SwitchState(behaviours.removeAirState);
            break;
        }
    }

    private void OnEnable() {
        BossMirror.OnMirrorExplode += DoIntro;
        BossMirror.OnMirrorExplode += MirrorShardRecolectReaction;
        MirrorShard.OnPickedUp += MirrShardPickupEvent;

        BossMirror.OnBossMirrorShardAttached += MirrorShardRecolectReaction;
        ForcefieldDemo.Forcefield.OnForceFieldEnter += PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit += PlayerIsOutsideForceField;
    }

    private void OnDisable() {
        BossMirror.OnMirrorExplode += DoIntro;
        BossMirror.OnMirrorExplode -= MirrorShardRecolectReaction;
        MirrorShard.OnPickedUp -= MirrShardPickupEvent;

        ForcefieldDemo.Forcefield.OnForceFieldEnter -= PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit -= PlayerIsOutsideForceField;
        BossMirror.OnBossMirrorShardAttached -= MirrorShardRecolectReaction;

        
    }
}
}