using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static bool ANIMATING = false;
    [SerializeField]
    private CanvasGroup group;

    [SerializeField]
    private Image loadingIcon;
    [SerializeField]
    private Image cornerLoadingIcon;

    private bool isLoading = false;
    [SerializeField]
    private float animationDuration = 1f;
    [SerializeField]
    private float animationDelay = 0f;

    private void Start() {
        if (SceneLoader.ANIMATING) {
            SceneLoader.ANIMATING = false;
            StartCoroutine(LoadOut(() => {
            }));
        } else {
            AudioListener.volume = 1;
        }
    }

    private void LoadNewScene(string sceneName, bool showMainLoadingScreen) {
        if(showMainLoadingScreen) {
            AudioHandler.Instance.FadeListener(0);
            if (isLoading) return;
            StartCoroutine(LoadingSceneAsyncMainLoadingScreen(sceneName));
        } else {
            if (isLoading) return;
            StartCoroutine(LoadingSceneAsyncWithCornerLoadingIcon(sceneName));
        }
    }
    public void LoadNewSceneAnimated(string sceneName) {
        LoadNewScene(sceneName, true);
    }

    public IEnumerator LoadingSceneAsyncMainLoadingScreen(string sceneName)
    {
        SceneLoader.ANIMATING = true;
        loadingIcon.gameObject.SetActive(false);
        isLoading = true;
        yield return StartCoroutine(FadeCanvasGroup(group, 1f, animationDuration, animationDelay));
        loadingIcon.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        isLoading = false;
    }
    public IEnumerator LoadingSceneAsyncWithCornerLoadingIcon(string sceneName)
    {
        SceneLoader.ANIMATING = true;
        cornerLoadingIcon.gameObject.SetActive(false);
        isLoading = true;
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        cornerLoadingIcon.gameObject.SetActive(true);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        isLoading = false;
    }

    public void Quit() {
        QuitGame();
    }
    public static void QuitGame() {
        #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #elif UNITY_WEBPLAYER
        string webplayerQuitURL = "http://google.com";
        Application.OpenURL(webplayerQuitURL);
         #else
         Application.Quit();
         #endif
    }

    private void OnEnable() {
        AlchemyItem.OnAlchemyEndScene += GoToSecondLevel;
    }

    private void OnDisable() {
        AlchemyItem.OnAlchemyEndScene -= GoToSecondLevel;
    }


    private void GoToSecondLevel() {
        SaveData.current.areaIndex = 1;
        SaveData.current.roomIndex = 0;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);

        Area.AUTO_SAVE_WHEN_DESTROY = false;
        LoadNewScene(Scenes.SECOND_LEVEL_SCENE_NAME, false);
        // LoadingSceneAsync(Scenes.SECOND_LEVEL_SCENE_NAME);

    }


    private IEnumerator LoadOut(Action callback)
    {
        group.alpha = 1;
        AudioListener.volume = 0;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.3f);
        AudioHandler.Instance.FadeListener(1);
        yield return StartCoroutine(FadeCanvasGroup(group, 0, .5f));
        callback();
    }
    public IEnumerator FadeCanvasGroup(CanvasGroup group, float end, float duration, float delay = 0) {
        yield return new WaitForSecondsRealtime(delay);
        float index = 0;
        float begin = group.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             group.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        group.alpha = end;
    }

}
