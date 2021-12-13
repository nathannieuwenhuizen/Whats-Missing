using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// Baby duck that has a different behavior
///</summary>
public class BabyDuck : Duck
{

    private FollowState followState;
    [SerializeField]
    private Duck[] allDucks;
    protected override void SetUpBehaviour()
    {
        followState = new FollowState() {
            _duck = this,
            _otherDucks = DetectDucks()
            };
        duckBehaviour = new FSM(followState);
    }

    public BabyDuck() {
        largeScale = 2f;
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        followState.seperation = 4f;
        followState.swimSpeed = 1f;
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        followState.seperation = 2f;
        followState.swimSpeed = 2f;

    }
    private List<Duck> DetectDucks() {
        List<Duck> result = new List<Duck>(allDucks);
        result.Remove(this);
        return result;  
    }
    public override void Quack()
    {
        animator.SetTrigger("quack");
        AudioHandler.Instance.Player3DSound(SFXFiles.baby_duck, transform, .2f, IsEnlarged ? .5f : 1f, false, true, 30f);
    }
}
