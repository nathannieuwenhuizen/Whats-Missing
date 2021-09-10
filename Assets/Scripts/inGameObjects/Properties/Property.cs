using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
