using System.Collections;
using System.Collections.Generic;
using Febucci.UI;

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

    [SerializeField]
    private BossRoom bossRoom;

    [SerializeField]
    private TextAnimatorPlayer textAnimatorPlayer;
    public TextAnimatorPlayer TextAnimatorPlayer {
        get { return textAnimatorPlayer;}
    }
    [SerializeField]
    private BossChangesHandler bossChangeHandler;
    public BossChangesHandler BossChangesHandler {
        get { return bossChangeHandler;}
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


    private BossVoice bossVoice;
    public BossVoice BossVoice {
        get { return bossVoice;}
    }

    protected override void Awake() {
        bossVoice = new BossVoice(transform);
        bossChangeHandler = new BossChangesHandler(textAnimatorPlayer, bossRoom, this);

        ai = GetComponent<BossAI>();
        ai.Setup(this);
    }
    private void Update() {
        AI.UpdateAI();
        bossVoice.Update();
        if (Input.GetKeyDown(KeyCode.L)) {
            bossChangeHandler.CreateChange("gravity" ,ChangeType.missing);
        }
    }

    private void OnDrawGizmos() {
        Eye?.OnDrawGizmos();
    }

    private void Reset() {
        Word = "spirit";
        AlternativeWords = new string[] { "spirit", "spirits", "boss" };
    }
}
