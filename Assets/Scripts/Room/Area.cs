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

    [SerializeField]
    private Room[] roomPrefabs;

    private List<Room> rooms = new List<Room>();
    [SerializeField]
    private Player player;

    [SerializeField]
    private AnimationCurve walkingCurve;

    [SerializeField]
    private int startingRoomIndex = 0;

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
            loadRoomState = false;
        }
    }

    private void UpdateRoomActiveStates() {
        int currentIndex = rooms.IndexOf(currentRoom);
        for (int i = 0; i < rooms.Count; i++) {
            if (i == currentIndex || i == currentIndex - 1) {
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
        playerPos = rooms[startingRoomIndex].StartPos.position;
        CurrentRoom = rooms[startingRoomIndex];
    }

    private void InitializeAllRooms() {
        Vector3 pos = Vector3.zero;
        foreach (Room prefab in roomPrefabs) {
            Room newRoom = Instantiate(prefab.gameObject, transform).GetComponent<Room>();
            newRoom.name = "Room #" + (rooms.Count + 1);

            //position room
            newRoom.transform.position = pos + (newRoom.transform.position - newRoom.StartPos.position) + new Vector3(0,0,2.3f);
            pos = newRoom.EndPos.position;
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
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
        InputManager.OnUndo -= UndoAction;
    }

    public void SaveProgress() {
        SaveData.current.roomIndex = rooms.IndexOf(currentRoom);
        SaveData.current.Save(currentRoom);
        SerializationManager.Save("test", SaveData.current);
    }

    private void OnDestroy() {
        SaveProgress();
    }

    public void LoadProgress() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/test.save");
        Debug.Log("load pos"+ data);
        if (data != null) {

            SaveData.current = data as SaveData;
            startingRoomIndex = SaveData.current.roomIndex;
            loadRoomState = true;
        }
    }

    private void UndoAction() {
        OnUndo?.Invoke(currentRoom);
    }

    void OnPassingThroughDoor(Door door) {
        OnNewRoomEnter?.Invoke();
        int index = rooms.IndexOf(door.room); // System.Array.IndexOf(roomPrefabs, door.room);
        if (door.room == currentRoom) {
            //next room
            if (index == rooms.Count - 1) {
                areaFinishedEvent?.Invoke();
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
