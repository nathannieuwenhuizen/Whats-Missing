using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// Here are all boss behaviours initialized and stored
    ///</summary>
    public class BossBehaviours {
        public BossBehaviours(BossAI _ai) {
            idleState = new IdleState() {bossAI = _ai };
            bossIntro = new BossIntroState() {bossAI = _ai };

            transformationState = new TransformationState() { bossAI = _ai};
            dieState = new DieState() { bossAI = _ai};
            removeAirState = new RemoveAirState() { bossAI = _ai};
            removeLightState = new RemoveLightState() { bossAI = _ai};

            wanderState = new WanderState() {bossAI = _ai };

            crawlingChaseState = new CrawlingChaseState() {bossAI = _ai };
            chargeAtShieldState = new ChargeAtShieldState() {bossAI = _ai };
            landingState = new LandingState() {bossAI = _ai };
            takeoffState = new TakeOffState() {bossAI = _ai };
        }

        //intro
        public IdleState idleState; 
        public BossIntroState bossIntro; 

        //cutscenes
        public TransformationState transformationState;
        public RemoveLightState removeLightState;
        public RemoveAirState removeAirState;
        public DieState dieState;

        //stealth
        public WanderState wanderState; 

        //movement
        public LandingState landingState;
        public TakeOffState takeoffState;

        //chase
        public CrawlingChaseState crawlingChaseState; 
        public ChargeAtShieldState chargeAtShieldState; 
    }
}
