using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Boss {
///<summary>
/// Main Ai for the boss holding all the statesand behaviour trees
///</summary>
public class BossAI : MonoBehaviour {


    [SerializeField]
    private WanderingPaths paths;

    [HideInInspector]
    public WanderingPath CurrentWanderingPath;

    [Header("positions")]

    [SerializeField]
    private Transform reactionPosition;
    public Transform ReactionPosition {
        get { return reactionPosition;}
    }

    [SerializeField]
    private Transform shieldDestroyPosition;
    public Transform ShieldDestroyPosition {
        get { return shieldDestroyPosition;}
    }

    [SerializeField]
    private Transform chargePosition;
    public Transform ChargePosition {
        get { return chargePosition;}
    }

    [SerializeField]
    private Transform diePosition;
    public Transform DiePosition {
        get { return diePosition;}
    }


    //states
    private FSM stateMachine;

    public FSM StateMachine {
        get { return stateMachine;}
    }

    private MountainAttackPose mountainAttackPosition;
    public MountainAttackPose MountainAttackPosition {
        get { return mountainAttackPosition;}
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
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKeyDown(KeyCode.T)) {
                stateMachine.SwitchState(behaviours.transformationState);
            }
            if (Input.GetKeyDown(KeyCode.Y)) {
                stateMachine.SwitchState(behaviours.dieState);
            }
            // if (Input.GetKeyDown(KeyCode.T)) {
            //     stateMachine.SwitchState(behaviours.destroyShieldState);
            // }
            // if (Input.GetKeyDown(KeyCode.T)) {
            //     stateMachine.SwitchState(behaviours.chargeAtShieldState);
            // }
        }
#endif
    }

    private void OnDrawGizmos() {
        stateMachine?.Debug();
    }

    private void DoIntro(BossMirror mirror) {
        //delta from player to mirror
        Vector3 delta = boss.Player.transform.position - mirror.transform.position;
        delta.y = 0;
        delta = delta.normalized;

        boss.transform.position = mirror.transform.position + new Vector3(0,-15,0) + (delta * -5f);
        boss.transform.LookAt(boss.Player.transform.position, Vector3.up);
        boss.transform.rotation = Quaternion.Euler(0, boss.transform.rotation.eulerAngles.y,0);
        
        stateMachine.SwitchState(behaviours.bossIntro);
    }


    public void PlayerIsInsdeForceField() {
        Player.INVINCIBLE = true;
    }

    public void PlayerIsOutsideForceField() {
        Player.INVINCIBLE = false;
    }

    ///<summary>
    /// Boss reacts when the shards is getting collected.
    ///</summary>
    public void MirrorShardRecolectReaction(BossMirror _bossMirror) {
        int shardNumber = _bossMirror.AmmountOfShardsAttached();
        if (CurrentWanderingPath != null) CurrentWanderingPath.showGizmo = false;
        // Debug.Log("shard recollect: " + shardNumber);
        switch(shardNumber) {
            case 0:
            CurrentWanderingPath = paths.firstShardPath;           
            break;
            case 1:
            CurrentWanderingPath = paths.secondShardPath;           
            break;
            case 2:
            CurrentWanderingPath = paths.thirdShardPath;           
            // stateMachine.SwitchState(behaviours.transformationState);
            Debug.Log("begin transformation state");
            break;
            case 3:
            CurrentWanderingPath = paths.fourthShardPath;  
            stateMachine.SwitchState(behaviours.kickShardState);
            break;
            case 4:    
            CurrentWanderingPath = paths.fifthShardPath;   
            break;        
            case 5:    
            //go instant into end chase mdoe
            stateMachine.SwitchState(behaviours.destroyShieldState);
            break;
        }
        if (CurrentWanderingPath != null) CurrentWanderingPath.showGizmo = true;

    }
    public void MirrShardPickupEvent(MirrorShard _shard) {
        switch (_shard.ShardIndex) {
            case 4:
            // stateMachine.SwitchState(behaviours.removeAirState);
            break;
        }
    }

    private void UpdateMountainAttackPosition(MountainAttackPose _pos) {
        mountainAttackPosition = _pos;
    }

    private void OnEnable() {
        BossMirror.OnMirrorExplode += DoIntro;
        BossMirror.OnMirrorExplode += MirrorShardRecolectReaction;
        MirrorShard.OnPickedUp += MirrShardPickupEvent;
        MountainAttackPose.OnPlayerEnteringAttackArea += UpdateMountainAttackPosition;

        BossMirror.OnBossMirrorShardAttached += MirrorShardRecolectReaction;
        ForcefieldDemo.Forcefield.OnForceFieldEnter += PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit += PlayerIsOutsideForceField;
    }

    private void OnDisable() {
        BossMirror.OnMirrorExplode += DoIntro;
        BossMirror.OnMirrorExplode -= MirrorShardRecolectReaction;
        MirrorShard.OnPickedUp -= MirrShardPickupEvent;
        MountainAttackPose.OnPlayerEnteringAttackArea -= UpdateMountainAttackPosition;

        ForcefieldDemo.Forcefield.OnForceFieldEnter -= PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit -= PlayerIsOutsideForceField;
        BossMirror.OnBossMirrorShardAttached -= MirrorShardRecolectReaction;

        
    }
}
}