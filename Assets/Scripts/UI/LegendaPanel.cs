using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LegendaPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private CanvasGroup group;

    private Coroutine wordCoroutine;
    private void Awake() {
        group = GetComponent<CanvasGroup>();
    }

    private void HidePanel() {
        Debug.Log("hide panel");
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(0, 1f, 0));
    }
    private void ShowPanel() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 0));
    }
    
    public void UpdateText(string word) {
        if (wordCoroutine != null)
            StopCoroutine(wordCoroutine);
        wordCoroutine = StartCoroutine(TransitionWord(word));
    }

    private IEnumerator TransitionWord(string word) {
        if (group.alpha > .5f) {
            yield return text.FadeText(0, .2f, 0);
        }
        text.text = word.ToLower();
        yield return text.FadeText(1, .5f, 0);
    }

    private void OnEnable() {
        PauseScreen.OnPause += HidePanel;
        PauseScreen.OnResume += ShowPanel;
        Player.OnCutsceneStart += HidePanel;
        Player.OnCutsceneEnd += ShowPanel;
        Legenda.OnFocus += UpdateText;
    }
    private void OnDisable() {
        PauseScreen.OnPause -= HidePanel;
        PauseScreen.OnResume -= ShowPanel;
        Player.OnCutsceneStart -= HidePanel;
        Player.OnCutsceneEnd -= ShowPanel;
        Legenda.OnFocus -= UpdateText;
    }
}
