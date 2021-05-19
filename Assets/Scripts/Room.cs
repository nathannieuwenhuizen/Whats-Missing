using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<RoomTelevision> allTelevisions;
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
    private GameObject particlePrefab;
    [SerializeField]
    private GameObject plopParticle;

    [SerializeField]
    private Door door;

    private bool animated;
    public bool Animated {
        set { 
            animated = value;
            foreach (IChangable obj in allObjects) obj.animated = value;
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
        Animated = false;
        
        foreach (RoomTelevision tv in allTelevisions)
        {
            if (tv.Word != "") {
                if (tv.isQuestion) CheckQuestion(tv);
                else AddTVChange(tv);
            }
        }
        Animated = true;
    }

    //Deactivate all changes 
    public void DeactivateChanges(){
        Animated = false;
        
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            RemoveChange(changes[i]);
        }
        Animated = true;
    }

    // Apply the change to the object 
    public void AddTVChange(RoomTelevision selectedTelevision) {
        bool hasChangedSomething = false;
        Change newChange = new Change(){word = selectedTelevision.Word, television = selectedTelevision};
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == selectedTelevision.Word) {
                if (obj.animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(selectedTelevision, obj, 1f, () => {
                        obj.SetChange(newChange);
                    }));
                } else obj.SetChange(newChange);

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

    
    //removes a tv change updating the room and tv
    public void RemoveTVChange(RoomTelevision selectedTelevision) {
        if (!selectedTelevision.IsOn) return;
        selectedTelevision.IsOn = false;
        CheckRoomCompletion();
        if (!selectedTelevision.isQuestion) RemoveChange(changes.Find(x => x.television == selectedTelevision));
    }

    //removes a change to the room updating the objects
    private void RemoveChange(Change change) {
        if (change == null) return;
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == change.word) {
                if (obj.animated && obj.Transform.GetComponent<Property>() == null) {
                    StartCoroutine(AnimateChangeEffect(change.television, obj, 1f, () => {
                        obj.RemoveChange(change);
                    }));
                } else obj.RemoveChange(change);
            }
        }
        changes.Remove(change);
    }

    public IEnumerator AnimateChangeEffect(RoomTelevision tv, IChangable o, float duration, Action callback) {
        float index = 0;
        Vector3 begin = tv.transform.position;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        GameObject particle = Instantiate(particlePrefab, begin, Quaternion.identity);
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            particle.transform.position = Vector3.LerpUnclamped(begin, o.Transform.position, curve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        GameObject plop = Instantiate(plopParticle, o.Transform.position, Quaternion.identity);
        callback();
    }

    

    public void CheckQuestion(RoomTelevision selectedTelevision) {
        bool questionIsCorrect = false;

        foreach (RoomTelevision tv in allTelevisions)
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
        foreach (RoomTelevision tv in allTelevisions)
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
