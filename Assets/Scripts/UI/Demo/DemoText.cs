using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoText : MonoBehaviour
{
    private CanvasGroup group;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    private void OnEnable() {
        AlchemyItem.OnAlchemyEndScene += FadeToWhite;
        TeddyBear.OnCutsceneEnd += FadeToBlack;
    }
    private void OnDisable() {
        AlchemyItem.OnAlchemyEndScene -= FadeToWhite;
        TeddyBear.OnCutsceneEnd -= FadeToBlack;

    }
    public void FadeToWhite() {
        StartCoroutine(group.FadeCanvasGroup(1, 3f, 1));
        StartCoroutine(WaitForExit());
    }
    public void FadeToBlack() {
        GetComponent<Image>().color = Color.black;
        StartCoroutine(group.FadeCanvasGroup(1, 3f, 1));
        StartCoroutine(WaitForExit());
    }

    private IEnumerator WaitForExit() {
        while(true) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                SceneLoader.QuitGame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
