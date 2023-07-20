using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

///<summary>
/// The room loads/handles the mirror states and is responsible when the room is completed or not.
///</summary>
public class Room : MonoBehaviour
{

    public delegate void RoomAction();
    public static event RoomAction OnRoomComplete;
    public static event RoomAction OnRoomLeaving;
    public static event RoomAction OnRoomEntering;

    public RoomLevel _roomLevel;
    public RoomLevel roomLevel {
        get { return _roomLevel;}
        set { _roomLevel = value; }
    }

    [HideInInspector]
    public int LoadIndex = 0;

    protected HintStopwatch hintStopwatch;

    private Area area;
    public Area Area {
        get { return area;}
        set { area = value; }
    }
    private ChangeHandler changeHandler;
    public ChangeHandler ChangeHandler {
        get { return changeHandler;}
    }
    private RoomStateHandler roomstateHandler;

    private SaveData beginState;

    public delegate void MakeRoomAction(Room _room, Change _change, bool _changeIsAdded, string previousWord = "");
    public static MakeRoomAction OnMakeRoomAction;

    ///<summary>
    /// Timescale on which the room opperates. from 1 (normal speed) to 0 (time stops)
    ///</summary>
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

    private bool isRoomFinished = false;
    private bool revealChangeAfterCompletion = true;
    public bool RevealChangeAfterCompletion {
        get { return revealChangeAfterCompletion;}
        set { revealChangeAfterCompletion = value; }
    }

    [HideInInspector]
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

    
    private GameObject changeLineObject;

    private GameObject plopParticle;

    //doors
    [SerializeField]
    private Door startDoor;
    public Door StartDoor {
        get { return startDoor;}
    }
    [SerializeField]
    private Door endDoor;
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
    private bool changeLineAnimated = true;
    public bool ChangeLineAnimated {
        get => changeLineAnimated;
        set => changeLineAnimated = value;
    }

    protected virtual void Awake() {
        hintStopwatch = new HintStopwatch(this);
        changeHandler = new ChangeHandler(this);
        roomstateHandler = new RoomStateHandler(this); 

        changeLineObject = Resources.Load<GameObject>("RoomPrefabs/change_line");
        plopParticle = Resources.Load<GameObject>("RoomPrefabs/plop_effect");

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
    private void Start() {
        endDoor.room = this;
        startDoor.room = this;
        Debug.Log("door room = " + endDoor.room + startDoor.room);
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
    public IEnumerator AnimateChangeEffect(float delay, IChangable o, float duration, Action callback) {
        yield return new WaitForSecondsRealtime(delay);
        ChangeLine changeLine = Instantiate(changeLineObject).GetComponent<ChangeLine>();
        // if (roomLevel.roomInfo.)
        changeLine.SetDestination(animationStartPos(), o.Transform.position);
        yield return changeLine.MoveTowardsDestination();

        GameObject plop = Instantiate(plopParticle, o.Transform.position, Quaternion.identity);
        Destroy(plop, 5f);
        callback();
    }

    ///<summary>
    /// Checks in all object if it has the same word as the change. If so it will send a callback with the Ichangable. Also returns true if any objects has the word.
    ///</summary>
    public bool ForEachObjectWithMirrorWord(IChange change, Action<IChangable> callBack) {
        bool result = false;
        float delay = 0;
        float totalTime = 1f;
        List<IChangable> foundObjects = new List<IChangable>();

        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.Word || obj.AlternativeWords.Contains(change.Word) && obj.Transform.gameObject != null) {
                foundObjects.Add(obj);
                result = true;
            }
        }

        foreach (IChangable obj in foundObjects)
        {
            if (obj.Animated && obj.Transform.GetComponent<Property>() == null && changeLineAnimated) {
                    StartCoroutine(AnimateChangeEffect(delay, obj, 1f, () => {
                        callBack(obj);
                    }));
                    delay += (totalTime / (float)foundObjects.Count);
            } else callBack(obj);
        }
        return result;
    }

    ///<summary>
    /// Where the changeline should spawn from
    ///</summary>
    public Vector3 animationStartPos() {
        if (mirrors.Count > 0) {
            return mirrors[0].transform.position;
        } else {
             return Vector3.zero;
        }
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
    public void RemoveChangeInRoomObjects(IChange change) {
        ForEachObjectWithMirrorWord(change, (IChangable obj) => { obj.RemoveChange(change); });  
    }

    ///<summary>
    /// adds the change to the room updating the objects
    ///</summary>
    public void AddChangeInRoomObjects(IChange change) {
        ForEachObjectWithMirrorWord(change, (IChangable obj) => { obj.AddChange(change); });  
    }

    
    ///<summary>
    /// Checks if a mirror question is correct with the changes that exist inside the room.
    ///</summary>
    public void CheckMirrorQuestion(Mirror selectedMirror) {
        Debug.Log("check is on: " + selectedMirror.Letters.Length);
        selectedMirror.IsOn = (changeHandler.ContainsWord(selectedMirror) || selectedMirror.Letters.Length == 0);
        CheckRoomCompletion();
    }


    ///<summary>
    /// Handles if the door should be open or not. 
    ///</summary>
    public virtual void CheckRoomCompletion() {
        if (AllMirrorsAreOn()) {
            if (!isRoomFinished) {
                isRoomFinished = true;
                hintStopwatch.Pause();
                OnRoomComplete?.Invoke();
                StartCoroutine(WaitBeforeOpeningDoor());
                if (revealChangeAfterCompletion) {
                    changeHandler.DeactivateChanges(false);
                }
            }
        } else {
            if (isRoomFinished) {
                isRoomFinished = false;
                endDoor.Locked = true;
                if (revealChangeAfterCompletion) {
                    changeHandler.ActivateChanges();
                }
            }
        }
    }
    private IEnumerator WaitBeforeOpeningDoor() {
        yield return new WaitForSeconds(2f);
        if (area != null) {
            if (AllMirrorsAreOn() && !area.IsLastRoom(this)){
                roomFinishedEvent?.Invoke();
                endDoor.Locked = false;
            }
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
    public virtual void OnRoomEnter(Player _player, bool loadSaveData = false) {
        player = _player;
        player.transform.parent = transform;
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
        AllObjects.Add(player);

        InArea = true;

        OnRoomEntering?.Invoke();

        hintStopwatch.Resume();
        changeLineAnimated = false;
        if (roomLevel != null) Animated = roomLevel.roomInfo.alwaysAnimate ? true : false;

        if (loadSaveData) {
            roomstateHandler.LoadState(SaveData.current);
        }

        foreach (IRoomObject item in GetAllObjectsInRoom<IRoomObject>())
        {
            item.InSpace = true;
            item.OnRoomEnter();
            RoomObject roomObject = item as RoomObject;
            if (roomObject != null) if (roomObject.EventSender != null) if (roomLevel != null) roomObject.EventSender.Active = roomLevel.roomInfo.EventSenderActive;
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
                if (!AllMirrorsAreOn())
                    changeHandler.ActivateChanges();
            } else 
                changeHandler.ActivateChanges();
        }


        beginState = SaveData.GetStateOfRoom(this);
        
        StartCoroutine(DelayActivatingAnimationOmEntering());
        changeLineAnimated = true;
        Animated = true;
        roomEnterEvent?.Invoke();
    }
    public IEnumerator DelayActivatingAnimationOmEntering() {
        yield return new WaitForSeconds(1f);

    }

    public virtual void ShowMirrorToggleHint() {
        if (roomLevel.roomInfo.hintText == "") return;
        foreach(Mirror mirror in mirrors) {
            if (mirror.isQuestion) {
                mirror.MirrorCanvas.ShowHintButton(roomLevel.roomInfo.hintText, roomLevel.roomInfo.durationBeforeHighlighting);
                break;
            }
        }
    }
    public virtual void ShowMirrorToggleSecondHint() {
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
            //disable al movement in rigidbodies except player.
            if (rb != player.Movement.RB) rb.sleepThreshold = Mathf.Infinity;
        }


        InArea = false;

        Animated = false;
        changeHandler.DeactivateChanges();
        hintStopwatch.Pause();
        OnRoomLeaving?.Invoke();
        // AllObjects.Remove(Player);
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
                mirror.IsOn = changeHandler.ContainsWord(mirror);
            }
        }
    }

    private void OnEnable() {
        RoomObjectEventSender.OnAltered += UpdateRoomObjectChanges;
        Potion.OnChanging += AddPotionChange;
        if (hintStopwatch != null) hintStopwatch.OnEnable();
    }

    private void OnDisable() {
        RoomObjectEventSender.OnAltered -= UpdateRoomObjectChanges;
        Potion.OnChanging -= AddPotionChange;
        if (hintStopwatch != null) hintStopwatch.OnDisable();

    }

    ///<summary>
    /// Fired when an room object event sender fired an onaltered. This automaticly checks the mirror if it is now correct.
    ///</summary>
    public void UpdateRoomObjectChanges(RoomObject _roomObject, ChangeType _changeType, bool _enabled) {
        if (!InArea) return;
        changeHandler.UpdateRoomObjectChanges(_roomObject, _changeType, _enabled);
        Debug.Log(" room object " + _roomObject.Word +  " is now " + _changeType + "  | " + _enabled);
        UpdateMirrorStates();
        CheckRoomCompletion();
    }

    ///<summary>
    /// Fired when a potion hitted a room object and fires an event to update the mirror and the room completion state.
    ///</summary>
    public void AddPotionChange(Potion potion, IChangable changable) {
        if (!InArea) return;
        changeHandler.AddPotionChange(potion, changable);
        UpdateMirrorStates();
        CheckRoomCompletion();
    }
    public void OnPlayerDie() {
        changeHandler.OnPlayerDie(player);
        UpdateMirrorStates();
        CheckRoomCompletion();
    }

    ///<summary>
    ///[DEPRECATED]
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
