using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{

    public static bool AUTO_SAVE_WHEN_DESTROY = true;


    [SerializeField]
    private Vector3 roomPositionOffset;
    [SerializeField]
    private float roomRotationOffset = 0;
    [SerializeField]
    private Color startColor = Color.black;
    [SerializeField]
    private int areaIndex = 0;

    public delegate void UndoActionEvent(Room _room);
    public static UndoActionEvent OnUndo;
    public delegate void RoomEvent();
    public static RoomEvent OnNewRoomEnter;
    public static RoomEvent OnFirstAreaEnter;
    public static RoomEvent OnSecondAreaEnter;
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
            if(directionalLight != null) directionalLight.RotateToMatchRoon(currentRoom.transform);

            if (rooms.IndexOf(currentRoom) == 0) {
                if (areaIndex == 0) OnFirstAreaEnter?.Invoke();
                else OnSecondAreaEnter?.Invoke();
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
        AUTO_SAVE_WHEN_DESTROY = true;
        LoadProgress();
        InitializeRooms();
    }
    
    void Start()
    {
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 0;
        AudioHandler.Instance.FadeListener(1f);
        AudioHandler.Instance.PlayMusic(areaIndex == 0? MusicFiles.planetarium : MusicFiles.garden, .5f);

        player.transform.position = rooms[loadRoomIndex].StartDoor.EndPos();
        player.transform.rotation = rooms[loadRoomIndex].StartDoor.transform.rotation;
        player.transform.Rotate(0,180,0);// = rooms[startingRoomIndex].StartDoor.transform.rotation;
        //playerPos = rooms[startingRoomIndex].StartPos.position;
        if(directionalLight != null) directionalLight.animating = false;
        CurrentRoom = rooms[loadRoomIndex];
        if(directionalLight != null) directionalLight.animating = true;
        if (loadRoomIndex == 0) {
            player.Respawn();
            BlackScreenOverlay.START_COLOR = startColor;
            OnRespawn?.Invoke();
        }
    }

    ///<summary>
    /// Loads and initializes all rooms from the inspector values.
    ///</summary>
    private void InitializeRooms() {
        Vector3 pos = Vector3.zero;
        Transform origin = transform;
        int index = 0;
        foreach (RoomLevel roomLevel in roomLevels) {
            // roomLevel.roomInfo.MirrorData = roomLevel.roomInfo.MirrorData;
            // List<Change> temp = new List<Change>();
            // foreach(MirrorData data in roomLevel.roomInfo.changeMirror) {
            //     temp.Add(new Change() {word = data.letters, changeType = data.changeType, Active = data.isOn});
            // }
            // roomLevel.roomInfo.loadedChanges = temp.ToArray();


            bool completed = index < loadRoomIndex;
            index += 1;

            //make new room
            Room newRoom = Instantiate(roomLevel.prefab.gameObject, transform).GetComponent<Room>();
            newRoom.name = "Room: " + (roomLevel.roomInfo != null ? roomLevel.name : roomLevel.prefab.name);
            newRoom.Area = this;
            newRoom.roomLevel = roomLevel;
            int mirrorIndex = 0;
            if (roomLevel.roomInfo != null) {
                newRoom.RevealChangeAfterCompletion = roomLevel.roomInfo.revealChangesAfterFinish;
                foreach (Mirror mirror in newRoom.GetAllObjectsInRoom<Mirror>())
                {
                    if (mirrorIndex < roomLevel.roomInfo.questionMirror.Length) {
                        MirrorData questionMirrorData = roomLevel.roomInfo.questionMirror[mirrorIndex].Clone;
                        mirror.MirrorData = questionMirrorData;
                        mirror.isQuestion = mirror.MirrorData.isQuestion;

                        if (completed && roomLevel.roomInfo.loadedChanges.Length > 0) {
                            mirror.PreAnswer = roomLevel.roomInfo.loadedChanges[0].word;
                            mirror.IsOn = true;
                        }  else {
                            if(mirror.IsOn) {
                                mirror.PreAnswer = mirror.MirrorData.letters;
                                mirror.Letters = "";
                            } else {
                                mirror.Letters = mirror.MirrorData.letters;
                            }
                        }
                        mirrorIndex++;
                    }
                }
            }
            
            newRoom.LoadMirrors();

            //position room
            if (areaIndex == 0) {
                newRoom.transform.rotation = origin.rotation; 
                newRoom.transform.Rotate(new Vector3(0,-90 + roomRotationOffset,0));
            }
            Vector3 spawnPos = origin.position + (newRoom.transform.position - newRoom.StartDoor.transform.position);
            spawnPos += new Vector3(roomPositionOffset.x * ((index % 2) == 0 ? 1 : -1), roomPositionOffset.y, roomPositionOffset.z);

            newRoom.transform.position = spawnPos;
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
    public void ResetPlayer(bool withAnimation, bool toPreviousLevel) {
        StartCoroutine(ResettingThePlayer(withAnimation, toPreviousLevel));
    }

    ///<summary>
    /// Coroutine that resets the player after some time.
    ///</summary>
    private IEnumerator ResettingThePlayer(bool withAnimation, bool toPreviousLevel) {
        yield return new WaitForSeconds(withAnimation ? 3.5f : 2.5f);
        Debug.Log("respawn!");
        player.Respawn();
        int index = rooms.IndexOf(currentRoom);
        if(index == 0) {
            CurrentRoom = rooms[0];
            player.transform.position = CurrentRoom.StartDoor.EndPos();
        } else if (toPreviousLevel){

            furthestCurrentRoomIndex = index - 1;
            CurrentRoom = rooms[index - 1];
            player.transform.position = CurrentRoom.EndDoor.StartPos();
        } else {
            CurrentRoom = rooms[index - 1];
            CurrentRoom = rooms[index];
            player.transform.position = CurrentRoom.StartDoor.EndPos();
            // player.transform.rotation = CurrentRoom.StartDoor.transform.rotation;
            player.transform.rotation = Quaternion.Euler(new Vector3(0,CurrentRoom.StartDoor.transform.rotation.y - 90f,0));
        }
         
        BlackScreenOverlay.START_COLOR = Color.white;
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

        if (AUTO_SAVE_WHEN_DESTROY) SaveProgress();
    }

    public void ResetRoom() {
        //CurrentRoom.ResetRoom();
    }

    public void SaveProgress() {
        // SaveData.current = SaveData.GetStateOfRoom(currentRoom);
        SaveData.current.roomIndex = rooms.IndexOf(currentRoom);
        SaveData.current.areaIndex = areaIndex;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);

        Debug.Log("save progress!");
    }

    private void OnDestroy() {
        if (AUTO_SAVE_WHEN_DESTROY) SaveProgress();
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

    public bool IsLastRoom(Room room) {
        int index = rooms.IndexOf(room);
        return index == rooms.Count - 1;
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
                if(directionalLight != null) {
                    directionalLight.animating = false;
                    directionalLight.RotateToMatchRoon(rooms[index - 1].transform);
                    directionalLight.animating = true;
                }
                
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
