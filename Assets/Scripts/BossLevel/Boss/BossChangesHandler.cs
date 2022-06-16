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

    public BossChangesHandler(TextAnimatorPlayer _textAnimator, BossRoom _bossRoom, Boss _boss) {
        textAnimator = _textAnimator;
        bossRoom = _bossRoom;
        boss = _boss;
        changeParticle = textAnimator.GetComponentInChildren<ParticleSystem>();
        textAnimator.onTextShowed.AddListener(ApplyChange);
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
        // textAnimator.ShowText(_change.Word + " is " + Change.GetChangeTypeText(_change.ChangeType));
        // yield return new WaitForSeconds(1f);
        ApplyChange();
    }
    private void ApplyChange() {
        bossRoom.ChangeHandler.AddBossChange(acitvatedChange);
        changeParticle.Emit(50);
        OnShockwave?.Invoke(boss.Head.transform);

    }

    public void RemoveChange() {
        // textAnimator.StartDisappearingText();
        bossRoom.ChangeHandler.RemoveBossChange(acitvatedChange);
        acitvatedChange = null;
    }
}
}