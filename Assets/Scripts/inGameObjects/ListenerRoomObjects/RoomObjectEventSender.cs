using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Is an active listener to any changes that happens to the room object and sends the event to the mirror if anything happens
///</summary>
public class RoomObjectEventSender
{
    private RoomObject roomObject;
    public delegate void OnAlteration(RoomObject roomObject, ChangeType changeType, bool enabled);
    public static OnAlteration OnAltered;

    public bool Active {get; set;} = false;
    private bool hasBeenFlipped = false;

    public RoomObjectEventSender(RoomObject _roomObject) {
        roomObject = _roomObject;
    }
    
    public void Update() {
        CheckFlipState();
    }
    public void CheckFlipState() {
        if (IsFlipped()) {
            if (!hasBeenFlipped) {
                hasBeenFlipped = true;
                SendFlipEvent();
            }
        } else {
            if (hasBeenFlipped) {
                hasBeenFlipped = false;
                SendUnflipEvent();
            }
        }
    }
    private bool IsFlipped() {
        return Vector3.Dot(roomObject.transform.up, Vector3.down) > 0;
    }

    public void SendFlipEvent() {
        OnAltered?.Invoke(roomObject, ChangeType.flipped, true);
    }
    public void SendUnflipEvent() {
        OnAltered?.Invoke(roomObject, ChangeType.flipped, false);
    }
    public void SendMissingEvent() {
        OnAltered?.Invoke(roomObject, ChangeType.missing, true);
    }
    public void SendAppearingEvent() {
        OnAltered?.Invoke(roomObject, ChangeType.flipped, false);
    }
}
