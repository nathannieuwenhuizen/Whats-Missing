using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using ForcefieldDemo;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// The main script of the boss. 
    ///</summary>
    [RequireComponent(typeof(BossAI))]
    [RequireComponent(typeof(BossPositioner))]
    // [RequireComponent(typeof(Rigidbody))]
    public class Boss : RoomObject
    {   
        
        public delegate void BossEvent();
        public static BossEvent OnBossIntroStart;

        
        //sizes
        public const float BOSS_SIZE = 8f;
        public const float BOSS_HEIGHT = 17f * .7f;

        //ground offset
        public const float BOSS_GROUND_OFFSET = 2f;

        //attack distances
        public const float BOSS_MELEE_ATTACK_RANGE = 14f;
        public const float BOSS_LASER_ATTACK_RANGE = 30f;
        public const float BOSS_ATTACK_SHIELD_RANGE = 8f;

        //min distances
        public const float BOSS_MIN_DISTANCE_TO_PLAYER = 4f;
        public const float BOSS_MIN_CRAWLING_DISTANCE_TO_PLAYER = 7.5f;

        ///<summary>
        /// what units does the palyer need to be close so that the boss 
        /// goes into the charge at shiled phase.
        ///</summary>
        public const float CHARGE_AT_SHIELD_THRESHHOLD = 30f;

        [Header("Boss info")]
        [Space]
        [SerializeField]
        private Player player;
        public Player Player {
            get { return player;}
        }

        [SerializeField]
        private Forcefield forceField;
        public Forcefield Forcefield {
            get { return forceField;}
        }

        [SerializeField]
        private BossRoom bossRoom;
        public BossMirror BossMirror {
            get { return bossRoom.BossMirror;}
        }

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
    	[SerializeField]
        private ParticleSystem magicBlast;


        private BossVoice bossVoice;
        public BossVoice BossVoice {
            get { return bossVoice;}
        }

        protected override void Awake() {
            largeScale = 5f * .7f;
            Animated = true;
            bossVoice = new BossVoice(transform);
            bossChangeHandler = new BossChangesHandler(textAnimatorPlayer, bossRoom, this, magicBlast);
            bossPositioner = GetComponent<BossPositioner>();
            Body.boss = this;

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
                bossChangeHandler.CreateChange("water" ,ChangeType.tooBig);
            }
        }
        private void OnEnable() {
            Water.OnWaterBigStart += RemoveMirrorAttack;
        }
        private void OnDisable() {
            Water.OnWaterBigStart -= RemoveMirrorAttack;
        }
        private void  RemoveMirrorAttack() {
            bossChangeHandler.RemoveText();
        }

        private void OnDrawGizmosSelected() {
            Eye?.OnDrawGizmosSelected();
        }

        private void Reset() {
            Word = "spirit";
            AlternativeWords = new string[] { "spirit", "spirits", "boss" };
        }

        public override void OnMissing()
        {
            ai.StateMachine.SwitchState(ai.Behaviours.dieState);
            bossChangeHandler.RemoveText();
            // base.OnMissing();
        }
    }
}