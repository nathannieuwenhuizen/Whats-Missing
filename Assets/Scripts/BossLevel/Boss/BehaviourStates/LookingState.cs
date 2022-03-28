using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBossState{
    public BossAI bossAI;
}


public class LookingState : BaseBossState, IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Coroutine lookCoroutine;

    private float minLookDuration = 1f;
    private float maxLookDuration = 2f;
    private float viewRange = 5f;

    public void DrawDebug()
    {

    }

    public void Exit()
    {
        bossAI.StopCoroutine(lookCoroutine);
    }

    public void Run()
    {

    }
    private IEnumerator Looking() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minLookDuration, maxLookDuration));
            bossAI.BossHead.SetAim(Extensions.RandomVector2(viewRange));
        }
    }

    void IState.Start()
    {
        lookCoroutine = bossAI.StartCoroutine(Looking());
    }
}
