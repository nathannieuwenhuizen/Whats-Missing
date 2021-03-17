using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : MonoBehaviour, IChangable
{
    [SerializeField]    private string word;
    public string Word { get =>  word; set => word = value; }

    public bool animated { get; set; }

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
