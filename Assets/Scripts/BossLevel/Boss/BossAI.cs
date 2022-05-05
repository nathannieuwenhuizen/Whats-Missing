using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Boss {

public abstract class BaseBossState : IState{
    public BossAI bossAI {get; set;}
    protected Boss Boss { get{ return bossAI.Boss; }}
    protected BossPositioner BossPositioner { get{ return bossAI.Boss.BossPositioner; }}

    public ILiveStateDelegate OnStateSwitch { get; set; }
    private GUIStyle debugStyle;
    public string stateName = "[no state name specified]";
        public virtual void DrawDebug()
        {
#if UNITY_EDITOR
            debugStyle = new GUIStyle();
            debugStyle.normal.textColor = Color.white;
            debugStyle.fontSize = 20;
            debugStyle.border = new RectOffset(5,5,5,5);
            GUI.backgroundColor = Color.black;
            Handles.Label(bossAI.transform.position, stateName, debugStyle);
#endif        
        }

        public virtual void Exit()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void Start()
        {

        }
    }

public class BossBehaviours {
    public BossBehaviours(BossAI _ai) {
        idleState = new IdleState() {bossAI = _ai };
        bossIntro = new BossIntroState() {bossAI = _ai };

        wanderState = new WanderState() {bossAI = _ai, wanderingPath = _ai.testPath };

        crawlingChaseState = new CrawlingChaseState() {bossAI = _ai };
        chagerAtShieldState = new ChargeAtShieldState() {bossAI = _ai };
        landingState = new LandingState() {bossAI = _ai };
        takeoffState = new TakeOffState() {bossAI = _ai };
    }

    //intro
    public IdleState idleState; 
    public BossIntroState bossIntro; 

    //stealth
    public WanderState wanderState; 

    //movement
    public LandingState landingState;
    public TakeOffState takeoffState;

    //chase
    public CrawlingChaseState crawlingChaseState; 
    public ChargeAtShieldState chagerAtShieldState; 
}
///<summary>
/// Main Ai for the boss holding all the statesand behaviour trees
///</summary>
public class BossAI : MonoBehaviour {

    public static bool PlayerIsInForceField = false;

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
        stateMachine?.Update();
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
        Debug.Log("force field enter");
        PlayerIsInForceField = true;
    }
    public void PlayerIsOutsideForceField() {
        Debug.Log("force field exit");
        PlayerIsInForceField = false;
    }

    private void OnEnable() {
        BossMirror.OnMirrorExplode += DoIntro;
        ForcefieldDemo.Forcefield.OnForceFieldEnter += PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit += PlayerIsOutsideForceField;
    }

    private void OnDisable() {
        BossMirror.OnMirrorExplode += DoIntro;
        ForcefieldDemo.Forcefield.OnForceFieldEnter -= PlayerIsInsdeForceField;
        ForcefieldDemo.Forcefield.OnForceFieldExit -= PlayerIsOutsideForceField;
        
    }
}
}