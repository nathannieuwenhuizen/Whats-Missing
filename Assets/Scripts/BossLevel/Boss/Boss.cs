using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// The main script of the boss. 
///</summary>
[RequireComponent(typeof(BossAI))]
public class Boss : RoomObject
{   
    [Header("Boss info")]
    [Space]
    [SerializeField]
    private Player player;
    public Player Player {
        get { return player;}
    }
    [Header("Boss parts")]
    [SerializeField]
    private BossEye eye;
    public BossEye Eye {
        get { return eye;}
    }
    [SerializeField]
    private BossHead head;
    public BossHead Head {
        get { return head;}
    }
    [SerializeField]
    private BossBody body;
    public BossBody Body {
        get { return body;}
    }
    private BossAI ai;
    public BossAI AI {
        get { return ai;}
    }
    protected override void Awake() {
        ai = GetComponent<BossAI>();
        ai.Setup(this);
    }
    private void Update() {
        AI.UpdateAI();
    }

    private void OnDrawGizmos() {
        Eye?.OnDrawGizmos();
    }

    private void Reset() {
        Word = "spirit";
        AlternativeWords = new string[] { "spirit", "spirits", "boss" };
    }
}
