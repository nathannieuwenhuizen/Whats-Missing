using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

///<summary>
/// Save data of the playthrough. Used for saving the game.
///</summary>
[System.Serializable]
public class SaveData
{

    public static string FILE_NAME = "test";
    private static SaveData _current;
    public static SaveData current {
        get { 
            if (_current == null) {
                _current = new SaveData();
            }
            return _current;
        }
        set { _current = value; }
    }

    public int roomIndex = 0;
    public TVState[] tvStates;
    public PickableRoomObjectCordinates[] cordinates;
    public PickableRoomObjectCordinates playerCordinates;

    public static SaveData GetStateOfRoom(Room _room) {
        SaveData newState = new SaveData();
        
        newState.tvStates = UndoHandeler.GetAllTVStates(_room);
        newState.cordinates = UndoHandeler.GetAllPickableRoomCordinates(_room);
        PickableRoomObjectCordinates _playerCordinates = new PickableRoomObjectCordinates() {
            position = _room.Player.transform.position,
            rotation = _room.Player.transform.rotation,
        };
        newState.playerCordinates = _playerCordinates;
        return newState;
    }
}

[System.Serializable]
public struct  TVState
{
    public int id;
    public bool isOn;
    public string word;
}
