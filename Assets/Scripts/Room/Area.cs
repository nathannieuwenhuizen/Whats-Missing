using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{

    public static bool AUTO_SAVE_WHEN_DESTROY = true;

    [SerializeField]
    private GameObject defaultRoom;

    [SerializeField]
    private Vector3 roomPositionOffset;
    [SerializeField]
    private float roomRotationOffset = 0;
    [SerializeField]
    private Color startColor = Color.black;
    [SerializeField]
    private int areaIndex = 0;
    [SerializeField]
    private bool isDemo = false;

    public delegate void UndoActionEvent(Room _room);
    public static UndoActionEvent OnUndo;
    public delegate void RoomEvent();
    public delegate void RoomRespawnEvent(bool withColor);
    public delegate void NextAreaEvent(int _index, bool animating);
    public static NextAreaEvent OnNextArea;
    public static NextAreaEvent OnEndOfDemo;
    public static RoomEvent OnNewRoomEnter;
    public static RoomEvent OnFirstAreaEnter;
    public static RoomEvent OnSecondAreaEnter;
    public static RoomRespawnEvent OnRespawn;

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
            UpdateRoomMusic(rooms.IndexOf(currentRoom));
            currentRoom.OnRoomEnter(player, loadRoomState);
            if(directionalLight != null) directionalLight.RotateToMatchRoon(currentRoom.transform);

            if (rooms.IndexOf(currentRoom) == 0) {
                if (areaIndex == 0) OnFirstAreaEnter?.Invoke();
                else if (areaIndex == 1) OnSecondAreaEnter?.Invoke();
            }
            loadRoomState = false;
        }
    }


    private void Awake() {
        if (defaultRoom != null) Destroy(defaultRoom);
        AUTO_SAVE_WHEN_DESTROY = true;
        LoadProgress();
        InitializeRooms();
    }
    
    void Start()
    {
        PlayAreaMusic();

        SetupPlayerPos();

        if(directionalLight != null) directionalLight.animating = false;
        CurrentRoom = FindRoomBasedOnLoadIndex(loadRoomIndex);
        if(directionalLight != null) directionalLight.animating = true;

        if (loadRoomIndex == 0) {
            player.Respawn();
            BlackScreenOverlay.START_COLOR = startColor;
            OnRespawn?.Invoke(true);
        }
    }

    private void UpdateRoomActiveStates(bool includingNextRoom = false) {
        int currentIndex = rooms.IndexOf(currentRoom);
        if (currentIndex >= rooms.Count - 1) StartCoroutine(LoadNextRoom());
        
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
            if (rooms[i].LoadIndex < furthestCurrentRoomIndex - 1) {
                rooms[i].EndDoor.Animated = false;
                rooms[i].EndDoor.Locked = true;
            } else if (rooms[i].LoadIndex >= (furthestCurrentRoomIndex - 1) && rooms[i].LoadIndex < furthestCurrentRoomIndex) {
                if (rooms[i].gameObject.activeSelf)
                    rooms[i].EndDoor.Locked = false;
            }


        }
    }

    #region  music

    ///<summary>
    /// Plays the area music according in which area you're in
    ///</summary>
    private void PlayAreaMusic() {
        //fade audio listener
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 0;
        AudioHandler.Instance.FadeListener(1f);

        //play music
        AudioHandler.Instance.PlayMusic(GetAreaMusic(), .5f, loadRoomIndex == 0 ? 5.5f : 0f);
    }

    ///<summary>
    /// Returns the appropiate music according ot the area index
    ///</summary>
    private MusicFiles GetAreaMusic() {
        MusicFiles result = MusicFiles.planetarium;
        switch(areaIndex) {
            case 0:
            result = MusicFiles.planetarium;
            break;
            case 1:
            result = MusicFiles.garden;
            break;
            case 2:
            result = MusicFiles.tower;
            break;
        }
        return result;
    }

    public Room FindRoomBasedOnLoadIndex(int index) {
        foreach(Room r in rooms) {
            if (r.LoadIndex == index) return r;
        }
        return null;
    }

    #endregion

    ///<summary>
    /// Sets the player positionn and rotation to the room start door. Called at the start.
    ///</summary>
    private void SetupPlayerPos() {
        player.transform.position = FindRoomBasedOnLoadIndex(loadRoomIndex).StartDoor.EndPos();
        player.transform.rotation = FindRoomBasedOnLoadIndex(loadRoomIndex).StartDoor.transform.rotation;
        player.transform.Rotate(0,180,0);
    }
    Transform loadPosition;
    ///<summary>
    /// Loads and initializes all rooms from the inspector values.
    ///</summary>
    private void InitializeRooms() {
        Vector3 pos = Vector3.zero;
        loadPosition = transform;
        int index = 0;
        foreach (RoomLevel roomLevel in roomLevels) {
            bool completed = index < loadRoomIndex;

            //dont load room in that are behind the player progression
            // if (index < loadRoomIndex - 2) continue;
            if (index < loadRoomIndex + 3 && index > loadRoomIndex - 3) 
            {
                // if (index > loadRoomIndex - 3)
                // {
                    Debug.Log("roomlevel: " + roomLevel.name + " | " + index + " | loadindex = " + loadRoomIndex);
                    LoadRoom(roomLevel, completed, index);
                // }
            }
            index++;
        }
    }

    private IEnumerator LoadNextRoom() {
        if (roomLevels.Length != rooms[rooms.Count - 1].LoadIndex + 1)
            LoadRoom(roomLevels[rooms[rooms.Count - 1].LoadIndex + 1], false, rooms[rooms.Count - 1].LoadIndex + 1);

        yield return new WaitForEndOfFrame();
    }


    private void LoadRoom(RoomLevel roomLevel, bool completed, int index) {
        
        //make new room
        Room newRoom = Instantiate(roomLevel.prefab.gameObject, transform).GetComponent<Room>();
        newRoom.name = "Room: " + (roomLevel.roomInfo != null ? roomLevel.name : roomLevel.prefab.name);
        newRoom.Area = this;
        newRoom.roomLevel = roomLevel;
        newRoom.Player = player;
        newRoom.LoadIndex = index;
        int mirrorIndex = 0;


        if (roomLevel.roomInfo != null) {
            if (roomLevel.roomInfo.loadedChanges.Length > 0) newRoom.SecondHintAnswer = roomLevel.roomInfo.loadedChanges[0].word;
            newRoom.RevealChangeAfterCompletion = roomLevel.roomInfo.revealChangesAfterFinish;
            foreach (Mirror mirror in newRoom.GetAllObjectsInRoom<Mirror>())
            {
                if (mirrorIndex < roomLevel.roomInfo.questionMirror.Length) {
                    MirrorData _mirrorData = roomLevel.roomInfo.questionMirror[mirrorIndex].Clone;
                    mirror.MirrorData = _mirrorData;
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
                    mirror.MirrorCanvas.IsInteractable = mirror.MirrorData.isInteractable;
                    mirrorIndex++;
                }
            }
        }
        
        newRoom.LoadMirrors();

        //position room
        if (areaIndex == 0) {
            newRoom.transform.rotation = loadPosition.rotation; 
            newRoom.transform.Rotate(new Vector3(0,-90 + roomRotationOffset,0));
        }
        Vector3 spawnPos = loadPosition.position + (newRoom.transform.position - newRoom.StartDoor.transform.position);
        spawnPos += new Vector3(roomPositionOffset.x * ((index % 2) == 0 ? 1 : -1), roomPositionOffset.y, roomPositionOffset.z);

        newRoom.transform.position = spawnPos;
        loadPosition = newRoom.EndDoor.transform;

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

    ///<summary>
    /// Fires when the palyer dies and has to respawn
    ///</summary>
    public void ResetPlayer(bool withAnimation, bool toPreviousLevel) {
        currentRoom?.OnPlayerDie();
        StartCoroutine(ResettingThePlayer(withAnimation, toPreviousLevel));
    }

    ///<summary>
    /// Coroutine that resets the player after some time.
    ///</summary>
    private IEnumerator ResettingThePlayer(bool withAnimation, bool toPreviousLevel) {
        yield return new WaitForSeconds(withAnimation ? 3.5f : 2.5f);
        player.Respawn();
        int index = rooms.IndexOf(currentRoom);
        if(index == 0) {
            CurrentRoom = rooms[0];
            player.transform.position = CurrentRoom.StartDoor.EndPos();
            player.transform.rotation = Quaternion.Euler(new Vector3(0,CurrentRoom.StartDoor.transform.eulerAngles.y - 180f,0));
        } else if (toPreviousLevel){

            furthestCurrentRoomIndex = index - 1;
            CurrentRoom = rooms[index - 1];
            player.transform.position = CurrentRoom.EndDoor.StartPos();
            player.transform.rotation = Quaternion.Euler(new Vector3(0,CurrentRoom.EndDoor.transform.eulerAngles.y - 180f, 0));

        } else {
            CurrentRoom = rooms[index - 1];
            CurrentRoom = rooms[index];
            player.transform.position = CurrentRoom.StartDoor.EndPos();
            Debug.Log("rotation = " + CurrentRoom.StartDoor.transform.eulerAngles.y);
            player.transform.rotation = Quaternion.Euler(new Vector3(0,CurrentRoom.StartDoor.transform.eulerAngles.y - 180f,0));
        }
         
        OnRespawn?.Invoke(false);
    }

    private void OnEnable() {
        Door.OnPassingThrough += OnPassingThroughDoor;
        Door.OnDoorOpen += ShowNextRoom;
        InputManager.OnUndo += UndoAction;
        Player.OnDie += ResetPlayer;
        TeddyBear.OnCutsceneEnd += EndOfArea;
        AlchemyItem.OnAlchemyEndScene += EndOfArea;
        InputManager.OnReset += ResetRoom;
    }

    private void OnDisable() {
        Door.OnPassingThrough -= OnPassingThroughDoor;
        Door.OnDoorOpen -= ShowNextRoom;
        InputManager.OnUndo -= UndoAction;
        Player.OnDie -= ResetPlayer;
        InputManager.OnReset -= ResetRoom;

        TeddyBear.OnCutsceneEnd -= EndOfArea;
        AlchemyItem.OnAlchemyEndScene -= EndOfArea;

        if (AUTO_SAVE_WHEN_DESTROY) SaveProgress();
    }

    public void ResetRoom() {
        //CurrentRoom.ResetRoom();
    }
    float testParam;
    public void UpdateRoomMusic(float _roomIndex) {
        if (areaIndex == 1){
            _roomIndex = (float)_roomIndex / (float)rooms.Count * 26f;
            Debug.Log("room music: " + _roomIndex);
            if (AudioHandler.Instance?.AudioManager.Music != null) 
                AudioHandler.Instance?.AudioManager.Music.FMODInstance.setParameterByName(FMODParams.LEVEL2_MUSIC, _roomIndex, true);
        }
    }

    public void SaveProgress() {
        // SaveData.current = SaveData.GetStateOfRoom(currentRoom);
        SaveData.current.roomIndex = currentRoom.LoadIndex;
        SaveData.current.areaIndex = areaIndex;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);
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
            furthestCurrentRoomIndex = door.room.LoadIndex + 1;
            CurrentRoom = rooms[index + 1]; 
        } else {
            //start door to previous room
            if (door == currentRoom.StartDoor) {
                CurrentRoom = rooms[index - 1];
            } else {
                //previous room
                CurrentRoom = rooms[index];
            }
        }
    }

    ///<summary>
    /// Go to next level. Or ned of demo
    ///</summary>
    public void EndOfArea() {
        if (isDemo) OnEndOfDemo?.Invoke(areaIndex, false);
        else OnNextArea?.Invoke(areaIndex, false);
    }

}
