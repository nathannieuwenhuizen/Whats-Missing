using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{

    public delegate void MakeRoomAction(Room _room, Change _change, bool _changeIsAdded, string previousWord = "");
    public static MakeRoomAction OnMakeRoomAction;

    public static float TimeScale = 1f;
    
    [SerializeField]
    private UnityEvent roomFinishedEvent;
    [SerializeField]
    private UnityEvent roomEnterEvent;

    private bool revealChangeAfterCompletion = true;

    private List<RoomTelevision> allTelevisions;
    private List<IChangable> allObjects;

    private Player player;
    public Player Player { 
        get => player; 
        set => player = value;
    }

    public List<IChangable> AllObjects {get { return allObjects;}}
    [SerializeField]
    private List<Change> changes = new List<Change>();
    public List<Change> Changes { get => changes; }
    
    [SerializeField]
    private Transform startPos;

    public Transform StartPos
    {
        get {return this.startPos;}
    }

    [SerializeField]
    private Transform endPos;
    public Transform EndPos
    {
        get {return this.endPos;}
    }

    [SerializeField]
    private GameObject changeLineObject;

    [SerializeField]
    private GameObject plopParticle;

    [SerializeField]
    private Door door;

    private bool animated;
    public bool Animated {
        set { 
            animated = value;
            foreach (IChangable obj in allObjects) obj.Animated = value;
        }
        get => animated;
    }

    void Awake() {
        door.room = this;
        allObjects = GetAllObjectsInRoom<IChangable>();

        for (int i = 0; i < allObjects.Count; i++)
        {
            allObjects[i].id = i;
        }

        allTelevisions = GetAllObjectsInRoom<RoomTelevision>().OrderBy( allObjects => !allObjects.isQuestion).ToList(); 
        for (int i = 0; i < allTelevisions.Count; i++)
        {
            allTelevisions[i].id = i;
            allTelevisions[i].Room = this;
        }
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
        
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (tv.Word != "") {
                if (tv.isQuestion) CheckQuestion(tv, false);
                else AddTVChange(tv, false);
            }
        }
    }

    //Deactivate all changes 
    public void DeactivateChanges(){
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            RemoveChange(changes[i]);
        }
    }

    ///<summary>
    /// Apply the change to the room 
    ///</summary>
    public void AddTVChange(RoomTelevision selectedTelevision, bool undoAble = true) {
        bool hasChangedSomething = false;
        // float delay = 0;
        Change newChange = new Change(){word = selectedTelevision.Word, television = selectedTelevision};

        if (ToggleChangeAllObjects(newChange, (IChangable obj) => { obj.SetChange(newChange); newChange.AlternativeWords = obj.AlternativeWords; })) {
            hasChangedSomething = true;
        }
        if (hasChangedSomething) {
            changes.Add(newChange);
            selectedTelevision.IsOn = true;
            if (undoAble) OnMakeRoomAction?.Invoke(this, newChange, true);
            CheckRoomCompletion();
        } else {
            selectedTelevision.IsOn = false;
        }
    }

    ///<summary>
    /// Checks in all object if it has the sme word as the change. If so it will change or its change will be removed. Also returns true if any objects has the word.
    ///</summary>
    private bool ToggleChangeAllObjects(Change change, Action<IChangable> callBack) {
        bool result = false;
        float delay = 0;
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.word || obj.AlternativeWords.Contains(change.word)) {
                result = true;
                if (obj.Animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(delay, change.television, obj, 1f, () => {
                        callBack(obj);
                    }));
                    delay += .5f;
                } else callBack(obj);
            }
        }
        return result;
    }

    
    /// <summary> 
    ///removes a tv change updating the room and tv
    ///</summary>
    public void RemoveTVChange(RoomTelevision selectedTelevision, bool undoAble = true) {
        if (!selectedTelevision.IsOn) return;
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
        Change removedChange = changes.Find(x => x.television == selectedTelevision);
        if (undoAble) OnMakeRoomAction?.Invoke(this, removedChange, false);
        if (!selectedTelevision.isQuestion) RemoveChange(removedChange);

    }

    //removes a change to the room updating the objects
    private void RemoveChange(Change change) {
        ToggleChangeAllObjects(change, (IChangable obj) => { obj.RemoveChange(change); });
        changes.Remove(change);
    }

    public IEnumerator AnimateChangeEffect(float delay, RoomTelevision tv, IChangable o, float duration, Action callback) {
        yield return new WaitForSecondsRealtime(delay);
        ChangeLine changeLine = Instantiate(changeLineObject).GetComponent<ChangeLine>();
        changeLine.SetDestination(tv.transform.position, o.Transform.position);
        yield return changeLine.MoveTowardsDestination();

        GameObject plop = Instantiate(plopParticle, o.Transform.position, Quaternion.identity);
        callback();
    }

    

    public void CheckQuestion(RoomTelevision selectedTelevision, bool undoAble = true) {
        // Debug.Log("check question " + selectedTelevision.Word);
        // Debug.Log("changes length: " + changes.Count);
        if (undoAble) {
            OnMakeRoomAction?.Invoke(this, new Change() {word = selectedTelevision.Word, television = selectedTelevision}, false, selectedTelevision.PreviousWord);
            selectedTelevision.PreviousWord = selectedTelevision.Word;
        }
        // Debug.Log("changes length: " + changes.Count);
        foreach (Change change in changes)
        {
            if (change.word == selectedTelevision.Word || change.AlternativeWords.Contains(selectedTelevision.Word)) {
                selectedTelevision.IsOn = true;
                CheckRoomCompletion();
                return;
            }
        }
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
    }
    
    private void CheckRoomCompletion() {
        if (AllTelevisionsAreOn()) {
            door.Locked = false;
            roomFinishedEvent?.Invoke();
            if (revealChangeAfterCompletion) {
                DeactivateChanges();
            }
        } else if (door.Locked == false) {
            Debug.Log("locked");
            door.Locked = true;
            if (revealChangeAfterCompletion) {
                ActivateChanges();
            }
        }
    }

    private bool AllTelevisionsAreOn() {
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (!tv.IsOn) return false;
        }
        return true;
    }

    public void OnRoomEnter(Player _player) {
        player = _player;
        player.transform.parent = transform;
        AllObjects.Add(player);

        Animated = false;
        ActivateChanges();
        Animated = true;
        roomEnterEvent?.Invoke();
    }
    public void OnRoomLeave() {
        AllObjects.Remove(Player);
        Player = null;

        Animated = false;
        DeactivateChanges();
    }
}
