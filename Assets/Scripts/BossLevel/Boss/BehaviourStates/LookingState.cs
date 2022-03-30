using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// In this state, the boss is wandering trying to find the player.
/// The boss will move arround the mountain and tries to find the player
///</summary>
public class LookingState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Coroutine lookCoroutine;

    private float minLookDuration = 1f;
    private float maxLookDuration = 2f;
    private float viewRange = 5f;
    private BossEye eye;
    private Player player;

    public void DrawDebug()
    {
        if (eye == null) return;

        Color debugColor = Color.Lerp(Color.red, Color.green, eye.NoticingValue / eye.NoticingThreshold);
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(eye.transform.position, 2f);
    }
    void IState.Start()
    {
        lookCoroutine = bossAI.StartCoroutine(SetRandomLookDirections());
        eye = bossAI.BossEye;
    }

    public void Exit()
    {
        bossAI.StopCoroutine(lookCoroutine);
    }

    public void Run()
    {
        Debug.Log("ai " + bossAI);
        bossAI.BossEye.UpdateNoticing(bossAI.Boss.Player);
        if (bossAI.BossEye.NoticesPlayer) {
            OnStateSwitch?.Invoke(bossAI.Behaviours.chaseState);
        }
    }
    private IEnumerator SetRandomLookDirections() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minLookDuration, maxLookDuration));
            bossAI.BossHead.SetAim(bossAI.BossHead.transform.position + bossAI.BossHead.transform.parent.forward * eye.ViewRange,  Extensions.RandomVector2(viewRange));
        }
    }

}
