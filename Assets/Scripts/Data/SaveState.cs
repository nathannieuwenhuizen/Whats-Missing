using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

///<summary>
/// State of the room. Used for saving the game.
///</summary>
[System.Serializable]
public struct SaveState
{
    public int roomIndex;
    public TVState[] tvStates;
    public PickableRoomObjectCordinates[] cordinates;
    public PickableRoomObjectCordinates playerCordinates;

    public void Save(Room _room) {
        
    }

    public static SaveState LoadSaveData() {
        return new SaveState();
    }
}

[System.Serializable]
public struct  TVState
{
    public RoomTelevision television;
    public bool isOn;
    public string word;
}
