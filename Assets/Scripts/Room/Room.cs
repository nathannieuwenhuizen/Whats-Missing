using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [SerializeField]
    private SaveData beginState;

    public delegate void MakeRoomAction(Room _room, Change _change, bool _changeIsAdded, string previousWord = "");
    public static MakeRoomAction OnMakeRoomAction;

    public static float TimeScale = 1f;

    private bool firstTimeEntering = true;
    public bool FirstTimeEntering {
        get { return firstTimeEntering;}
        set { firstTimeEntering = value; }
    }
    
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
    public List<RoomTelevision> AllTelevisions {get { return allTelevisions;}}
    [SerializeField]
    private List<Change> changes = new List<Change>();
    public List<Change> Changes { get => changes; }
    
    [SerializeField]
    private GameObject changeLineObject;

    [SerializeField]
    private GameObject plopParticle;

    [SerializeField]
    private Door endDoor;
    [SerializeField]
    private Door startDoor;
    public Door StartDoor {
        get { return startDoor;}
    }
    public Door EndDoor {
        get { return endDoor;}
    }

    private bool animated;
    public bool Animated {
        set { 
            animated = value;
            foreach (IChangable obj in allObjects) obj.Animated = value;
        }
        get => animated;
    }

    void Awake() {
        endDoor.room = this;
        startDoor.room = this;
        allObjects = GetAllObjectsInRoom<IChangable>();

        for (int i = 0; i < allObjects.Count; i++)
        {
            allObjects[i].id = i;
        }

        allTelevisions = GetAllObjectsInRoom<RoomTelevision>().OrderBy( allObjects => allObjects.isQuestion).ToList(); 
        for (int i = 0; i < allTelevisions.Count; i++)
        {
            allTelevisions[i].id = i;
            allTelevisions[i].Room = this;
        }
    }

    ///<summary>
    /// Finds all object with a transform propery in the room up to two children levels deep.
    ///</summary>
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

    ///<summary>
    /// Activate the existing changes in the room.
    ///</summary>
    public void ActivateChanges(){
        Debug.Log("activate changes!");
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            AddChangeInRoomObjects(changes[i]);
            changes[i].active = true;
        }
        
    }
    ///<summary>
    /// Deactivate all existing changes. Called when the player leaves the room or a question-lvl has been cleared. The changes are still in the list.
    ///</summary>
    public void DeactivateChanges(){
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            RemoveChangeInRoomObjects(changes[i]);
            changes[i].active = false;
        }
    }

    ///<summary>
    /// Checks and apply the change to the room 
    ///</summary>
    public void AddTVChange(RoomTelevision selectedTelevision, bool undoAble = true) {
        Change newChange = CreateChange(selectedTelevision);

        if (newChange != null) {
            AddChangeInRoomObjects(newChange);
            changes.Add(newChange);
            selectedTelevision.IsOn = true;
            if (undoAble) OnMakeRoomAction?.Invoke(this, newChange, true);
            CheckRoomCompletion();

        } else {
            selectedTelevision.IsOn = false;
        }
    }

    ///<summary>
    /// Checks in all object if it has the sme word as the change. If so it will send a callback with the Ichangable. Also returns true if any objects has the word.
    ///</summary>
    private bool DoesObjectWordMatch(Change change, Action<IChangable> callBack) {
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
        if (!selectedTelevision.isQuestion) {
            RemoveChangeInRoomObjects(removedChange);
            changes.Remove(removedChange);
        }

    }

    ///<summary>
    /// removes a change to the room updating the objects
    ///</summary>
    private void RemoveChangeInRoomObjects(Change change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.RemoveChange(change); });  
    }

    ///<summary>
    /// adds the change to the room updating the objects
    ///</summary>
    private void AddChangeInRoomObjects(Change change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.AddChange(change); });  
    }

    ///<summary>
    /// Spawns a changeline and waits for it to finish to implement the change.
    ///</summary>
    public IEnumerator AnimateChangeEffect(float delay, RoomTelevision tv, IChangable o, float duration, Action callback) {
        yield return new WaitForSecondsRealtime(delay);
        ChangeLine changeLine = Instantiate(changeLineObject).GetComponent<ChangeLine>();
        changeLine.SetDestination(tv.transform.position, o.Transform.position);
        yield return changeLine.MoveTowardsDestination();

        GameObject plop = Instantiate(plopParticle, o.Transform.position, Quaternion.identity);
        callback();
    }

    
    ///<summary>
    /// Checks if a tv question is correct with the changes that exist inside the room.
    ///</summary>
    public void CheckTVQuestion(RoomTelevision selectedTelevision, bool undoAble = true) {
        if (undoAble) {
            OnMakeRoomAction?.Invoke(this, new Change() {word = selectedTelevision.Word, television = selectedTelevision}, false, selectedTelevision.PreviousWord);
            selectedTelevision.PreviousWord = selectedTelevision.Word;
        }
        if (TVQuestionIsInchanges(selectedTelevision)) {
                selectedTelevision.IsOn = true;
                CheckRoomCompletion();
                return;
        }
        
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
    }

    private bool TVQuestionIsInchanges(RoomTelevision selectedTelevision) {
        foreach (Change change in changes)
        {
            if (change.word == selectedTelevision.Word || change.alternativeWords.Contains(selectedTelevision.Word)) {
                return true;
            }
        }
        return false;

    }

    ///<summary>
    /// Handles if the door should be open or not. 
    ///</summary>
    private void CheckRoomCompletion() {
        if (AllTelevisionsAreOn()) {
            endDoor.Locked = false;
            roomFinishedEvent?.Invoke();
            if (revealChangeAfterCompletion) {
                DeactivateChanges();
            }
        } else if (endDoor.Locked == false) {
            endDoor.Locked = true;
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

    public void OnRoomEnter(Player _player, bool loadSaveData = false) {
        player = _player;
        player.transform.parent = transform;
        AllObjects.Add(player);

        Animated = false;

        if (loadSaveData) {
            LoadState(SaveData.current);
        }
        if (firstTimeEntering) {
            firstTimeEntering = false;
            changes = LoadChanges();
            ActivateChanges();
            UpdateTVStates();
            CheckRoomCompletion();
        } else {
            if (revealChangeAfterCompletion) {
                if (AllTelevisionsAreOn() == false)
                    ActivateChanges();
            } else 
                ActivateChanges();
        }


        beginState = SaveData.GetStateOfRoom(this);
        
        Animated = true;
        roomEnterEvent?.Invoke();
    }

    public void OnRoomLeave() {
        AllObjects.Remove(Player);
        Player = null;

        Animated = false;
        DeactivateChanges();
    }

    ///<summary>
    /// Returns the list of all changes that the room has with the televisions at the time of the call.
    ///</summary>
    public List<Change> LoadChanges() {
        List<Change> result = new List<Change>();
        foreach (RoomTelevision tv in allTelevisions)
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

    ///<summary>
    /// Creates a tv change and returns null if the change doesn't find any objects with the word.
    ///</summary>
    public Change CreateChange(RoomTelevision selectedTelevision) {
        Change newChange = new Change(){word = selectedTelevision.Word, television = selectedTelevision};

        if (DoesObjectWordMatch(newChange, (IChangable obj) => {newChange.alternativeWords = obj.AlternativeWords; })) {
            return newChange;
        } else {
            return null;
        }
    }

    ///<summary>
    /// Updates the tv.ison states with the existing changes.
    ///</summary>
    public void UpdateTVStates() {
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (!tv.isQuestion) {
                tv.IsOn = changes.Find(c => c.television = tv) != null;
            } else {
                tv.IsOn = TVQuestionIsInchanges(tv);
            }
        }
    }


    ///<summary>
    /// Resets the room by deactivation all the changes and returns the room state and activates the changes 
    ///</summary>
    public void ResetRoom() {
        DeactivateChanges();
        foreach (RoomTelevision tv in allTelevisions)
        {
            tv.IsOn = false;
        }
        LoadState(beginState);
        changes = LoadChanges();
        ActivateChanges();
    }


    ///<summary>
    /// Loads the savedata with all the television states and the cordinates of the roomobjects
    ///</summary>
    public void LoadState(SaveData data) {
        Debug.Log(player);
        Debug.Log(data);
        player.transform.position = data.playerCordinates.position;
        player.transform.rotation = data.playerCordinates.rotation;
        List<PickableRoomObjectCordinates> cordinates = data.cordinates.ToList<PickableRoomObjectCordinates>();
        List<TVState> tvStates = data.tvStates.ToList<TVState>();
        foreach (PickableRoomObject item in GetAllObjectsInRoom<PickableRoomObject>())
        {
            // Debug.Log("item id" + item.id);
            PickableRoomObjectCordinates itemCordinate = cordinates.Find(x => x.id == item.id);
            item.transform.position = itemCordinate.position;
            item.transform.rotation = itemCordinate.rotation;
        }
        foreach (RoomTelevision tv in allTelevisions)
        {
            TVState tvState = tvStates.Find(x => x.id == tv.id);
            tv.DeselectLetters();
            Debug.Log("tv word = "+ tvState.word);
            tv.Word = tvState.word;
            // tv.UpdateAnswerTextPosition();
        }
    }
}
