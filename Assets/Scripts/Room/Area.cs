using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    public delegate void UndoActionEvent(Room _room);
    public static UndoActionEvent OnUndo;
    public delegate void RoomEvent();
    public static RoomEvent OnNewRoomEnter;
    public static RoomEvent OnFirstRoomEnter;
    public static RoomEvent OnRespawn;

    [SerializeField]
    private RoomDirectionalLight directionalLight;

    [SerializeField]
    private RoomLevel[] roomLevels;

    private List<Room> rooms = new List<Room>();
    public List<Room> Rooms {
        get => rooms;
    }
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
            int oldindex = rooms.IndexOf(currentRoom);
            currentRoom = value;

            if (oldindex < rooms.IndexOf(value)) {
                UpdateRoomActiveStates();
            } else {
                UpdateRoomActiveStates(true);
            }

            currentRoom.OnRoomEnter(player, loadRoomState);
            directionalLight.RotateToMatchRoon(currentRoom.transform);

            if (rooms.IndexOf(currentRoom) == 0) {
                OnFirstRoomEnter?.Invoke();
            }
            loadRoomState = false;
        }
    }

    private void UpdateRoomActiveStates(bool includingNextRoom = false) {
        int currentIndex = rooms.IndexOf(currentRoom);
        for (int i = 0; i < rooms.Count; i++) {
            if (i <= currentIndex + (includingNextRoom ? 1 : 0) && i >= currentIndex - 1) {
                rooms[i].gameObject.SetActive(true);
            } else {
                rooms[i].gameObject.SetActive(false);
            }
            if (i < rooms.IndexOf(currentRoom)) {
                if (rooms[i].gameObject.activeSelf)
                    rooms[i].EndDoor.Locked = false;
            }
        }
    }

    private void Awake() {
        InitializeRooms();
        LoadProgress();
    }
    
    void Start()
    {
        AudioHandler.Instance.AudioListenerVolume = 1;
        AudioHandler.Instance.PlayMusic(MusicFiles.gallery, 1f);
        player.transform.position = rooms[loadRoomIndex].StartDoor.EndPos();
        player.transform.rotation = rooms[loadRoomIndex].StartDoor.transform.rotation;
        player.transform.Rotate(0,180,0);// = rooms[startingRoomIndex].StartDoor.transform.rotation;
        //playerPos = rooms[startingRoomIndex].StartPos.position;
        directionalLight.animating = false;
        CurrentRoom = rooms[loadRoomIndex];
        directionalLight.animating = true;

    }

    ///<summary>
    /// Loads and initializes all rooms from the inspector values.
    ///</summary>
    private void InitializeRooms() {
        Vector3 pos = Vector3.zero;
        Transform origin = transform;
        
        foreach (RoomLevel roomLevel in roomLevels) {
            //make new room
            Room newRoom = Instantiate(roomLevel.prefab.gameObject, transform).GetComponent<Room>();
            newRoom.name = "Room: " + (roomLevel.roomInfo != null ? roomLevel.name : roomLevel.prefab.name);
            newRoom.Area = this;
            int changeMirrorIndex = 0;
            if (roomLevel.roomInfo != null) {
                newRoom.RevealChangeAfterCompletion = roomLevel.roomInfo.revealChangesAfterFinish;
                foreach (RoomTelevision tv in newRoom.GetAllObjectsInRoom<RoomTelevision>())
                {
                    if (tv.isQuestion) {
                        tv.Letters = roomLevel.roomInfo.questionMirror.letters;
                        tv.changeType = roomLevel.roomInfo.questionMirror.changeType;
                        tv.roomIndexoffset = roomLevel.roomInfo.questionMirror.roomIndexoffset;
                    } else {
                        tv.PreAnswer = roomLevel.roomInfo.changeMirror[changeMirrorIndex].letters;
                        tv.changeType = roomLevel.roomInfo.changeMirror[changeMirrorIndex].changeType;
                        tv.roomIndexoffset = roomLevel.roomInfo.changeMirror[changeMirrorIndex].roomIndexoffset;
                        changeMirrorIndex++;
                    }
                }
            }
            newRoom.LoadTVs();

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


    //Todo: Probably move this to the door class.
    ///<summary>
    /// Animates the player walking thuogh a door
    ///</summary>
    public IEnumerator Walking(Vector3 endPos, float duration, float delay = 0) {
        float index = 0;
        player.Movement.EnableWalk = false;
        Vector3 begin = playerPos;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            playerPos = Vector3.LerpUnclamped(begin, endPos, walkingCurve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        player.Movement.EnableWalk = true;
        playerPos = endPos;
    }
 

    ///<summary>
    /// Fires when the palyer dies and has to respawn
    ///</summary>
    public void ResetPlayer() {
        StartCoroutine(ResettingThePlayer());
    }
    private IEnumerator ResettingThePlayer() {
        yield return new WaitForSeconds(2);

        int index = rooms.IndexOf(currentRoom);
        if(index == 0) {
            CurrentRoom = rooms[0];
            player.transform.position = CurrentRoom.StartDoor.EndPos();
        } else {
            CurrentRoom = rooms[index - 1];
            player.transform.position = CurrentRoom.EndDoor.StartPos();
        }
        player.Respawn();
        OnRespawn?.Invoke();
    }

    private void OnEnable() {
        Door.OnPassingThrough += OnPassingThroughDoor;
        Door.OnDoorOpen += ShowNextRoom;
        InputManager.OnUndo += UndoAction;
        Player.OnDie += ResetPlayer;
        InputManager.OnReset += ResetRoom;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
        Door.OnDoorOpen -= ShowNextRoom;
        InputManager.OnUndo -= UndoAction;
        Player.OnDie -= ResetPlayer;
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

    private void ShowNextRoom(Door door) {
        UpdateRoomActiveStates(true);
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
