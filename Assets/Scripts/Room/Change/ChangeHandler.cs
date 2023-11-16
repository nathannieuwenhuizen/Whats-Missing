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
    public List<RoomChange> RoomChanges { get => roomChanges; }

    private List<IChange> changes = new List<IChange>();
    public List<IChange> Changes { get => changes; }

    ///<summary>
    /// Returns the list of all changes that the room has with the mirrors at the time of the call.
    ///</summary>
    private void LoadingChanges() {
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
        mirrorChanges = result;
        changes = room.roomLevel != null ? new List<IChange>(room.roomLevel.roomInfo.loadedChanges) : new List<IChange>();
        foreach (IChange change in changes)
        {
            room.ForEachObjectWithMirrorWord(change, (IChangable obj) => {
            List<string> allWords = new List<string>(obj.AlternativeWords);
            allWords.Add(obj.Word);
            change.AltarnativeWords = allWords.ToArray(); 
            });
        }
    }
    
    public void LoadChanges() {
        LoadingChanges();
    }
    ///<summary>
    /// Creates a mirror change and returns null if the change doesn't find any objects with the word.
    ///</summary>
    public MirrorChange CreateChange(Mirror selectedMirror) {
        if (room.roomLevel != null)
            if (room.roomLevel.name == "MirrorChangeIntroduction" && selectedMirror.Word == "air") {
                Debug.Log("set achievement");
                SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.WhoNeedsAirAnyway);
            }

        MirrorChange newChange = new MirrorChange(){
            word = selectedMirror.Word, 
            mirror = selectedMirror,
            changeType = selectedMirror.MirrorData.changeType,
            active = true
            };

        if (room.ForEachObjectWithMirrorWord(newChange, (IChangable obj) => {
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
                room.AddChangeInRoomObjects(mirrorChanges[i]);
                mirrorChanges[i].active = true;
            }
        }
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            room.AddChangeInRoomObjects(changes[i]);
            changes[i].Active = true;
        }
    }

    private bool ConnectedRoomIsComplete(Change change) {
        return room.AllMirrorsAreOn();
    }
    
    ///<summary>
    /// Deactivate all existing changes. Called when the player leaves the room or a question-lvl has been cleared. The changes are still in the list.
    /// If the force is false, the change can still be active if the roomoffset isnt 0 and the corresponding room isn't finished.
    ///</summary>
    public void DeactivateChanges(bool force = true){
        for (int i = mirrorChanges.Count - 1; i >= 0; i--)
        {
            Debug.Log("change: " + mirrorChanges[i].word + " | " + mirrorChanges[i].active);
            if (mirrorChanges[i].active) {
                room.RemoveChangeInRoomObjects(mirrorChanges[i]);
                mirrorChanges[i].active = false;
            }
        }
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            room.RemoveChangeInRoomObjects(changes[i]);
            changes[i].Active = false;
        }

        for (int i = roomChanges.Count - 1; i >= 0; i--)
        {
            Debug.Log("room change removed" + roomChanges[i].word);
            room.RemoveChangeInRoomObjects(roomChanges[i]);
            roomChanges[i].Active = false;
        }
    }
    ///<summary>
    /// Returns true if the changes contains the word of the selected mirror
    ///</summary>
    public bool ContainsWord(Mirror mirror) {
        foreach (MirrorChange change in mirrorChanges) {
            if ((change.word == mirror.Word || change.alternativeWords.Contains(mirror.Word)) && change.mirror.MirrorData.changeType == mirror.ChangeType) {
                return true;
            }
        } 
        foreach (RoomChange change in roomChanges) {
            if ((change.roomObject.Word == mirror.Word || change.roomObject.AlternativeWords.Contains(mirror.Word)) && change.changeType == mirror.ChangeType) {
                return true;
            }
        }
        foreach (IChange change in Changes) {
            if ((change.Word == mirror.Word || (change.AltarnativeWords != null && change.AltarnativeWords.Contains(mirror.Word))) && change.ChangeType == mirror.ChangeType) {
                return true;
            }
        }
        return false;
    }


    ///<summary>
    /// Updates the room object changes when the event sender did an invoke.
    ///</summary>
    public void UpdateRoomObjectChanges(RoomObject _roomObject, ChangeType _changeType, bool _enabled) {
        //find if there is a change that has the same object and changetype
        int index = roomChanges.FindIndex(x => x.roomObject == (IChangable)_roomObject && x.changeType == _changeType);
        if (_enabled) {
            //check if not the same change exist
            if (index == -1) roomChanges.Add(new RoomChange() { roomObject = _roomObject, changeType = _changeType});
            
        } else {
            //if there is a change, remove that one since the change has been disabled
            if (index != -1) roomChanges.RemoveAt(index);
        }
    }

    public void AddPotionChange(Potion potion, IChangable changable) {
        RemovePotionChange(potion.ChangeType);

        //check if existing change has been made to the same object by a potion
        int index = roomChanges.FindIndex(x => x.changeCausation == ChangeCausation.potion && x.roomObject == changable);
        if (index != -1) {
            //same object? But different potion? remove, if not, do nothing
            if (roomChanges[index].changeType == potion.ChangeType) return;

            //remove change from other potion
            roomChanges[index].roomObject.RemoveChange(roomChanges[index]);
            roomChanges.RemoveAt(index);
        }
        
        //add change
        RoomChange newChange = new RoomChange() {changeType = potion.ChangeType, roomObject = changable, changeCausation = ChangeCausation.potion, Active = true};
        newChange.word = changable.Word;
        Debug.Log("new hcange by potion: " + newChange.word);
        roomChanges.Add(newChange);
        changable.AddChange(newChange);
    }
    public void RemovePotionChange(ChangeType _changeType) {
        int index = roomChanges.FindIndex(x => x.changeCausation == ChangeCausation.potion && x.changeType == _changeType);
        Debug.Log("index = " + index + " | length = " + roomChanges.Count);
        if (index != -1) {
            roomChanges[index].roomObject.RemoveChange(roomChanges[index]);
            roomChanges.RemoveAt(index);
        }
    }

    public void OnPlayerDie(Player _player) {

        for (int i = roomChanges.Count - 1; i >= 0; i--) {
            // if ((roomChanges[i].roomObject.Word == _player.Word  || roomChanges[i].roomObject.AlternativeWords.Contains(_player.Word)) 
            //     && roomChanges[i].changeCausation == ChangeCausation.potion) {
            //     RemovePotionChange(roomChanges[i].changeType);
            // }
            if (roomChanges[i].changeCausation == ChangeCausation.potion) {
                RemovePotionChange(roomChanges[i].changeType);
            }
        }
    }

    public void AddBossChange(IChange _change) {
        Changes.Add(_change);
        room.ChangeLineAnimated = false;
        room.AddChangeInRoomObjects(_change);
        room.ChangeLineAnimated = true;
    }
    public void RemoveBossChange(IChange _change) {
        Changes.Remove(_change);
        room.ChangeLineAnimated = false;
        room.RemoveChangeInRoomObjects(_change);
        room.ChangeLineAnimated = true;

    }
    
}
