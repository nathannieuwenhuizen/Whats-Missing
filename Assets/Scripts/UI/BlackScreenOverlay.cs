using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenOverlay : MonoBehaviour
{

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
        AlchemyItem.OnPickingAlchemyItem += FadeToWhite;
    }
    private void OnDisable() {
        Player.OnDie -= DeathFade;
        Area.OnRespawn -= RemoveOverlay;
        AlchemyItem.OnPickingAlchemyItem -= FadeToWhite;
    }

    public void DeathFade(bool withAnimation) {
        if (withAnimation) {
            image.color = Color.black;
            StartCoroutine(group.FadeCanvasGroup(1, 1.5f, 1f));
        } else {
            image.color = Color.white;
            StartCoroutine(group.FadeCanvasGroup(1, 1.5f));
        }
    }

    public void FadeToBlack() {
        image.color = Color.black;
        StartCoroutine(group.FadeCanvasGroup(1, 1.5f, 1f));
    }
    public void FadeToWhite() {
        image.color = Color.white;
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 8f));
    }
    public void RemoveOverlay() {
        StopAllCoroutines();
        group.alpha = 1;
        StartCoroutine(group.FadeCanvasGroup(0, 1.5f, 1f));
    }

}
