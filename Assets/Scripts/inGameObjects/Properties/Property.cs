using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A non-physical object inside the room.
///</summary>
public abstract class Property : MonoBehaviour, IChangable, IRoomObject
{

    public delegate void OnPropertyToggle();


    [SerializeField]    protected string word;
    public string Word { get =>  word; set => word = value; }
    [SerializeField]
    private string[] alternateWords;
    public string[] AlternativeWords { get => alternateWords; set => alternateWords = value; }

    public bool Animated { get; set; }

    private bool inSpace= false;
    public bool InSpace => inSpace;

    public delegate void OnShockwave(Transform origin);
    public static event OnShockwave onShockwave;


    public Transform Transform => transform;

    public MissingChangeEffect MissingChangeEffect => throw new System.NotImplementedException();

    public int id { get; set; }

    protected Change currentChange;

    public virtual void OnAppearing()
    {
        if (Animated) {
            onShockwave?.Invoke(currentChange.television.transform);
            StartCoroutine(AnimateAppearing());
        } else {
            OnAppearingFinish();
        }
    }

    public virtual void OnMissing()
    {
        if (Animated) {
            onShockwave?.Invoke(currentChange.television.transform);
            StartCoroutine(AnimateMissing());
        } else {
            OnMissingFinish();
        }
    }

    public void RemoveChange(Change change)
    {
        switch (change.television.changeType) {
            case ChangeType.missing:
                OnAppearing();
                break;
            case ChangeType.tooSmall:
                OnShrinkRevert();
                break;
        }
    }

    public void AddChange(Change change)
    {
        currentChange = change;
        switch (change.television.changeType) {
            case ChangeType.missing:
                OnMissing();
                break;
            case ChangeType.tooSmall:
                OnShrinking();
                break;
        }
    }

    public virtual IEnumerator AnimateMissing()
    {
        yield return null;
        OnMissingFinish();
    }

    public virtual void OnMissingFinish()
    {
        
    }

    public virtual IEnumerator AnimateAppearing()
    {
        yield return null;
        OnAppearingFinish();
    }

    public virtual void OnAppearingFinish()
    {
    }

    public virtual void OnRoomEnter()
    {
        inSpace = true;
    }

    public virtual void OnRoomLeave()
    {
        inSpace = false;
    }
    
    #region  shrinking/enlarging
    public void OnShrinking()
    {
        if (Animated) {
            onShockwave?.Invoke(currentChange.television.transform);
            StartCoroutine(AnimateShrinking());
        } else {
            OnShrinkingFinish();
        }
    }

    public IEnumerator AnimateShrinking()
    {
        yield return null;
        OnShrinkingFinish();
    }

    public void OnShrinkingFinish()
    {
    }

    public void OnShrinkRevert()
    {
        if (Animated) {
            onShockwave?.Invoke(currentChange.television.transform);
            StartCoroutine(AnimateShrinkRevert());
        } else {
            OnShrinkingRevertFinish();
        }
    }

    public IEnumerator AnimateShrinkRevert()
    {
        yield return null;
        OnShrinkingRevertFinish();
    }

    public void OnShrinkingRevertFinish()
    {
    }
    #endregion
}
