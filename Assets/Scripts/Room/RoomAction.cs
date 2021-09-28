using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Action a player makes between changes. Can be used to undo the action
///</summary>
public struct RoomAction
{
    //TODO: replace change class with tvstate
    public Change change;
    public bool changeIsAdded;
    public string previousWord;
    public PickableRoomObjectCordinates[] cordinates;
    public PickableRoomObjectCordinates playerCordinates;
}

///<summary>
/// State of the room. Used for saving the game.
///</summary>
public struct SaveState
{
    public int roomIndex;
    public TVState[] tvStates;
    public PickableRoomObjectCordinates[] cordinates;
    public PickableRoomObjectCordinates playerCordinates;
}

public struct  TVState
{
    public RoomTelevision television;
    public bool isOn;
    public string word;
}


///<summary>
/// Cordinate of a pickable room object.
///</summary>
public struct PickableRoomObjectCordinates
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
}