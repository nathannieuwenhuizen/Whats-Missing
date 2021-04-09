using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Property : MonoBehaviour, IChangable
{
    [SerializeField]    protected string word;
    public string Word { get =>  word; set => word = value; }

    public bool animated { get; set; }

    private bool inSpace= false;
    public bool InSpace => inSpace;

    public Transform Transform => transform;

    public virtual void onAppearing()
    {
    }

    public virtual void onMissing()
    {
    }

    public void RemoveChange(ChangeType changeType)
    {
        switch (changeType) {
            case ChangeType.missing:
                onAppearing();
                break;
        }
    }

    public void SetChange(ChangeType changeType)
    {
        switch (changeType) {
            case ChangeType.missing:
                onMissing();
                break;
        }
    }
}
