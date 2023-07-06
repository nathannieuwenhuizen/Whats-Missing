using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneLoaderTrigger : MonoBehaviour, ITriggerArea
{
    public bool InsideArea { get; set; } = true;
    

    [SerializeField]
    private BlackScreenOverlay blackScreenOverlay;

    private bool fading = false;

    [SerializeField]
    private BossRoom bossRoom;

    public void OnAreaEnter(Player player)
    {
        if (fading) return;
        fading = true;

        StartCoroutine(FadeToEnd());
    }

    public IEnumerator FadeToEnd() {
        blackScreenOverlay.FadeToColor(RenderSettings.fogColor);
        yield return new WaitForSeconds(3f);
        bossRoom.EndOfArea();
    }

    public void OnAreaExit(Player player)
    {
        
    }
}
