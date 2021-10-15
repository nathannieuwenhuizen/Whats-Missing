using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A non-physical object inside the room.
///</summary>
public abstract class Property : MonoBehaviour, IChangable
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
            onAppearingFinish();
        }
    }

    public virtual void OnMissing()
    {
        if (Animated) {
            onShockwave?.Invoke(currentChange.television.transform);
            StartCoroutine(AnimateMissing());
        } else {
            onMissingFinish();
        }
    }

    public void RemoveChange(Change change)
    {
        switch (change.television.changeType) {
            case ChangeType.missing:
                OnAppearing();
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
        }
    }

    public virtual IEnumerator AnimateMissing()
    {
        yield return null;
        onMissingFinish();
    }

    public virtual void onMissingFinish()
    {
        
    }

    public virtual IEnumerator AnimateAppearing()
    {
        yield return null;
        onAppearingFinish();
    }

    public virtual void onAppearingFinish()
    {
    }
}
