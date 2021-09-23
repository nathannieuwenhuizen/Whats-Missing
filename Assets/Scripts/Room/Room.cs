using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
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
        allTelevisions = GetAllObjectsInRoom<RoomTelevision>().OrderBy( allObjects => !allObjects.isQuestion).ToList(); 
        foreach(RoomTelevision tv in allTelevisions) tv.Room = this;
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
                if (tv.isQuestion) CheckQuestion(tv);
                else AddTVChange(tv);
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

    // Apply the change to the object 
    public void AddTVChange(RoomTelevision selectedTelevision) {
        bool hasChangedSomething = false;
        float delay = 0;
        Change newChange = new Change(){word = selectedTelevision.Word, television = selectedTelevision};
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == selectedTelevision.Word || obj.AlternativeWords.Contains(selectedTelevision.Word)) {
                if (obj.Animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(delay, selectedTelevision, obj, 1f, () => {
                        obj.SetChange(newChange);
                    }));
                    delay += .5f;
                } else obj.SetChange(newChange);
                newChange.AlternativeWords = obj.AlternativeWords;
                hasChangedSomething = true;
            }
        }
        if (hasChangedSomething) {
            changes.Add(newChange);
            selectedTelevision.IsOn = true;
            CheckRoomCompletion();
        } else {
            selectedTelevision.IsOn = false;
        }
    }

    
    /// <summary> 
    ///removes a tv change updating the room and tv
    ///</summary>
    public void RemoveTVChange(RoomTelevision selectedTelevision) {
        if (!selectedTelevision.IsOn) return;
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
        if (!selectedTelevision.isQuestion) RemoveChange(changes.Find(x => x.television == selectedTelevision));
    }

    //removes a change to the room updating the objects
    private void RemoveChange(Change change) {

        float delay = 0;
        if (change == null) return;
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.word ||  obj.AlternativeWords.Contains(change.word)) {
                if (obj.Animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(delay, change.television, obj, 1f, () => {
                        obj.RemoveChange(change);
                    }));
                    delay += .5f;
                } else obj.RemoveChange(change);
            }
        }
        changes.Remove(change);
    }

    public IEnumerator AnimateChangeEffect(float delay, RoomTelevision tv, IChangable o, float duration, Action callback) {
        yield return new WaitForSecondsRealtime(delay);
        ChangeLine changeLine = Instantiate(changeLineObject).GetComponent<ChangeLine>();
        changeLine.SetDestination(tv.transform.position, o.Transform.position);
        yield return changeLine.MoveTowardsDestination();


        // float index = 0;
        // Vector3 begin = tv.transform.position;
        // AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        // GameObject particle = Instantiate(particlePrefab, begin, Quaternion.identity);
        // while (index < duration) {
        //     index += Time.unscaledDeltaTime;
        //     particle.transform.position = Vector3.LerpUnclamped(begin, o.Transform.position, curve.Evaluate(index / duration));
        //     yield return new WaitForEndOfFrame();
        // }
        GameObject plop = Instantiate(plopParticle, o.Transform.position, Quaternion.identity);
        callback();
    }

    

    public void CheckQuestion(RoomTelevision selectedTelevision) {
        foreach (Change change in changes)
        {
            if (change.word == selectedTelevision.Word || change.AlternativeWords.Contains(selectedTelevision.Word)) {
                selectedTelevision.IsOn = true;
                CheckRoomCompletion();
                return;
            }
        }
        selectedTelevision.IsOn = false;
    }
    
    private void CheckRoomCompletion() {
        if (AllTelevisionsAreOn()) {
            door.Locked = false;
            roomFinishedEvent?.Invoke();
            if (revealChangeAfterCompletion) {
                DeactivateChanges();
            }
        } else if (door.Locked == false) {
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
        Debug.Log("player" + Player);
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
