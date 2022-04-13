using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LoadingStyle {
    none,
    mainScreen,
    cornerIcon
}

public class SceneLoader : MonoBehaviour
{
    public static LoadingStyle LOADING_STYLE = LoadingStyle.none;
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
        if (SceneLoader.LOADING_STYLE != LoadingStyle.none) {
            StartCoroutine(LoadOut(() => {
                SceneLoader.LOADING_STYLE = LoadingStyle.none;
            }));
            
        } else {
            AudioHandler.Instance.AudioManager.AudioListenerVolume = 1;
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
        SceneLoader.LOADING_STYLE = LoadingStyle.mainScreen;
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
        SceneLoader.LOADING_STYLE = LoadingStyle.cornerIcon;
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
        PauseScreen.OnQuit += BackToMenu;
    }


    private void BackToMenu() {
        LoadNewSceneAnimated("Menu");
    }

    private void OnDisable() {
        AlchemyItem.OnAlchemyEndScene -= GoToSecondLevel;
        PauseScreen.OnQuit -= BackToMenu;

    }


    private void GoToSecondLevel() {
        SaveData.current.areaIndex = 1;
        SaveData.current.roomIndex = 0;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);

        Area.AUTO_SAVE_WHEN_DESTROY = false;
        LoadNewScene(Scenes.SECOND_LEVEL_SCENE_NAME, false);
        // LoadingSceneAsync(Scenes.SECOND_LEVEL_SCENE_NAME);

    }

    ///<summary>
    /// When the new scene gets loaded i, the loading screen should fisrt be visible and then fade out
    ///</summary>
    private IEnumerator LoadOut(Action callback)
    {
        if (LOADING_STYLE == LoadingStyle.mainScreen)
        {
            group.alpha = 1;
            loadingIcon.gameObject.SetActive(true);
        } else if (LOADING_STYLE == LoadingStyle.cornerIcon) {
            cornerLoadingIcon.gameObject.SetActive(true);

        }
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 0;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.3f);
        AudioHandler.Instance.FadeListener(1);
        if (LOADING_STYLE == LoadingStyle.mainScreen) 
            yield return StartCoroutine(FadeCanvasGroup(group, 0, .5f));
        
        
        cornerLoadingIcon.gameObject.SetActive(false);
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
