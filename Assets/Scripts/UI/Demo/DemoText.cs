using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoText : MonoBehaviour
{
    [SerializeField]
    private SceneLoader sceneLoader;
    private CanvasGroup group;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    private void OnEnable() {
        Area.OnEndOfDemo += ShowDemoText;
        AlchemyItem.OnAlchemyEndScene += FadeToWhite;
    }
    private void OnDisable() {
        Area.OnEndOfDemo -= ShowDemoText;
        AlchemyItem.OnAlchemyEndScene -= FadeToWhite;
    }

    private void ShowDemoText(int _areaIndex, bool animating) {
        StartCoroutine(group.FadeCanvasGroup(1, 3f, 1));
        StartCoroutine(WaitForExit());
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
        StartCoroutine(group.FadeCanvasGroup(1, 3f, 7f));
        float index = 0;
        bool clicked = false;
        while(index < 10f) {
            index += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape) && !clicked) {
                clicked = true;
                SceneManager.LoadSceneAsync(Scenes.MENU_SCENE_NAME, LoadSceneMode.Single);
            }
            yield return new WaitForEndOfFrame();
        }
        if (!clicked) SceneManager.LoadSceneAsync(Scenes.MENU_SCENE_NAME, LoadSceneMode.Single);

    }
}
