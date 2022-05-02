using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// The main script of the boss. 
    ///</summary>
    [RequireComponent(typeof(BossAI))]
    [RequireComponent(typeof(BossPositioner))]
    public class Boss : RoomObject
    {       
        public const float BOSS_SIZE = 8f;
        public const float BOSS_GROUND_OFFSET = 20f;
        public const float BOSS_ATTACK_PLAYER_RANGE = 5f;
        public const float BOSS_ATTACK_SHIELD_RANGE = 5f;

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

        private BossPositioner bossPositioner;
        public BossPositioner BossPositioner {
            get { return bossPositioner;}
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
            bossPositioner = GetComponent<BossPositioner>();

            ai = GetComponent<BossAI>();
            ai.Setup(this);
        }
        private void Start() { 
            ai.InitializeStateMachine();
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
}