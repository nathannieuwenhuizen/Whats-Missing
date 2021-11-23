using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{

    public delegate void RoomAction();
    public static event RoomAction OnRoomComplete;
    public static event RoomAction OnRoomLeaving;

    public RoomLevel roomLevel;

    [SerializeField]
    private HintStopwatch hintStopwatch;

    private Area area;
    public Area Area {
        get { return area;}
        set { area = value; }
    }
    [SerializeField]
    private ChangeHandler changeHandler;
    public ChangeHandler ChangeHandler {
        get { return changeHandler;}
    }
    private RoomStateHandler roomstateHandler;

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

    public List<Mirror> mirrors;
    public List<IChangable> allObjects;

    [SerializeField]
    private Player player;
    public Player Player { 
        get => player; 
        set => player = value;
    }

    public List<IChangable> AllObjects {get { return allObjects;}}
    public List<Mirror> Mirrors {get { return mirrors;}}

    
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
        roomstateHandler = new RoomStateHandler(this);

        endDoor.room = this;
        startDoor.room = this;
        allObjects = GetAllObjectsInRoom<IChangable>();
        for (int i = 0; i < allObjects.Count; i++)
        {
            allObjects[i].id = i;
        }

        mirrors = GetAllObjectsInRoom<Mirror>().OrderBy( allObjects => allObjects.isQuestion).ToList(); 
        for (int i = 0; i < mirrors.Count; i++)
        {
            mirrors[i].id = i;
            mirrors[i].Room = this;
        }
    }

    public void LoadMirrors() {
        hintStopwatch.room = this;
        hintStopwatch.Duration = roomLevel.roomInfo.durationBeforeHint;
        for (int i = 0; i < mirrors.Count; i++)
        {
            mirrors[i].SetupCanvas();
            mirrors[i].UpdateIndicatorLight();
        }
    }

    ///<summary>
    /// Finds all object with a transform propery in the room up to two children levels deep.
    ///</summary>
    public List<T> GetAllObjectsInRoom<T>(Transform tr = null) {
        List<T> result = new List<T>();
        if (tr == null) tr = transform;
        result = new List<T>(tr.GetComponentsInChildren<T>(false));
        return result;
    }
    
    ///<summary>
    /// Spawns a changeline and waits for it to finish to implement the change.
    ///</summary>
    public IEnumerator AnimateChangeEffect(float delay, Mirror tv, IChangable o, float duration, Action callback) {
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
                    StartCoroutine(AnimateChangeEffect(delay, change.mirror, obj, 1f, () => {
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
    public void AddTVChange(Mirror selectedMirror, bool undoAble = true) {
        Change newChange = changeHandler.CreateChange(selectedMirror);

        if (newChange != null) {
            AddChangeInRoomObjects(newChange);
            changeHandler.Changes.Add(newChange);
            selectedMirror.IsOn = true;
            if (undoAble) OnMakeRoomAction?.Invoke(this, newChange, true);
            CheckRoomCompletion();

        } else {
            selectedMirror.IsOn = false;
        }
    }

    
    /// <summary> 
    ///removes a mirror  change updating the room and mirror
    ///</summary>
    public void RemoveMirrorChange(Mirror selectedMirror, bool undoAble = true) {
        if (!selectedMirror.IsOn) return;
        selectedMirror.IsOn = false;
        CheckRoomCompletion();
        Change removedChange = changeHandler.Changes.Find(x => x.mirror == selectedMirror);
        if (undoAble) OnMakeRoomAction?.Invoke(this, removedChange, false);
        if (!selectedMirror.isQuestion) {
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
    /// Checks if a mirror question is correct with the changes that exist inside the room.
    ///</summary>
    public void CheckTVQuestion(Mirror selectedMirror, bool undoAble = true) {
        if (undoAble) {
            OnMakeRoomAction?.Invoke(this, new Change() {word = selectedMirror.Word, mirror = selectedMirror}, false, selectedMirror.PreviousWord);
            selectedMirror.PreviousWord = selectedMirror.Word;
        }

        ChangeHandler checkChangeHandler = changeHandler;
        if (selectedMirror.roomIndexoffset == -1) {
            checkChangeHandler = area.Rooms[area.Rooms.IndexOf(this) - 1].ChangeHandler;
        }
        if (checkChangeHandler.WordMatchesChanges(selectedMirror)) {
                selectedMirror.IsOn = true;
                CheckRoomCompletion();
                return;
        }
        
        selectedMirror.IsOn = false;
        CheckRoomCompletion();
    }


    ///<summary>
    /// Handles if the door should be open or not. 
    ///</summary>
    private void CheckRoomCompletion() {
        if (AllMirrorsAreOn()) {
            hintStopwatch.Pause();
            OnRoomComplete?.Invoke();
            StartCoroutine(WaitBeforeOpeningDoor());
            if (revealChangeAfterCompletion) {
                changeHandler.DeactivateChanges(false);
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
        if (AllMirrorsAreOn()){
            roomFinishedEvent?.Invoke();
            endDoor.Locked = false;
        }
    }

    public bool AllMirrorsAreOn() {
        foreach (Mirror mirror in mirrors)
        {
            if (!mirror.IsOn) return false;
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

        hintStopwatch.Resume();
        Animated = false;

        if (loadSaveData) {
            roomstateHandler.LoadState(SaveData.current);
        }

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.OnRoomEnter();
        }
        if (firstTimeEntering) {
            firstTimeEntering = false;
            changeHandler.LoadChanges();
            changeHandler.ActivateChanges();
            UpdateMirrorStates();
            CheckRoomCompletion();
        } else {
            if (revealChangeAfterCompletion) {
                if (AllMirrorsAreOn() == false)
                    changeHandler.ActivateChanges();
                else 
                    changeHandler.CheckAndActiveOtherRoomChanges();
            } else 
                changeHandler.ActivateChanges();
        }


        beginState = SaveData.GetStateOfRoom(this);
        
        Animated = true;
        roomEnterEvent?.Invoke();
    }

    public void ShowMirrorToggleHint() {
        foreach(Mirror mirror in mirrors) {
            if (mirror.isQuestion) {
                mirror.MirrorCanvas.ShowHintButton(roomLevel.roomInfo.hintText);
                break;
            }
        }
    }


    ///<summary>
    /// Fires whem the player leaves the room.
    ///</summary>
    public void OnRoomLeave() {

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.OnRoomLeave();
        }

        Animated = false;
        changeHandler.DeactivateChanges();
        hintStopwatch.Pause();
        OnRoomLeaving?.Invoke();
        AllObjects.Remove(Player);
        // Player = null;
    }

    ///<summary>
    /// Updates the tv.ison states with the existing changes.
    ///</summary>
    public void UpdateMirrorStates() {
        foreach (Mirror mirror in mirrors)
        {
            if (!mirror.isQuestion) {
                mirror.IsOn = changeHandler.Changes.Find(c => c.mirror == mirror) != null;
            } else {
                mirror.IsOn = changeHandler.WordMatchesChanges(mirror);
            }
        }
    }


    ///<summary>
    /// Resets the room by deactivation all the changes and returns the room state and activates the changes 
    ///</summary>
    public void ResetRoom() {
        Animated = false;
        changeHandler.DeactivateChanges();
        foreach (Mirror mirror in mirrors) mirror.IsOn = false;
        
        roomstateHandler.LoadState(beginState);
        changeHandler.LoadChanges();
        changeHandler.ActivateChanges();
        CheckRoomCompletion();
        Animated = true;
    }
}
