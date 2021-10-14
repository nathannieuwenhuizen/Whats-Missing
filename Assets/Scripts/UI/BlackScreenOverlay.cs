using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenOverlay : MonoBehaviour
{

    private CanvasGroup group;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    private void OnEnable() {
        Player.OnDie += FadeToBlack;
        Area.OnRespawn += RemoveBlack;
    }
    private void OnDisable() {
        Player.OnDie -= FadeToBlack;
        Area.OnRespawn -= RemoveBlack;
    }
    public void FadeToBlack() {
        StartCoroutine(group.FadeCanvasGroup(1, 1.5f));
    }
    public void RemoveBlack() {
        group.alpha = 0;
    }

}
