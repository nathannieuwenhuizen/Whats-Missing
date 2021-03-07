using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    private List<Television> allTelevisions;
    private List<IChangable> allObjects;
    private List<Change> changes;
    
    [SerializeField]
    private Door door;

    private void Start() {
        allObjects = GetAllObjectsInRoom();
        allTelevisions = GetAllTVsInRoom();
        prepareChanges();
    }
    public List<IChangable> GetAllObjectsInRoom() {
        List<IChangable> result = new List<IChangable>();
        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<IChangable>() != null) {
                result.Add(transform.GetChild(i).GetComponent<IChangable>());
            }
        }
        return result;
    }
    public List<Television> GetAllTVsInRoom() {
        List<Television> result = new List<Television>();
        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<Television>() != null) {
                result.Add(transform.GetChild(i).GetComponent<Television>());
            }
        }
        return result;
    }

    //prepare changes so that the room is already changedwhen player walks in.
    public void prepareChanges(){
        foreach (IChangable obj in allObjects)
        {
            obj.animated = false;
        }
        foreach (Television tv in allTelevisions)
        {
            tv.Room = this;
            Debug.Log(tv.Word);
            if (tv.Word != "") {
                if (tv.isQuestion) ApplyQuestion(tv);
                else ApplyChange(tv);
            }
        }
        foreach (IChangable obj in allObjects)
        {
            obj.animated = true;
        }
    }

    // Apply the change to the object 
    public void ApplyChange(Television selectedTelevision) {
        bool hasChangedSomething = false;
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == selectedTelevision.Word) {
                obj.setChange(selectedTelevision.changeType);
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
            //door open
            door.Locked = false;
        } else {
            //door closed
            door.Locked = true;
        }
    }

    private bool AllTelevisionsAreOn() {
        foreach (Television tv in allTelevisions)
        {
            if (!tv.IsOn) {
                return false;
            }
        }
        return true;
    }
}
