using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SceneLoader : MonoBehaviour
{
    public static bool ANIMATING = false;
    private CanvasGroup group;
    [SerializeField]
    private Slider loadingSlider;

    [SerializeField]
    private Image loadingImage;

    private bool isLoading = false;
    [SerializeField]
    private float animationDuration = 1f;
    [SerializeField]
    private float animationDelay = 0f;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        if (SceneLoader.ANIMATING) {
            SceneLoader.ANIMATING = false;
            StartCoroutine(LoadOut(() => {
            }));
        } else {
            AudioListener.volume = 1;
        }
    }

    private void LoadNewScene(string sceneName, bool showTransition) {
        if(showTransition) {
            AudioHandler.Instance.FadeListener(0);
            if (isLoading) return;
            StartCoroutine(LoadingSceneAsync(sceneName));
        } else {
            SceneManager.LoadScene(sceneName);
        }
    }
    public void LoadNewSceneAnimated(string sceneName) {
        LoadNewScene(sceneName, true);
    }

    public IEnumerator LoadingSceneAsync(string sceneName)
    {
        Debug.Log("load scene async");
        SceneLoader.ANIMATING = true;
        loadingSlider.gameObject.SetActive(false);
        loadingImage.gameObject.SetActive(false);
        isLoading = true;
        yield return StartCoroutine(FadeCanvasGroup(group, 1f, animationDuration, animationDelay));
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // loadingSlider.gameObject.SetActive(true);
        loadingImage.gameObject.SetActive(true);
        loadingSlider.value = 0;
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(.2f);
            // loadingSlider.value = asyncLoad.progress;
            yield return new WaitForSeconds(.2f);
            yield return null;
        }
        isLoading = false;
    }

    public void Quit() {
        #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #elif UNITY_WEBPLAYER
        string webplayerQuitURL = "http://google.com";
        Application.OpenURL(webplayerQuitURL);
         #else
         Application.Quit();
         #endif
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
        Debug.Log("go to second levels");
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
        Debug.Log("load out!");
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
