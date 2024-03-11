using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

public class BossRoom : Room
{
    public delegate void RoomEvent(bool withColor);
    public static RoomEvent OnRespawn;
    [SerializeField]
    private SceneLoader sceneLoader;
    
    
    [SerializeField]
    private BossMirror bossMirror;
    public BossMirror BossMirror {
        get { return bossMirror;}
    }

    [SerializeField]
    private Transform spawnPosition;
    [SerializeField]
    private BlackScreenOverlay blackScreenOverlay;

    [SerializeField]
    private bool setPlayerAtDoor = false;

    [SerializeField]
    private Boss.Boss boss;

    
    protected override void Awake() {
        base.Awake();
        bossMirror.Room = this;
        Area.AUTO_SAVE_WHEN_DESTROY = true;
    }
    
    //auto enter for the player
    private void Start() {
        hintStopwatch.room = this;
        hintStopwatch.Duration = 1;

        OnRoomEnter(Player, false);
        allObjects.Add(boss);
        
#if !UNITY_EDITOR
        setPlayerAtDoor = true;
#endif
        if (setPlayerAtDoor) {
            Player.transform.position = StartDoor.EndPos();
            Player.Respawn();
            blackScreenOverlay?.FadeFromColor(Color.black);
        }
        EndDoor.gameObject.SetActive(false);
    }

    private void OnEnable() {
        Player.OnDie += ResetPlayer;
        // DieState.OnBossDie += SpawnEndDoor;
        BossMirror.OnMirrorShardAmmountUpdate += StartHintTimer;
        PauseScreen.OnNextLevel += EndOfArea;

    }

    private void OnDisable() {
        Player.OnDie -= ResetPlayer;
        // DieState.OnBossDie -= SpawnEndDoor;
        BossMirror.OnMirrorShardAmmountUpdate -= StartHintTimer;
        if (Area.AUTO_SAVE_WHEN_DESTROY) SaveProgress();
        PauseScreen.OnNextLevel -= EndOfArea;

    }
    private void OnDestroy() {
        if (Area.AUTO_SAVE_WHEN_DESTROY) SaveProgress();
    }

    public override void ShowMirrorToggleHint() {
        foreach(Mirror mirror in mirrors) {
            mirror.MirrorCanvas.ShowSecondHintButton((mirror as BossMirror).GetAnswer());
        }
    }
    public override void ShowMirrorToggleSecondHint() {
        foreach(Mirror mirror in mirrors) {
            mirror.MirrorCanvas.ShowSecondHintButton((mirror as BossMirror).GetAnswer());
        }
    }

    public void StartHintTimer(BossMirror b) {
        // Debug.Log(" start hint timer!");
        hintStopwatch.timerForSecondHint = false;
        hintStopwatch.StartTimerSecondHint("", 5);
    }


    private void SpawnEndDoor() {
        // EndDoor.gameObject.SetActive(true);
        // Vector3 endScale = EndDoor.transform.localScale;
        // EndDoor.transform.localScale = Vector3.zero;
        // StartCoroutine(EndDoor.transform.AnimatingLocalScale(endScale, AnimationCurve.EaseInOut(0,0,1,1), 3f));
        // EndDoor.Locked = false;
    }


    ///<summary>
    /// Fires when the player dies and has to respawn
    ///</summary>
    public void ResetPlayer(bool withAnimation, bool toPreviousLevel) {
        PlayerData.DIE_AREA_3 = true;
        PlayerData.CheckSeekerfDeathAchievements();

        StartCoroutine(ResettingThePlayer(withAnimation, toPreviousLevel));
    }

    ///<summary>
    /// Coroutine that resets the player after some time.
    ///</summary>
    private IEnumerator ResettingThePlayer(bool withAnimation, bool toPreviousLevel) {
        yield return new WaitForSeconds(withAnimation ? 3.5f : 2.5f);
        Player.transform.position = spawnPosition.position;
        Player.transform.rotation = spawnPosition.rotation;
        Player.Respawn();
         
        BlackScreenOverlay.START_COLOR = Color.white;
        OnRespawn?.Invoke(true);
    }
    public void EndOfArea() {
        sceneLoader.GoToNextLevel(3, false);
    }

    public void SaveProgress() {
        SaveData.current.roomIndex = 0;
        SaveData.current.areaIndex = 3;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);
    }


    //no base call
    public override void CheckRoomCompletion()
    {
        // base.CheckRoomCompletion();
    }


}
