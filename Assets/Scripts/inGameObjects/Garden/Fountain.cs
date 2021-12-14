using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : RoomObject
{

    [SerializeField]
    private Animator animator;

    void Start()
    {
        UpdateAnimatorTimeScale();
    }
    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateAnimatorTimeScale;
        TimeProperty.onTimeAppearing += UpdateAnimatorTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateAnimatorTimeScale;
        TimeProperty.onTimeAppearing -= UpdateAnimatorTimeScale;
    }

    private void UpdateAnimatorTimeScale() {
        // animator.speed = Room.TimeScale;
    }

    private void Reset() {
        Word = "fountain";
        AlternativeWords = new string[] { "fountains" };
    }

}
