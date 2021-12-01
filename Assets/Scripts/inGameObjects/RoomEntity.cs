using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomEntity :  MonoBehaviour, IChangable, IRoomObject
{
    [SerializeField]
    protected string word;

    [SerializeField]
    protected string[] alternateWords;

    protected Vector3 startMissingScale;

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

    protected float largeScale = 2;
    public float LargeScale => largeScale;

    protected float shrinkScale = .5f;
    public float ShrinkScale => shrinkScale;
    public Coroutine ShrinkCoroutine {get; set;}

    protected float normalScale = 1f;
    public float NormalScale => normalScale;

    protected float animationDuration = 3f;
    public float AnimationDuration { get => animationDuration; set => animationDuration = value; }
    public bool IsShrinked { get; set; } = false;
    public bool IsEnlarged { get; set; } = false;
    public bool IsMissing { get; set; } = false;

    public virtual void AddChange(Change change) {
        switch (change.mirror.changeType) {
            case ChangeType.missing:
                OnMissing();
                break;
            case ChangeType.tooSmall:
                OnShrinking();
                break;
            case ChangeType.tooBig:
                OnEnlarge();
                break;
            case ChangeType.flipped:
                OnFlipped();
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
            case ChangeType.tooBig:
                OnEnlargeRevert();
                break;
            case ChangeType.flipped:
                OnFlippingRevert();
                break;

        }
    }

    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void OnAppearing()
    {
        IsMissing = false;
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
        IsMissing = true;
        startMissingScale = transform.localScale;
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
        OnAppearingFinish();
        yield return null;
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
        transform.localScale = startMissingScale;
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
        IsShrinked = true;
        if (ShrinkCoroutine != null) StopCoroutine(ShrinkCoroutine);
        if (Animated) {
            ShrinkCoroutine = StartCoroutine(AnimateShrinking());
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
        IsShrinked = false;

        if (ShrinkCoroutine != null) StopCoroutine(ShrinkCoroutine);
        if (Animated) {
            ShrinkCoroutine = StartCoroutine(AnimateShrinkRevert());
        }
        else OnShrinkingRevertFinish();
    }

    public virtual IEnumerator AnimateShrinkRevert()
    {
        yield return null;
        OnShrinkingRevertFinish();
    }

    public virtual void OnShrinkingRevertFinish(){}

    public virtual void OnEnlarge()
    {
        IsEnlarged = true;
        if (Animated)StartCoroutine(AnimateEnlarging());
        else OnEnlargingFinish();
    }

    public virtual IEnumerator AnimateEnlarging()
    {
        yield return null;
        OnEnlargingFinish();
    }

    public virtual void OnEnlargingFinish(){}

    public virtual void OnEnlargeRevert()
    {
        IsEnlarged = false;

        if (Animated)StartCoroutine(AnimateEnlargeRevert());
        else OnEnlargeRevertFinish();
    }

    public virtual IEnumerator AnimateEnlargeRevert()
    {
        yield return null;
        OnEnlargeRevertFinish();
    }

    public virtual void OnEnlargeRevertFinish(){}

    #endregion

    #region flipped
    public virtual void OnFlipped()
    {
        if (Animated)StartCoroutine(AnimateFlipping());
        else OnFlippingFinish();
    }

    public virtual IEnumerator AnimateFlipping()
    {
        yield return null;
        OnFlippingFinish();
    }

    public virtual void OnFlippingFinish(){}

    public virtual void OnFlippingRevert()
    {
        if (Animated)StartCoroutine(AnimateFlippingRevert());
        else OnFlippingRevertFinish();
    }

    public virtual IEnumerator AnimateFlippingRevert()
    {
        yield return null;
        OnFlippingRevertFinish();
    }

    public virtual void OnFlippingRevertFinish(){}
    #endregion
}
