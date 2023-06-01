using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenOverlay : MonoBehaviour
{
    public static Color START_COLOR;
    private CanvasGroup group;
    private Image image;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    private void OnEnable() {
        Player.OnDie += DeathFade;
        Area.OnRespawn += RemoveOverlay;
        BossRoom.OnRespawn += RemoveOverlay;
        AlchemyItem.OnPickingAlchemyItem += OnAlchemyItemPicked;
        ThirdAreaEndTrigger.OnEndOfCutscene += FadeToBlack;

    }
    private void OnDisable() {
        Player.OnDie -= DeathFade;
        Area.OnRespawn -= RemoveOverlay;
        BossRoom.OnRespawn -= RemoveOverlay;
        AlchemyItem.OnPickingAlchemyItem -= OnAlchemyItemPicked;
        ThirdAreaEndTrigger.OnEndOfCutscene -= FadeToBlack;
    }

    public void DeathFade(bool withAnimation, bool toPreviousLevel) {
        Debug.Log("fade to white");
        if (withAnimation) {
            image.color = Color.black;
            StartCoroutine(group.FadeCanvasGroup(1, 1.5f, 1f));
        } else {
            image.color = Color.white;
            StartCoroutine(group.FadeCanvasGroup(1, 1.5f));
        }
    }

    private void OnAlchemyItemPicked() {
        FadeToWhite();
    }

    public void FadeToBlack() {
        image.color = Color.black;
        StartCoroutine(group.FadeCanvasGroup(1, 1.5f, 1f));
    }
    public void FadeToWhite(float duration = 1f, float delay = 8f) {
        image.color = Color.white;
        StartCoroutine(group.FadeCanvasGroup(1, duration, delay));
    }
    public void FadeToWhiteImmeditately() {
        image.color = Color.white;
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 0f));
    }
    public void RemoveOverlay(bool withStartColor = true) {
        StopAllCoroutines();
        if (withStartColor) image.color = START_COLOR;
        group.alpha = 1;
        StartCoroutine(group.FadeCanvasGroup(0, 1.5f, 1f));
    }

}
