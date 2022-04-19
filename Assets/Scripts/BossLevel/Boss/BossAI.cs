using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

public interface IBaseBossState{
    public BossAI bossAI {get; set;}
}

public class BossBehaviours {
    public BossBehaviours(BossAI _ai) {
        lookingState = new LookingState() {bossAI = _ai };
        chaseState = new ChaseState() {bossAI = _ai };
        idleState = new IdleState() {bossAI = _ai };
        bossIntro = new BossIntroState() {bossAI = _ai };
        wanderState = new WanderState() {bossAI = _ai, wanderingPath = _ai.testPath };

    }

    public LookingState lookingState; 
    public ChaseState chaseState; 
    public IdleState idleState; 
    public BossIntroState bossIntro; 
    public WanderState wanderState; 
}
///<summary>
/// Main Ai for the boss holding all the statesand behaviour trees
///</summary>
public class BossAI : MonoBehaviour {

    public WanderingPath testPath;

    //states
    private FSM stateMachine;

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
    }
    public void InitializeStateMachine() {
        stateMachine = new FSM(behaviours.wanderState);
    }
    public void UpdateAI() {
        stateMachine.Update();
    }

    private void OnDrawGizmos() {
        stateMachine?.Debug();
    }

    private void DoIntro(BossMirror mirror) {
        boss.transform.position = mirror.transform.position + new Vector3(0,-15,0);
        stateMachine.SwitchState(behaviours.bossIntro);
    }

    private void OnEnable() {
        BossMirror.OnMirrorExplode += DoIntro;
    }

    private void OnDisable() {
        BossMirror.OnMirrorExplode += DoIntro;
        
    }
}
}