using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{

    [SerializeField]
    private Room[] rooms;
    [SerializeField]
    private Player player;

    [SerializeField]
    private Room currentRoom;
    public Room CurrentRoom {
        get { return currentRoom; }
        set {
            if (currentRoom != null) {
                currentRoom?.OnRoomLeave();
                currentRoom.AllObjects.Remove(player);
                currentRoom.Player = null;
            }
            currentRoom = value;
            player.transform.position = currentRoom.getStartPos.position;
            player.transform.parent = currentRoom.transform;
            currentRoom.AllObjects.Add(player);
            currentRoom.Player = player;
            currentRoom.OnRoomEnter();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentRoom = currentRoom;
    }

    private void OnEnable() {
        Door.OnPassingThrough += OnPassingThroughDoor;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
    }

    void OnPassingThroughDoor(Door door) {
        Debug.Log("door" + door);
        int index = System.Array.IndexOf(rooms, door.room);

        if (currentRoom == door.room) {
            if (index == rooms.Length - 1) {
                Debug.Log("area finished!");
            } else {
                CurrentRoom = rooms[index + 1];
            }
        } else {
            CurrentRoom = rooms[index];
        }
    }

}
