using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private Slider slider;

    private bool isLoading = false;

    public void LoadNextScene(string scene)
    {
        if (isLoading) return;

        isLoading = true;
        loadingPanel.SetActive(true);
        StartCoroutine(Loading(scene));
    }

    public IEnumerator Loading(string scene)
    {
        slider.value = 0;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(.2f);
            slider.value = asyncLoad.progress;
            yield return new WaitForSeconds(.2f);
            yield return null;
        }
        isLoading = false;
    }
}

