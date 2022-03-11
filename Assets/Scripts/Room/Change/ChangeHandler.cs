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

    ///<summary>
    /// Changes made by the mirrors
    ///</summary>
    private List<MirrorChange> mirrorChanges = new List<MirrorChange>();
    public List<MirrorChange> MirrorChanges { get => mirrorChanges; }

    //changes made by the palyer, mirror or enviroment.
    private List<RoomChange> roomChanges = new List<RoomChange>();
    public List<RoomChange> RoomObjectChanges { get => roomChanges; }



    ///<summary>
    /// Returns the list of all changes that the room has with the mirrors at the time of the call.
    ///</summary>
    private List<MirrorChange> LoadingChanges() {
        List<MirrorChange> result = new List<MirrorChange>();
        foreach (Mirror mirror in room.mirrors)
        {
            if (!mirror.isQuestion) {
                MirrorChange newChange = CreateChange(mirror);
                if (newChange != null) {
                    result.Add(newChange);
                }
            }
        }
        return result;
    }
    
    public void LoadChanges() {
        mirrorChanges = LoadingChanges();
    }
    ///<summary>
    /// Creates a mirror change and returns null if the change doesn't find any objects with the word.
    ///</summary>
    public MirrorChange CreateChange(Mirror selectedMirror) {
        MirrorChange newChange = new MirrorChange(){
            word = selectedMirror.Word, 
            mirror = selectedMirror,
            changeType = selectedMirror.changeType,
            roomIndexOffset = selectedMirror.roomIndexoffset
            };

        if (room.DoesObjectWordMatch(newChange, (IChangable obj) => {
            List<string> allWords = new List<string>(obj.AlternativeWords);
            allWords.Add(obj.Word);
            newChange.alternativeWords = allWords.ToArray(); 
            })) {
            return newChange;
        } else {
            return null;
        }
    }

    ///<summary>
    /// Activate the existing changes in the room.
    ///</summary>
    public void ActivateChanges(){
        // Debug.Log("activate changes!");
        for (int i = mirrorChanges.Count - 1; i >= 0; i--)
        {
            if (mirrorChanges[i].active == false) {
                if (mirrorChanges[i].roomIndexOffset == 0) {
                    room.AddChangeInRoomObjects(mirrorChanges[i]);
                    mirrorChanges[i].active = true;
                } else if (ConnectedRoomIsComplete(mirrorChanges[i]) == false) {
                    room.AddChangeInRoomObjects(mirrorChanges[i]);
                    mirrorChanges[i].active = true;
                }
            }
        }
    }

    ///<summary>
    /// Activate the other changes if the room connected to it is not finished.
    ///</summary>
    public void CheckAndActiveOtherRoomChanges() {
        for (int i = mirrorChanges.Count - 1; i >= 0; i--)
        {
            if (mirrorChanges[i].active == false) {
                if (mirrorChanges[i].roomIndexOffset != 0) {
                    if (ConnectedRoomIsComplete(mirrorChanges[i]) == false) {
                        room.AddChangeInRoomObjects(mirrorChanges[i]);
                        mirrorChanges[i].active = true;
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
        for (int i = mirrorChanges.Count - 1; i >= 0; i--)
        {
            if (mirrorChanges[i].active) {
                if (force || mirrorChanges[i].roomIndexOffset == 0) {
                    room.RemoveChangeInRoomObjects(mirrorChanges[i]);
                    mirrorChanges[i].active = false;
                } else {
                    if (ConnectedRoomIsComplete(mirrorChanges[i])) {
                        room.RemoveChangeInRoomObjects(mirrorChanges[i]);
                        mirrorChanges[i].active = false;
                    }
                }
            }
        }
    }
    ///<summary>
    /// Returns true if the changes contains the word of the selected television
    ///</summary>
    public bool WordMatchesChanges(Mirror mirror) {
        foreach (MirrorChange change in mirrorChanges) {
            if ((change.word == mirror.Word || change.alternativeWords.Contains(mirror.Word)) && change.mirror.changeType == mirror.changeType) {
                return true;
            }
        } 
        foreach (RoomChange change in roomChanges) {
            if ((change.roomObject.Word == mirror.Word || change.roomObject.AlternativeWords.Contains(mirror.Word)) && change.changeType == mirror.changeType) {
                return true;
            }
        }
        return false;
    }


    ///<summary>
    /// Updates the room object changes when the event sender did an invoke.
    ///</summary>
    public void UpdateRoomObjectChanges(RoomObject _roomObject, ChangeType _changeType, bool _enabled) {
        int index = roomChanges.FindIndex(x => x.roomObject == (IChangable)_roomObject);
        Debug.Log("index: " + index);
        if (_enabled) {
            if (index == -1) roomChanges.Add(new RoomChange() { roomObject = _roomObject, changeType = _changeType});
            
        } else {
            if (index != -1) roomChanges.RemoveAt(index);
        }
    }

    public void AddPotionChange(Potion potion, IChangable changable) {
        //check if existing change has been made by same potion
        int index = roomChanges.FindIndex(x => x.changeCausation == ChangeCausation.potion && x.changeType == potion.ChangeType);
        Debug.Log("index: " + index);

        bool sameObject = false;
        if (index != -1) {
            //same object? then just ignore the rest
            sameObject = roomChanges[index].roomObject == changable;
            if (sameObject) return;

            //remove other change
            roomChanges[index].roomObject.RemoveChange(roomChanges[index]);
            roomChanges.RemoveAt(index);

        }
        RoomChange newChange = new RoomChange() {changeType = potion.ChangeType, roomObject = changable, changeCausation = ChangeCausation.potion};
        roomChanges.Add(newChange);
        changable.AddChange(newChange);
    }
}
