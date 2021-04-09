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
    private AnimationCurve walkingCurve;

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
            player.transform.parent = currentRoom.transform;
            currentRoom.AllObjects.Add(player);
            currentRoom.Player = player;
            currentRoom.OnRoomEnter();
        }
    }
    
    void Start()
    {
        playerPos = currentRoom.StartPos.position;
        CurrentRoom = currentRoom;
    }

    private Vector3 playerPos {
        get => player.transform.position;
        set {
            player.transform.position = new Vector3(value.x, player.transform.position.y, value.z);
        }
    }

    public IEnumerator Walking(Vector3 endPos, float duration, float delay = 0) {
        float index = 0;
        Vector3 begin = playerPos;
        while (index < duration) {
            index += Time.deltaTime;
            playerPos = Vector3.LerpUnclamped(begin, endPos, walkingCurve.Evaluate(index / duration));
            yield return new WaitForFixedUpdate();
        }
        playerPos = endPos;
    }
 

    private void OnEnable() {
        Door.OnPassingThrough += OnPassingThroughDoor;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
    }

    void OnPassingThroughDoor(Door door) {
        int index = System.Array.IndexOf(rooms, door.room);
        if (door.room == currentRoom) {
            //next room
            if (index == rooms.Length - 1) {
                Debug.Log("area finished!");
            } else {
                CurrentRoom = rooms[index + 1];
                StartCoroutine(Walking(CurrentRoom.StartPos.position, .5f, .2f));
            }
        } else {
            //previous room
            CurrentRoom = rooms[index];
            StartCoroutine(Walking(CurrentRoom.EndPos.position, .5f, .2f));
        }
    }

}
