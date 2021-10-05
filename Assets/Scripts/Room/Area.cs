using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    public delegate void UndoActionEvent(Room _room);
    public static UndoActionEvent OnUndo;
    public delegate void NewRoomEvent();
    public static NewRoomEvent OnNewRoomEnter;

    public static NewRoomEvent OnFirstRoomEnter;

    [SerializeField]
    private Room[] roomPrefabs;

    private List<Room> rooms = new List<Room>();
    [SerializeField]
    private Player player;

    [SerializeField]
    private AnimationCurve walkingCurve;

    [SerializeField]
    private int loadRoomIndex = 0;

    [SerializeField]
    private UnityEvent areaFinishedEvent;

    private bool loadRoomState = false;


    private Room currentRoom;
    public Room CurrentRoom {
        get { return currentRoom; }
        set {
            if (currentRoom != null) {
                currentRoom?.OnRoomLeave();
            }
            currentRoom = value;
            UpdateRoomActiveStates();
            currentRoom.OnRoomEnter(player, loadRoomState);
            if (rooms.IndexOf(currentRoom) == 0) {
                OnFirstRoomEnter?.Invoke();
            }
            loadRoomState = false;
        }
    }

    private void UpdateRoomActiveStates() {
        int currentIndex = rooms.IndexOf(currentRoom);
        for (int i = 0; i < rooms.Count; i++) {
            if (i < rooms.IndexOf(currentRoom)) {
                rooms[i].EndDoor.Locked = false;
            }
            if (i <= currentIndex + 1 && i >= currentIndex - 1) {
                rooms[i].gameObject.SetActive(true);
            } else {
                rooms[i].gameObject.SetActive(false);
            }
        }
    }

    private void Awake() {
        InitializeAllRooms();
        LoadProgress();
    }
    
    void Start()
    {
        AudioHandler.Instance.PlayMusic(MusicFiles.gallery, 1f);
        player.transform.position = rooms[loadRoomIndex].StartDoor.EndPos();
        player.transform.rotation = rooms[loadRoomIndex].StartDoor.transform.rotation;
        player.transform.Rotate(0,180,0);// = rooms[startingRoomIndex].StartDoor.transform.rotation;
        //playerPos = rooms[startingRoomIndex].StartPos.position;
        CurrentRoom = rooms[loadRoomIndex];
    }

    private void InitializeAllRooms() {
        Vector3 pos = Vector3.zero;
        Transform origin = transform;
        
        foreach (Room prefab in roomPrefabs) {
            //make new room
            Room newRoom = Instantiate(prefab.gameObject, transform).GetComponent<Room>();
            newRoom.name = "Room #" + (rooms.Count + 1);

            //position room
            newRoom.transform.rotation = origin.rotation; 
            newRoom.transform.Rotate(new Vector3(0,-90,0));
            newRoom.transform.position = origin.position + (newRoom.transform.position - newRoom.StartDoor.transform.position);
            origin = newRoom.EndDoor.transform;

            //deactivate the startdoor
            newRoom.StartDoor.gameObject.SetActive(rooms.Count == 0);

            //add to the list
            rooms.Add(newRoom);
        }
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
            index += Time.unscaledDeltaTime;
            playerPos = Vector3.LerpUnclamped(begin, endPos, walkingCurve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        playerPos = endPos;
    }
 

    private void OnEnable() {
        Door.OnPassingThrough += OnPassingThroughDoor;
        InputManager.OnUndo += UndoAction;
        InputManager.OnReset += ResetRoom;

    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
        InputManager.OnUndo -= UndoAction;
        InputManager.OnReset -= ResetRoom;
    }

    public void ResetRoom() {
        CurrentRoom.ResetRoom();
    }

    public void SaveProgress() {
        SaveData.current.roomIndex = rooms.IndexOf(currentRoom);
        SaveData.current = SaveData.GetStateOfRoom(currentRoom);
        SerializationManager.Save("test", SaveData.current);
    }

    private void OnDestroy() {
        SaveProgress();
    }

    public void LoadProgress() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/test.save");
        if (data != null) {

            SaveData.current = data as SaveData;
            //startingRoomIndex = SaveData.current.roomIndex;
            //loadRoomState = true;
        }
    }

    private void UndoAction() {
        OnUndo?.Invoke(currentRoom);
    }

    private void OnPassingThroughDoor(Door door) {
        float duration = 1.5f;
        float delay = 0f;
        OnNewRoomEnter?.Invoke();
        int index = rooms.IndexOf(door.room); // System.Array.IndexOf(roomPrefabs, door.room);
        if (door.room == currentRoom) {
            //next room
            if (index == rooms.Count - 1) {
                areaFinishedEvent?.Invoke();
                Debug.Log("area finished!");
            } else {
                CurrentRoom = rooms[index + 1];
                StartCoroutine(Walking(CurrentRoom.StartDoor.EndPos(), duration, delay));
            }
        } else {
            //previous room
            CurrentRoom = rooms[index];
            StartCoroutine(Walking(CurrentRoom.EndDoor.StartPos(), duration, delay));
        }
    }

}
