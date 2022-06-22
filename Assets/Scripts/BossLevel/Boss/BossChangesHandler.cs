using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
namespace Boss {

///<summary>
/// Handles the boss change attacks
///</summary>
public class BossChangesHandler
{
    public delegate void ShockWaveEvent(Transform _transform);
    public static ShockWaveEvent OnShockwave;

    private Coroutine creatingChangeCoroutine;
    private IChange acitvatedChange;

    private TextAnimatorPlayer textAnimator;
    private BossRoom bossRoom;
    private Boss boss;
    private ParticleSystem changeParticle;

    public ParentCoord canvasCoord;
    private Canvas changeCanvas;

    public BossChangesHandler(TextAnimatorPlayer _textAnimator, BossRoom _bossRoom, Boss _boss, ParticleSystem _particleSystem) {
        textAnimator = _textAnimator;
        bossRoom = _bossRoom;
        boss = _boss;
        changeParticle = _particleSystem;
        textAnimator.onTextShowed.AddListener(ApplyChange);
        changeCanvas = textAnimator.transform.parent.GetComponent<Canvas>();

        canvasCoord.startParent = changeCanvas.transform.parent;
        canvasCoord.startPos = changeCanvas.transform.localPosition;
        canvasCoord.startRot = changeCanvas.transform.localRotation;

    }

    ///<summary>
    /// Creates a mirror change depending on the type of attack
    ///</summary>
    public void CreateChange(string _word, ChangeType _changeType) {
        Change _change = new Change() {
            word = _word,
            changeType = _changeType
        };
        creatingChangeCoroutine = bossRoom.StartCoroutine(CreatingChange(_change));
    }

    ///<summary>
    /// Coroutine of creating the change
    ///</summary>
    private IEnumerator CreatingChange(IChange _change) {
        if (acitvatedChange != null) {
            RemoveChange();
            yield return new WaitForSeconds(1f);
        }
        acitvatedChange = _change;
        textAnimator.ShowText("<Wiggle>" + _change.Word + " is " + Change.GetChangeTypeText(_change.ChangeType));
        // yield return new WaitForSeconds(1f);
        ApplyChange();
    }

    ///<summary>
    /// Applies the change
    ///</summary>
    private void ApplyChange() {
        ReattachCanvas();
        DetachCanvas();

        bossRoom.ChangeHandler.AddBossChange(acitvatedChange);
        changeParticle.Play();
        OnShockwave?.Invoke(boss.Head.transform);

    }

    ///<summary>
    /// Removes the change
    ///</summary>
    public void RemoveChange() {
        textAnimator.StartDisappearingText();
        bossRoom.ChangeHandler.RemoveBossChange(acitvatedChange);
        acitvatedChange = null;
    }

    private void DetachCanvas() {
        changeCanvas.transform.parent = null;
        changeCanvas.transform.localPosition = canvasCoord.startParent.transform.position + canvasCoord.startPos;
        changeCanvas.transform.localRotation = canvasCoord.startParent.rotation * canvasCoord.startRot;

    }

    private void ReattachCanvas() {
        changeCanvas.transform.parent = canvasCoord.startParent;
        changeCanvas.transform.localPosition = canvasCoord.startPos;
        changeCanvas.transform.localRotation = canvasCoord.startRot;

    }
}
}