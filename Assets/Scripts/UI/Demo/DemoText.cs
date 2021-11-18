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
    }
    private void OnDisable() {
        AlchemyItem.OnAlchemyEndScene -= FadeToWhite;

    }
    public void FadeToWhite() {
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
