using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room
{
    public delegate void RoomEvent(bool withColor);
    public static RoomEvent OnRespawn;
    
    [SerializeField]
    private BossMirror bossMirror;

    [SerializeField]
    private Transform spawnPosition;

    [SerializeField]
    private bool setPlayerAtDoor = false;
    
    protected override void Awake() {
        base.Awake();
        bossMirror.Room = this;
    }
    
    //auto enter for the player
    private void Start() {
        OnRoomEnter(Player, false);
#if !UNITY_EDITOR
        Player.transform.position = StartDoor.EndPos();
#endif
        if (setPlayerAtDoor) Player.transform.position = StartDoor.EndPos();
    }

    private void OnEnable() {
        Player.OnDie += ResetPlayer;
    }

    private void OnDisable() {
        Player.OnDie -= ResetPlayer;
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
        Player.transform.position = spawnPosition.position;
        Player.transform.rotation = spawnPosition.rotation;
        Player.Respawn();
         
        BlackScreenOverlay.START_COLOR = Color.white;
        OnRespawn?.Invoke(true);
    }

    public override void CheckRoomCompletion()
    {
        // base.CheckRoomCompletion();
    }


}
