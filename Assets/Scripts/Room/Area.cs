using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    [SerializeField]
    private Vector3 roomPositionOffset;
    [SerializeField]
    private float roomRotationOffset = 0;

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
    private int loadRoomIndex = 0;
    private int furthestCurrentRoomIndex;

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
            }
        }
        LockPreviousRoomDoors();
    }

    ///<summary>
    /// Locks all the doors of the previous rooms so that the player doesn't have to backtrack
    ///</summary>
    private void LockPreviousRoomDoors() {
        int currentIndex = rooms.IndexOf(currentRoom);
        for (int i = 0; i < rooms.Count; i++) {
            if (i < furthestCurrentRoomIndex - 1) {
                rooms[i].EndDoor.Animated = false;
                rooms[i].EndDoor.Locked = true;
            } else if (i >= (furthestCurrentRoomIndex - 1) && i < furthestCurrentRoomIndex) {
                if (rooms[i].gameObject.activeSelf)
                    rooms[i].EndDoor.Locked = false;
            }


        }
    }

    private void Awake() {
        LoadProgress();
        InitializeRooms();
    }
    
    void Start()
    {
        AudioHandler.Instance.PlayMusic(MusicFiles.planetarium, 1f);
        player.transform.position = rooms[loadRoomIndex].StartDoor.EndPos();
        player.transform.rotation = rooms[loadRoomIndex].StartDoor.transform.rotation;
        player.transform.Rotate(0,180,0);// = rooms[startingRoomIndex].StartDoor.transform.rotation;
        //playerPos = rooms[startingRoomIndex].StartPos.position;
        directionalLight.animating = false;
        CurrentRoom = rooms[loadRoomIndex];
        directionalLight.animating = true;
        if (loadRoomIndex == 0) {
            player.Respawn();
            OnRespawn?.Invoke();
        }
        Debug.Log("save progress " + rooms.IndexOf(currentRoom));

    }

    ///<summary>
    /// Loads and initializes all rooms from the inspector values.
    ///</summary>
    private void InitializeRooms() {
        Vector3 pos = Vector3.zero;
        Transform origin = transform;
        int index = 0;
        foreach (RoomLevel roomLevel in roomLevels) {
            bool completed = index < loadRoomIndex;
            index += 1;

            //make new room
            Room newRoom = Instantiate(roomLevel.prefab.gameObject, transform).GetComponent<Room>();
            newRoom.name = "Room: " + (roomLevel.roomInfo != null ? roomLevel.name : roomLevel.prefab.name);
            newRoom.Area = this;
            newRoom.roomLevel = roomLevel;
            int changeMirrorIndex = 0;
            if (roomLevel.roomInfo != null) {
                newRoom.RevealChangeAfterCompletion = roomLevel.roomInfo.revealChangesAfterFinish;
                foreach (Mirror mirror in newRoom.GetAllObjectsInRoom<Mirror>())
                {
                    if (mirror.isQuestion) {
                        if (completed) {
                            mirror.PreAnswer = roomLevel.roomInfo.changeMirror[0].letters;
                            mirror.IsOn = true;
                        }  else
                            mirror.Letters = roomLevel.roomInfo.questionMirror.letters;
                        mirror.changeType = roomLevel.roomInfo.questionMirror.changeType;
                        mirror.roomIndexoffset = roomLevel.roomInfo.questionMirror.roomIndexoffset;
                    } else {
                        mirror.PreAnswer = roomLevel.roomInfo.changeMirror[changeMirrorIndex].letters;
                        mirror.changeType = roomLevel.roomInfo.changeMirror[changeMirrorIndex].changeType;
                        mirror.roomIndexoffset = roomLevel.roomInfo.changeMirror[changeMirrorIndex].roomIndexoffset;
                        changeMirrorIndex++;
                    }
                }
            }
            newRoom.LoadMirrors();

            //position room
            newRoom.transform.rotation = origin.rotation; 
            newRoom.transform.Rotate(new Vector3(0,-90 + roomRotationOffset,0));
            newRoom.transform.position = origin.position + (newRoom.transform.position - newRoom.StartDoor.transform.position) + roomPositionOffset;
            origin = newRoom.EndDoor.transform;

            //deactivate the startdoors if the door isn't a portal
            if (newRoom.StartDoor.transform.GetComponent<PortalDoor>() == null) {
                newRoom.StartDoor.gameObject.SetActive(rooms.Count == 0);
            } else {
                //connect the two portal doors with each other.
                if (rooms.Count > 0) {
                    newRoom.StartDoor.GetComponent<PortalDoor>().ConnectedDoor = rooms[rooms.Count - 1].EndDoor.GetComponent<PortalDoor>();
                    rooms[rooms.Count - 1].EndDoor.GetComponent<PortalDoor>().ConnectedDoor = newRoom.StartDoor.GetComponent<PortalDoor>();
                }
            }

            //add to the list
            rooms.Add(newRoom);
        }
    }

    ///<summary>
    /// Fires when the palyer dies and has to respawn
    ///</summary>
    public void ResetPlayer() {
        StartCoroutine(ResettingThePlayer());
    }
    private IEnumerator ResettingThePlayer() {
        yield return new WaitForSeconds(3.5f);

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
        //CurrentRoom.ResetRoom();
    }

    public void SaveProgress() {
        // SaveData.current = SaveData.GetStateOfRoom(currentRoom);
        SaveData.current.roomIndex = rooms.IndexOf(currentRoom);
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);
    }

    private void OnDestroy() {
        SaveProgress();
    }

    public void LoadProgress() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/" + SaveData.FILE_NAME +".save");
        if (data != null) {

            SaveData.current = data as SaveData;
#if !UNITY_EDITOR
            loadRoomIndex = SaveData.current.roomIndex;
#endif
            //loadRoomState = true;
        }
        furthestCurrentRoomIndex = loadRoomIndex;
    }

    private void UndoAction() {
        OnUndo?.Invoke(currentRoom);
    }

    private void ShowNextRoom(Door door) {
        UpdateRoomActiveStates(true);
    }

    private void OnPassingThroughDoor(Door door) {
        OnNewRoomEnter?.Invoke();
        int index = rooms.IndexOf(door.room);

        if (door.room == currentRoom && door == currentRoom.EndDoor) {
            //next room
            if (index == rooms.Count - 1) {
                //loop room 
                Vector3 localPos = CurrentRoom.EndDoor.transform.InverseTransformPoint(player.transform.position);
                player.transform.position = rooms[index - 1].EndDoor.transform.TransformPoint(localPos);
                player.transform.Rotate(0,90,0);
                directionalLight.animating = false;
                directionalLight.RotateToMatchRoon(rooms[index - 1].transform);
                directionalLight.animating = true;
                
                CurrentRoom = rooms[index];
                rooms[index - 1].EndDoor.GoingThrough();
                StartCoroutine(rooms[index - 1].EndDoor.Walking(1.5f, player));

                return;
            } else {
                furthestCurrentRoomIndex = index + 1;
                CurrentRoom = rooms[index + 1];
            }
        } else {
            //start door to previous room
            if (door == currentRoom.StartDoor) {
                CurrentRoom = rooms[index - 1];
            } else {
                //previous room
                CurrentRoom = rooms[index];
            }
        }
        StartCoroutine(door.Walking(1.5f, player));
    }

}
