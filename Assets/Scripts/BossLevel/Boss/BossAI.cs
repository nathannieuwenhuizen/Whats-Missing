using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Main Ai for the boss holding all the statesand behaviour trees
///</summary>
public class BossAI : MonoBehaviour {

    //states
    public FSM bossBehavior;
    private LookingState lookingState;
    [SerializeField]
    private BossEye eye;
    public BossEye BossEye {
        get { return eye;}
    }
    public Light light;
    [SerializeField]
    private Boss boss;
    public Boss Boss {
        get { return boss;}
        set { boss = value; }
    }
    public void Setup(Boss _boss) {
        lookingState = new LookingState() {bossAI = this };
        bossBehavior = new FSM(lookingState);
        boss = _boss;
    }
    public void UpdateAI() {
        bossBehavior.Update();
    }

    private void OnDrawGizmos() {
        eye.OnDrawGizmos(boss);
    }
}
