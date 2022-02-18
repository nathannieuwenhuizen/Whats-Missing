using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIKLimb
{
    public bool HasContact { get; set;}
    public bool IsActive { get; set;}
    public float Weight {get; set; }

    public Vector3 GetRayCastPosition();
    public void UpdateIK();
    public IEnumerator AnimateWeight(float end);
    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool rightHand = true);
}
