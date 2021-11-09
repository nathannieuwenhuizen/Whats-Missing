using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntity :  MonoBehaviour, IChangable, IRoomObject
{
    [SerializeField]
    protected string word;

    [SerializeField]
    protected string[] alternateWords;

    protected Vector3 currentScale;

    public string Word {
        get { return word;}
        set {word = value;}
    }

    public bool Animated { 
        get; set; 
    }

    protected bool inSpace = true;
    public bool InSpace { get => inSpace; }

    public Transform Transform => transform;

    public string[] AlternativeWords { get => alternateWords; set => alternateWords = value; }

    [SerializeField]
    private MissingChangeEffect missingChangeEffect = MissingChangeEffect.scale;
    public MissingChangeEffect MissingChangeEffect => missingChangeEffect;

    public int id {get; set; }

    public virtual void AddChange(Change change) {
        switch (change.mirror.changeType) {
            case ChangeType.missing:
                OnMissing();
                break;
            case ChangeType.tooSmall:
                OnShrinking();
                break;
        }    
    }
    public void RemoveChange(Change change) {
        switch (change.mirror.changeType) {
            case ChangeType.missing:
                OnAppearing();
                break;
            case ChangeType.tooSmall:
                OnShrinkRevert();
                break;
        }
    }

    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void OnAppearing()
    {
        if (Animated) {
            StartCoroutine(AnimateAppearing());
        } else {
            OnAppearingFinish();
        }
    }

    ///<summary>
    /// Fires when the object starts to disappear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void OnMissing()
    {
        currentScale = transform.localScale;
        if (Animated) {
            StartCoroutine(AnimateMissing());
        } else {
            OnMissingFinish();
        }
    }

    ///<summary>
    /// Coroutine that animates the roomobject into oblivion. 
    ///</summary>
    public virtual IEnumerator AnimateMissing() {
        yield return null;
        OnMissingFinish();
    }

    ///<summary>
    /// Coroutine that animates the roomobject into existing. 
    ///</summary>
    public virtual IEnumerator AnimateAppearing() {
        yield return null;
        OnAppearingFinish();
    }

    
    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be missing.
    ///</summary>
    public virtual void OnMissingFinish()
    {
    }

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be appearing.
    ///</summary>
    public virtual void OnAppearingFinish()
    {
        transform.localScale = currentScale;
    }

    #endregion


    public virtual void OnRoomEnter()
    {
        inSpace = true;
    }

    public virtual void OnRoomLeave()
    {
        inSpace = false;
    }

    #region  shrinking/enlarging
    public virtual void OnShrinking()
    {
        if (Animated) {
            StartCoroutine(AnimateShrinking());
        } else {
            OnShrinkingFinish();
        }
    }

    public virtual IEnumerator AnimateShrinking()
    {
        yield return null;
        OnShrinkingFinish();
    }

    public virtual void OnShrinkingFinish(){}

    public virtual void OnShrinkRevert()
    {
        if (Animated) {
            StartCoroutine(AnimateShrinkRevert());
        } else {
            OnShrinkingRevertFinish();
        }
    }

    public virtual IEnumerator AnimateShrinkRevert()
    {
        yield return null;
        OnShrinkingRevertFinish();
    }

    public virtual void OnShrinkingRevertFinish(){}
    #endregion
}
