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

    public int roomIndex;
    public TVState[] tvStates;
    public PickableRoomObjectCordinates[] cordinates;
    public PickableRoomObjectCordinates playerCordinates;

    public void Save(Room _room) {

        tvStates = UndoHandeler.GetAllTVStates(_room);
        cordinates = UndoHandeler.GetAllPickableRoomCordinates(_room);
        Debug.Log(cordinates[0].position + " | " + cordinates[0].id);
        PickableRoomObjectCordinates _playerCordinates = new PickableRoomObjectCordinates() {
            position = _room.Player.transform.position,
            rotation = _room.Player.transform.rotation,
        };
        playerCordinates = _playerCordinates;
    }
}

[System.Serializable]
public struct  TVState
{
    public int id;
    public bool isOn;
    public string word;
}
