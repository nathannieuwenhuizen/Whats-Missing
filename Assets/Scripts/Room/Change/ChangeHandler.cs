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
    /// Returns the list of all changes that the room has with the mirrors at the time of the call.
    ///</summary>
    private List<Change> LoadingChanges() {
        List<Change> result = new List<Change>();
        foreach (Mirror mirror in room.mirrors)
        {
            if (!mirror.isQuestion) {
                Change newChange = CreateChange(mirror);
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
    /// Creates a mirror change and returns null if the change doesn't find any objects with the word.
    ///</summary>
    public Change CreateChange(Mirror selectedMirror) {
        Change newChange = new Change(){
            word = selectedMirror.Word, 
            mirror = selectedMirror,
            roomIndexOffset = selectedMirror.roomIndexoffset
            };

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
                if (changes[i].roomIndexOffset == 0) {
                    room.AddChangeInRoomObjects(changes[i]);
                    changes[i].active = true;
                } else if (ConnectedRoomIsComplete(changes[i]) == false) {
                    room.AddChangeInRoomObjects(changes[i]);
                    changes[i].active = true;
                }
            }
        }
    }

    ///<summary>
    /// Activate the other changes if the room connected to it is not finished.
    ///</summary>
    public void CheckAndActiveOtherRoomChanges() {
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            if (changes[i].active == false) {
                if (changes[i].roomIndexOffset != 0) {
                    if (ConnectedRoomIsComplete(changes[i]) == false) {
                        room.AddChangeInRoomObjects(changes[i]);
                        changes[i].active = true;
                    }
                }
            }
        }
    }

    private bool ConnectedRoomIsComplete(Change change) {
        return room.Area.Rooms[room.Area.Rooms.IndexOf(room) + change.roomIndexOffset].AllMirrorsAreOn();
    }
    
    ///<summary>
    /// Deactivate all existing changes. Called when the player leaves the room or a question-lvl has been cleared. The changes are still in the list.
    /// If the force is false, the change can still be active if the roomoffset isnt 0 and the corresponding room isn't finished.
    ///</summary>
    public void DeactivateChanges(bool force = true){
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            if (changes[i].active) {
                if (force || changes[i].roomIndexOffset == 0) {
                    room.RemoveChangeInRoomObjects(changes[i]);
                    changes[i].active = false;
                } else {
                    if (ConnectedRoomIsComplete(changes[i])) {
                        room.RemoveChangeInRoomObjects(changes[i]);
                        changes[i].active = false;
                    }
                }
            }
        }
    }
    ///<summary>
    /// Returns true if the changes contains the word of the selected television
    ///</summary>
    public bool WordMatchesChanges(Mirror mirror) {
        foreach (Change change in changes)
        {
            if (change.word == mirror.Word || change.alternativeWords.Contains(mirror.Word)) {
                return true;
            }
        }
        return false;

    }

}
