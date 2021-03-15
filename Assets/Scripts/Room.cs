using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private Room nextRoom;
    private List<Television> allTelevisions;
    private List<IChangable> allObjects;
    private List<Change> changes = new List<Change>();
    
    [SerializeField]
    private Door door;

    private void Start() {
        door.room = this;
        allObjects = GetAllObjectsInRoom<IChangable>();
        allTelevisions = GetAllObjectsInRoom<Television>(); //TODO: sort by question
        SetupChanges();
    }

    public List<T> GetAllObjectsInRoom<T>() {
        List<T> result = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<T>() != null) {
                result.Add(transform.GetChild(i).GetComponent<T>());
            }
        }
        return result;
    }

    //prepare changes so that the room is already changedwhen player walks in.
    public void SetupChanges(){
        foreach (IChangable obj in allObjects) obj.animated = false;
        
        foreach (Television tv in allTelevisions)
        {
            tv.Room = this;
            if (tv.Word != "") {
                if (tv.isQuestion) ApplyQuestion(tv);
                else ApplyChange(tv);
            }
        }
        foreach (IChangable obj in allObjects) obj.animated = true;
    }

    // Apply the change to the object 
    public void ApplyChange(Television selectedTelevision) {
        bool hasChangedSomething = false;
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == selectedTelevision.Word) {
                obj.SetChange(selectedTelevision.changeType);
                hasChangedSomething = true;
            }
        }
        if (hasChangedSomething) {
            changes.Add(new Change(){word = selectedTelevision.Word, television = selectedTelevision});
            selectedTelevision.IsOn = true;
            CheckRoomCompletion();
        } else {
            selectedTelevision.IsOn = false;
        }
    }
    public void RemoveChange(Television removeTelevision) {
        if (!removeTelevision.IsOn) return;
        removeTelevision.IsOn = false;
        CheckRoomCompletion();
        
        if (!removeTelevision.isQuestion) {
            Change change = changes.Find(x => x.television == removeTelevision);
            if (change == null) return;
            changes.Remove(change);
            foreach (IChangable obj in allObjects)
            {
                if (obj.Word == change.word) {
                    obj.RemoveChange(change.television.changeType);
                }
            }
        }
    }
    

    public void ApplyQuestion(Television television) {
        bool questionIsCorrect = false;

        foreach (Television tv in allTelevisions)
        {
            if (tv.IsOn && !tv.isQuestion) {
                if (tv.Word == television.Word) {
                    questionIsCorrect = true;
                    television.IsOn = true;
                    CheckRoomCompletion();
                    return;
                }
            }
        }

        if (!questionIsCorrect) {
            television.IsOn = false;
        }
    }
    
    private void CheckRoomCompletion() {
        if (AllTelevisionsAreOn()) {
            door.Locked = false;
        } else {
            door.Locked = true;
        }
    }

    private bool AllTelevisionsAreOn() {
        foreach (Television tv in allTelevisions)
        {
            if (!tv.IsOn) return false;
        }
        return true;
    }

    public void OnRoomEnter() {
        //SetupChanges();

    }
    public void OnRoomLeave() {

    }
}
