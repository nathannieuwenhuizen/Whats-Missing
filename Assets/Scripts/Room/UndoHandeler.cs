using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoHandeler : MonoBehaviour
{
    
    private List<RoomAction> actions = new List<RoomAction>();

    public void AddAction(Room _room, Change _change, bool _changeIsAdded, string _previousWord = "") {
        PickableRoomObjectCordinates _playerCordinates = new PickableRoomObjectCordinates() {
                position = _room.Player.transform.position,
                rotation = _room.Player.transform.rotation,
                id = _room.Player.gameObject.GetInstanceID()
            };
        RoomAction newAction = new RoomAction() {
            change = _change,
            changeIsAdded = _changeIsAdded,
            cordinates = UndoHandeler.GetAllPickableRoomCordinates(_room),
            playerCordinates = _playerCordinates,
            previousWord = _previousWord
        };
        actions.Add(newAction);
        Debug.Log("action added: " + newAction.change.television);
    }

    public static PickableRoomObjectCordinates[] GetAllPickableRoomCordinates(Room room) {
        List<PickableRoomObjectCordinates> result = new List<PickableRoomObjectCordinates>();
        foreach (PickableRoomObject item in room.GetAllObjectsInRoom<PickableRoomObject>())
        {
            PickableRoomObjectCordinates cordinates = new PickableRoomObjectCordinates() {
                position = item.transform.position,
                rotation = item.transform.rotation,
                id = item.id
            };
            result.Add(cordinates);
        }
        return result.ToArray();
    }
    public static TVState[] GetAllTVStates(Room room) {
        List<TVState> result = new List<TVState>();
        foreach (RoomTelevision item in room.AllTelevisions)
        {
            TVState cordinates = new TVState() {
                id = item.id,
                isOn = item.IsOn,
                word = item.Word
            };
            result.Add(cordinates);
        }
        return result.ToArray();
    }

    public void UndoAction(Room room) {
        if (actions.Count == 0) return;

        RoomAction currentAction = actions[actions.Count - 1];


        room.Animated = false;
        if (currentAction.change.television.isQuestion) {
            currentAction.change.television.DeselectLetters();
            currentAction.change.television.Word = currentAction.previousWord;
            room.CheckTVQuestion(currentAction.change.television, false);
        } else {
            if (currentAction.changeIsAdded) {
                room.RemoveTVChange(currentAction.change.television, false);
            } else {
                currentAction.change.television.DeselectLetters();
                currentAction.change.television.Word = currentAction.change.word;
                room.AddTVChange(currentAction.change.television, false);
            }
        }
        room.Animated = true;

        foreach (PickableRoomObject item in room.GetAllObjectsInRoom<PickableRoomObject>())
        {
            PickableRoomObjectCordinates cordinate = new List<PickableRoomObjectCordinates>(currentAction.cordinates).Find(x => x.id == item.id);
            item.transform.position = cordinate.position;
            item.transform.rotation = cordinate.rotation;
        }
        room.Player.transform.position = currentAction.playerCordinates.position;
        room.Player.transform.rotation = currentAction.playerCordinates.rotation;

        actions.Remove(currentAction);
        Debug.Log("action removed");
    }

    public void ResetActions() {
        actions = new List<RoomAction>();
    }

    private void OnEnable() {
        Room.OnMakeRoomAction += AddAction;
        Area.OnNewRoomEnter += ResetActions;
        Area.OnUndo += UndoAction;
    }

    private void OnDisable() {
        Room.OnMakeRoomAction -= AddAction;
        Area.OnNewRoomEnter -= ResetActions;
        Area.OnUndo -= UndoAction;
    }
}
