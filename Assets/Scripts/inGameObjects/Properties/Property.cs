using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A non-physical object inside the room.
///</summary>
public abstract class Property : MonoBehaviour, IChangable
{
    [SerializeField]    protected string word;
    public string Word { get =>  word; set => word = value; }
    [SerializeField]
    private string[] alternateWords;
    public string[] AlternativeWords { get => alternateWords; set => alternateWords = value; }

    public bool Animated { get; set; }

    private bool inSpace= false;
    public bool InSpace => inSpace;

    public Transform Transform => transform;

    public MissingChangeEffect MissingChangeEffect => throw new System.NotImplementedException();

    public int id { get; set; }

    protected Change currentChange;

    public virtual void onAppearing()
    {
    }

    public virtual void onMissing()
    {
    }

    public void RemoveChange(Change change)
    {
        switch (change.television.changeType) {
            case ChangeType.missing:
                onAppearing();
                break;
        }
    }

    public void SetChange(Change change)
    {
        currentChange = change;
        switch (change.television.changeType) {
            case ChangeType.missing:
                onMissing();
                break;
        }
    }

    public virtual IEnumerator AnimateMissing()
    {
        throw new System.NotImplementedException();
    }

    public virtual void onMissingFinish()
    {
        throw new System.NotImplementedException();
    }

    public virtual IEnumerator AnimateAppearing()
    {
        throw new System.NotImplementedException();
    }

    public virtual void onAppearingFinish()
    {
        throw new System.NotImplementedException();
    }
}
