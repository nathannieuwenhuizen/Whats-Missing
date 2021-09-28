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
/// Cordinate of a pickable room object.
///</summary>
[System.Serializable]
public struct PickableRoomObjectCordinates
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
}