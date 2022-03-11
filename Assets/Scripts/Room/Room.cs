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
    public static event RoomAction OnRoomEntering;

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

    private bool inArea = false;
    public bool InArea {
        get { return inArea;}
        set { inArea = value; }
    }

    private string secondHintAnswer = "";
    public string SecondHintAnswer {
        get { return secondHintAnswer;}
        set { secondHintAnswer = value; }
    }

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
        foreach(Rigidbody rb in GetAllObjectsInRoom<Rigidbody>()) {
            rb.sleepThreshold = Mathf.Infinity;
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
    public bool DoesObjectWordMatch(MirrorChange change, Action<IChangable> callBack) {
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
        Debug.Log(change.changeType + " | " + change.mirror.name);


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
    public void AddMirrorChange(Mirror selectedMirror) {
        MirrorChange newChange = changeHandler.CreateChange(selectedMirror);

        if (newChange != null) {
            AddChangeInRoomObjects(newChange);
            changeHandler.MirrorChanges.Add(newChange);
            selectedMirror.IsOn = true;
            CheckRoomCompletion();

        } else {
            selectedMirror.IsOn = false;
        }
    }

    
    /// <summary> 
    ///removes a mirror change updating the room and mirror
    ///</summary>
    public void RemoveMirrorChange(Mirror selectedMirror) {
        if (!selectedMirror.IsOn) return;
        selectedMirror.IsOn = false;
        CheckRoomCompletion();
        MirrorChange removedChange = changeHandler.MirrorChanges.Find(x => x.mirror == selectedMirror);
        if (!selectedMirror.isQuestion) {
            RemoveChangeInRoomObjects(removedChange);
            changeHandler.MirrorChanges.Remove(removedChange);
        }

    }

    ///<summary>
    /// removes a change to the room updating the objects
    ///</summary>
    public void RemoveChangeInRoomObjects(MirrorChange change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.RemoveChange(change); });  
    }

    ///<summary>
    /// adds the change to the room updating the objects
    ///</summary>
    public void AddChangeInRoomObjects(MirrorChange change) {
        DoesObjectWordMatch(change, (IChangable obj) => { obj.AddChange(change); });  
    }
    
    ///<summary>
    /// Checks if a mirror question is correct with the changes that exist inside the room.
    ///</summary>
    public void CheckMirrorQuestion(Mirror selectedMirror) {
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
    public void CheckRoomCompletion() {
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
        if (AllMirrorsAreOn() && !area.IsLastRoom(this)){
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
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
        AllObjects.Add(player);

        InArea = true;

        OnRoomEntering?.Invoke();

        hintStopwatch.Resume();
        Animated = false;

        if (loadSaveData) {
            roomstateHandler.LoadState(SaveData.current);
        }

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.InSpace = true;
            item.OnRoomEnter();
            RoomObject roomObject = item as RoomObject;
            if (roomObject != null) roomObject.EventSender.Active = !revealChangeAfterCompletion;
        }
        foreach(Rigidbody rb in GetAllObjectsInRoom<Rigidbody>()) {
            rb.sleepThreshold = 0.14f;
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
                mirror.MirrorCanvas.ShowHintButton(roomLevel.roomInfo.hintText, roomLevel.roomInfo.durationBeforeHighlighting);
                break;
            }
        }
    }
    public void ShowMirrorToggleSecondHint() {
        foreach(Mirror mirror in mirrors) {
            if (mirror.isQuestion) {
                mirror.MirrorCanvas.ShowSecondHintButton(SecondHintAnswer);
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
            item.InSpace = false;
        }
        foreach(Rigidbody rb in GetAllObjectsInRoom<Rigidbody>()) {
            if (rb != player.Movement.RB) rb.sleepThreshold = Mathf.Infinity;
        }


        InArea = false;

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
                mirror.IsOn = changeHandler.MirrorChanges.Find(c => c.mirror == mirror) != null;
            } else {
                mirror.IsOn = changeHandler.WordMatchesChanges(mirror);
            }
        }
    }

    private void OnEnable() {
        RoomObjectEventSender.OnAltered += UpdateRoomObjectChanges;
        Potion.OnChanging += AddPotionChange;
    }

    private void OnDisable() {
        RoomObjectEventSender.OnAltered -= UpdateRoomObjectChanges;
        Potion.OnChanging -= AddPotionChange;
    }

    public void UpdateRoomObjectChanges(RoomObject _roomObject, ChangeType _changeType, bool _enabled) {
        if (!InArea) return;
        changeHandler.UpdateRoomObjectChanges(_roomObject, _changeType, _enabled);
        UpdateMirrorStates();
        CheckRoomCompletion();
    }
    public void AddPotionChange(Potion potion, IChangable changable) {
        if (!InArea) return;

        changeHandler.AddPotionChange(potion, changable);
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
