using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// The main script of the boss. 
///</summary>
public class Boss : RoomObject
{   
    [SerializeField]
    private Player player;
    public Player Player {
        get { return player;}
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

    private void Reset() {
        Word = "spirit";
        AlternativeWords = new string[] { "spirit", "spirits", "boss" };
    }

}
