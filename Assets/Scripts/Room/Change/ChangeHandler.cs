using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


///<summary>
/// Is responsible for managing the changes inside the room. It activates, deactivates and creates them.
///</summary>
[System.Serializable]
public class ChangeHandler
{

    private Room room;

    public ChangeHandler(Room _room) {
        room = _room;
    }

    [SerializeField]
    private List<Change> changes = new List<Change>();
    public List<Change> Changes { get => changes; }

    ///<summary>
    /// Returns the list of all changes that the room has with the televisions at the time of the call.
    ///</summary>
    private List<Change> LoadingChanges() {
        List<Change> result = new List<Change>();
        foreach (RoomTelevision tv in room.allTelevisions)
        {
            if (!tv.isQuestion) {
                Change newChange = CreateChange(tv);
                if (newChange != null) {
                    result.Add(newChange);
                }
            }
        }
        return result;
    }
    public void LoadChanges() {
        changes = LoadingChanges();
    }
    ///<summary>
    /// Creates a tv change and returns null if the change doesn't find any objects with the word.
    ///</summary>
    public Change CreateChange(RoomTelevision selectedTelevision) {
        Change newChange = new Change(){word = selectedTelevision.Word, television = selectedTelevision};

        if (room.DoesObjectWordMatch(newChange, (IChangable obj) => {newChange.alternativeWords = obj.AlternativeWords; })) {
            return newChange;
        } else {
            return null;
        }
    }

    ///<summary>
    /// Activate the existing changes in the room.
    ///</summary>
    public void ActivateChanges(){
        Debug.Log("activate changes!");
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            if (changes[i].active == false) {
                room.AddChangeInRoomObjects(changes[i]);
                changes[i].active = true;
            }
        }
    }
    ///<summary>
    /// Deactivate all existing changes. Called when the player leaves the room or a question-lvl has been cleared. The changes are still in the list.
    ///</summary>
    public void DeactivateChanges(){
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            if (changes[i].active) {
                room.RemoveChangeInRoomObjects(changes[i]);
                changes[i].active = false;
            }
        }
    }
    ///<summary>
    /// Returns true if the changes contains the word of the selected television
    ///</summary>
    public bool TVWordMatchesChanges(RoomTelevision selectedTelevision) {
        foreach (Change change in changes)
        {
            if (change.word == selectedTelevision.Word || change.alternativeWords.Contains(selectedTelevision.Word)) {
                return true;
            }
        }
        return false;

    }

}
