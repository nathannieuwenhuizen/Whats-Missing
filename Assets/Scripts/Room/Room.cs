using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [SerializeField]
    private ChangeHandler changeHandler;
    public ChangeHandler ChangeHandler {
        get { return changeHandler;}
    }

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
    public bool RevealChangeAfterCompletion {
        get { return revealChangeAfterCompletion;}
        set { revealChangeAfterCompletion = value; }
    }

    public List<RoomTelevision> allTelevisions;
    public List<IChangable> allObjects;

    private Player player;
    public Player Player { 
        get => player; 
        set => player = value;
    }

    public List<IChangable> AllObjects {get { return allObjects;}}
    public List<RoomTelevision> AllTelevisions {get { return allTelevisions;}}

    
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
        changeHandler = new ChangeHandler(this);
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

    public void LoadTVs() {
        for (int i = 0; i < allTelevisions.Count; i++)
        {
            allTelevisions[i].InitializeLetters();
            allTelevisions[i].UpdateIndicatorLight();
        }
    }

    ///<summary>
    /// Finds all object with a transform propery in the room up to two children levels deep.
    ///</summary>
    public List<T> GetAllObjectsInRoom<T>(Transform tr = null) {
        List<T> result = new List<T>();
        if (tr == null) tr = transform;
        result = new List<T>(tr.GetComponentsInChildren<T>());

        // for(int i = 0; i < tr.childCount; i++) {
        //     if (tr.GetChild(i).GetComponent<T>() != null) {
        //         if (tr.GetChild(i).GetComponent<T>().ToString() != "null") {
        //             result.Add(tr.GetChild(i).GetComponent<T>());
        //         }
        //     }
        //     List<T> childResult = GetAllObjectsInRoom<T>(tr.GetChild(i));
        //     result = result.Concat(childResult).ToList();
        // }
        return result;
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
        Destroy(plop, 5f);
        callback();
    }

    ///<summary>
    /// Checks in all object if it has the sme word as the change. If so it will send a callback with the Ichangable. Also returns true if any objects has the word.
    ///</summary>
    public bool DoesObjectWordMatch(Change change, Action<IChangable> callBack) {
        bool result = false;
        float delay = 0;
        float totalTime = 1f;
        List<IChangable> foundObjects = new List<IChangable>();

        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.word || obj.AlternativeWords.Contains(change.word)) {
                foundObjects.Add(obj);
                result = true;
            }
        }

        foreach (IChangable obj in foundObjects)
        {
            if (obj.Animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(delay, change.television, obj, 1f, () => {
                        callBack(obj);
                    }));
                    delay += (totalTime / (float)foundObjects.Count);
            } else callBack(obj);
        }
        return result;
    }

    ///<summary>
    /// Checks and apply the change to the room 
    ///</summary>
    public void AddTVChange(RoomTelevision selectedTelevision, bool undoAble = true) {
        Change newChange = changeHandler.CreateChange(selectedTelevision);

        if (newChange != null) {
            AddChangeInRoomObjects(newChange);
            changeHandler.Changes.Add(newChange);
            selectedTelevision.IsOn = true;
            if (undoAble) OnMakeRoomAction?.Invoke(this, newChange, true);
            CheckRoomCompletion();

        } else {
            selectedTelevision.IsOn = false;
        }
    }

    
    /// <summary> 
    ///removes a tv change updating the room and tv
    ///</summary>
    public void RemoveTVChange(RoomTelevision selectedTelevision, bool undoAble = true) {
        if (!selectedTelevision.IsOn) return;
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
        Change removedChange = changeHandler.Changes.Find(x => x.television == selectedTelevision);
        if (undoAble) OnMakeRoomAction?.Invoke(this, removedChange, false);
        if (!selectedTelevision.isQuestion) {
            RemoveChangeInRoomObjects(removedChange);
            changeHandler.Changes.Remove(removedChange);
        }

    }

    ///<summary>
    /// removes a change to the room updating the objects
    ///</summary>
    public void RemoveChangeInRoomObjects(Change change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.RemoveChange(change); });  
    }

    ///<summary>
    /// adds the change to the room updating the objects
    ///</summary>
    public void AddChangeInRoomObjects(Change change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.AddChange(change); });  
    }
    
    ///<summary>
    /// Checks if a tv question is correct with the changes that exist inside the room.
    ///</summary>
    public void CheckTVQuestion(RoomTelevision selectedTelevision, bool undoAble = true) {
        if (undoAble) {
            OnMakeRoomAction?.Invoke(this, new Change() {word = selectedTelevision.Word, television = selectedTelevision}, false, selectedTelevision.PreviousWord);
            selectedTelevision.PreviousWord = selectedTelevision.Word;
        }
        if (changeHandler.TVWordMatchesChanges(selectedTelevision)) {
                selectedTelevision.IsOn = true;
                CheckRoomCompletion();
                return;
        }
        
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
    }


    ///<summary>
    /// Handles if the door should be open or not. 
    ///</summary>
    private void CheckRoomCompletion() {
        if (AllTelevisionsAreOn()) {
            StartCoroutine(WaitBeforeOpeningDoor());
            if (revealChangeAfterCompletion) {
                changeHandler.DeactivateChanges();
            }
        } else {
            endDoor.Locked = true;
            if (revealChangeAfterCompletion) {
                changeHandler.ActivateChanges();
            }
        }
    }
    private IEnumerator WaitBeforeOpeningDoor() {
        yield return new WaitForSeconds(2f);
        if (AllTelevisionsAreOn()){
            roomFinishedEvent?.Invoke();
            endDoor.Locked = false;
        }
    }

    private bool AllTelevisionsAreOn() {
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (!tv.IsOn) return false;
        }
        return true;
    }


    ///<summary>
    /// Fire when the player goes into the room.
    ///</summary>
    public void OnRoomEnter(Player _player, bool loadSaveData = false) {
        player = _player;
        player.transform.parent = transform;
        AllObjects.Add(player);

        Animated = false;

        if (loadSaveData) {
            LoadState(SaveData.current);
        }

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.OnRoomEnter();
        }
        if (firstTimeEntering) {
            firstTimeEntering = false;
            changeHandler.LoadChanges();
            changeHandler.ActivateChanges();
            UpdateTVStates();
            CheckRoomCompletion();
        } else {
            if (revealChangeAfterCompletion) {
                if (AllTelevisionsAreOn() == false)
                    changeHandler.ActivateChanges();
            } else 
                changeHandler.ActivateChanges();
        }


        beginState = SaveData.GetStateOfRoom(this);
        
        Animated = true;
        roomEnterEvent?.Invoke();
    }


    ///<summary>
    /// Fires whem the player leaves the room.
    ///</summary>
    public void OnRoomLeave() {
        AllObjects.Remove(Player);
        Player = null;

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.OnRoomEnter();
        }

        Animated = false;
        changeHandler.DeactivateChanges();
    }

    ///<summary>
    /// Updates the tv.ison states with the existing changes.
    ///</summary>
    public void UpdateTVStates() {
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (!tv.isQuestion) {
                tv.IsOn = changeHandler.Changes.Find(c => c.television = tv) != null;
            } else {
                tv.IsOn = changeHandler.TVWordMatchesChanges(tv);
            }
        }
    }


    ///<summary>
    /// Resets the room by deactivation all the changes and returns the room state and activates the changes 
    ///</summary>
    public void ResetRoom() {
        changeHandler.DeactivateChanges();
        foreach (RoomTelevision tv in allTelevisions) tv.IsOn = false;
        
        LoadState(beginState);
        changeHandler.LoadChanges();
        changeHandler.ActivateChanges();
    }


    ///<summary>
    /// Loads the savedata with all the television states and the cordinates of the roomobjects
    ///</summary>
    public void LoadState(SaveData data) {
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
            tv.Word = tvState.word;
            // tv.UpdateAnswerTextPosition();
        }
    }
}
