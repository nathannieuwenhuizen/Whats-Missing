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

    public static string FILE_NAME = "test2";
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
    public int areaIndex = 0;
    public int roomIndex = 0;

    public static SaveData GetStateOfRoom(Room _room) {
        SaveData newState = new SaveData();
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
