using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {

public interface IIKBossBone {
    public Animator animator {get; set;}
    public void OnAnimatorIK();
    public bool IsActive { get; set;}
    public float Weight {get; set; }
    public IEnumerator AnimateWeight(float end);
    public float WeightAnimationDuration { get; set;}
    
    public Vector3 IKPos {get; set; }
}
public class IKBossBone : MonoBehaviour, IIKBossBone
{
    public Animator animator { get; set; }
    public bool IsActive { get; set; } = false;
    public float Weight { get; set; }
    public float WeightAnimationDuration { get; set; } = .5f;
    public Vector3 IKPos { get; set; }

    public IEnumerator AnimateWeight(float end)
    {
        yield return StartCoroutine(Extensions.AnimateCallBack(Weight, end, AnimationCurve.EaseInOut(0,0,1,1), (float _val) => Weight = _val, WeightAnimationDuration));
    }

    public virtual void OnAnimatorIK()
    {
        if (animator == null || IsActive == false) return;
    }

}

}