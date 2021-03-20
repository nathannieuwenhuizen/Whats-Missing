using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Television> allTelevisions;
    private List<IChangable> allObjects;

    private Player player;
    public Player Player { 
        get => player; 
        set => player = value;
    }

    public List<IChangable> AllObjects {get { return allObjects;}}
    private List<Change> changes = new List<Change>();
    
    [SerializeField]
    private Transform startPos;

    public Transform getStartPos
    {
        get {return this.startPos;}
    }


    [SerializeField]
    private Door door;

    public bool Animated {
        set { 
            foreach (IChangable obj in allObjects) obj.animated = value;
        }
    }

    private void Awake() {
        door.room = this;
        allObjects = GetAllObjectsInRoom<IChangable>();
        allTelevisions = GetAllObjectsInRoom<Television>().OrderBy( allObjects => !allObjects.isQuestion).ToList(); 
        foreach(Television tv in allTelevisions) tv.Room = this;
    }

    public List<T> GetAllObjectsInRoom<T>(Transform tr = null) {
        List<T> result = new List<T>();
        if (tr == null) tr = transform;

        for(int i = 0; i < tr.childCount; i++) {
            if (tr.GetChild(i).GetComponent<T>() != null) {
                if (tr.GetChild(i).GetComponent<T>().ToString() != "null") {
                    result.Add(tr.GetChild(i).GetComponent<T>());
                }
            }
            List<T> childResult = GetAllObjectsInRoom<T>(tr.GetChild(i));
            result = result.Concat(childResult).ToList();
        }
        return result;
    }

    //prepare changes so that the room is already changedwhen player walks in.
    public void ActivateChanges(){
        Animated = false;
        
        foreach (Television tv in allTelevisions)
        {
            if (tv.Word != "") {
                if (tv.isQuestion) CheckQuestion(tv);
                else AddTVChange(tv);
            }
        }
        Animated = true;
    }
    public void DeactivateChanges(){
        Animated = false;
        
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            RemoveChange(changes[i]);
        }
        Animated = true;
    }

    // Apply the change to the object 
    public void AddTVChange(Television selectedTelevision) {
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

    //removes a tv change updating the room and tv
    public void RemoveTVChange(Television selectedTelevision) {
        if (!selectedTelevision.IsOn) return;
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
        if (!selectedTelevision.isQuestion) RemoveChange(changes.Find(x => x.television == selectedTelevision));
    }

    //removes a change to the room updating the objects
    private void RemoveChange(Change change) {
        if (change == null) return;
        changes.Remove(change);
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.word) {
                obj.RemoveChange(change.television.changeType);
            }
        }
    }
    

    public void CheckQuestion(Television selectedTelevision) {
        bool questionIsCorrect = false;

        foreach (Television tv in allTelevisions)
        {
            if (tv.IsOn && !tv.isQuestion) {
                if (tv.Word == selectedTelevision.Word) {
                    questionIsCorrect = true;
                    selectedTelevision.IsOn = true;
                    CheckRoomCompletion();
                    return;
                }
            }
        }

        if (!questionIsCorrect) {
            selectedTelevision.IsOn = false;
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
        ActivateChanges();
    }
    public void OnRoomLeave() {
        DeactivateChanges();
    }
}
