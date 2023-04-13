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
        OnRoomEnter(Player, false);
        allObjects.Add(boss);
        
#if !UNITY_EDITOR
        Player.transform.position = StartDoor.EndPos();
        Player.Respawn();
#endif
        if (setPlayerAtDoor) {
            Player.transform.position = StartDoor.EndPos();
            Player.Respawn();
        }
        EndDoor.gameObject.SetActive(false);
    }

    private void OnEnable() {
        Player.OnDie += ResetPlayer;
        DieState.OnBossDie += SpawnEndDoor;
    }

    private void OnDisable() {
        Player.OnDie -= ResetPlayer;
        DieState.OnBossDie -= SpawnEndDoor;
        if (Area.AUTO_SAVE_WHEN_DESTROY) SaveProgress();
    }
    private void OnDestroy() {
        if (Area.AUTO_SAVE_WHEN_DESTROY) SaveProgress();
    }

    private void SpawnEndDoor() {
        EndOfArea();
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
        sceneLoader.GoToNextLevel(3, true);
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
