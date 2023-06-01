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

    private bool active = false;
    public bool Active {
        get {
            return active;
        } set {
            active = value;
            startRotation = roomObject.transform.rotation;
        }
    }
    private bool hasBeenFlipped = false;

    private Quaternion startRotation;

    public RoomObjectEventSender(RoomObject _roomObject) {
        roomObject = _roomObject;
    }
    
    public void Update() {
        if (!Active) return;

        //note: otherwise table throw from bridge while upside-down won't result in level complete.
        // CheckFlipState();
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
        if (roomObject.Word == "book") {
            // Debug.Log("start rotation | " + startRotation);
        }
        // return Quaternion.Angle(roomObject.transform.rotation, startRotation) > 90f;
        return Vector3.Dot(roomObject.transform.up, Vector3.down) > 0;
    }

    public void SendFlipEvent() {
        if (Active) OnAltered?.Invoke(roomObject, ChangeType.flipped, true);
    }
    public void SendUnflipEvent() {
        if (Active) OnAltered?.Invoke(roomObject, ChangeType.flipped, false);
    }
    public void SendMissingEvent() {
        if (Active) OnAltered?.Invoke(roomObject, ChangeType.missing, true);
    }
    public void SendAppearingEvent() {
        if (Active) OnAltered?.Invoke(roomObject, ChangeType.missing, false);
    }
}
