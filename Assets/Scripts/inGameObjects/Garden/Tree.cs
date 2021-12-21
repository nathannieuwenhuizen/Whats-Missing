using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : RoomObject
{
    [SerializeField]
    private MeshRenderer leaves;
    private Material leavesMaterial;


    private void Awake() {
        normalScale = transform.localScale.x;
        shrinkScale = normalScale * .2f;
        leavesMaterial = leaves.material;
    }
    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
    }


    public void UpdateTimeScale() {
        if (InSpace == false) return;
        leavesMaterial.SetFloat("_RoomTime", Room.TimeScale);
    }

    private void Reset() {
        Word = "tree";
        AlternativeWords = new string[] { "trees", "plant", "life"};
    }
}
