using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LegendaPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private CanvasGroup group;


    private void Awake() {
        group = GetComponent<CanvasGroup>();
    }

    private void HidePanel() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(0, 1f, 0));
    }
    private void ShowPanel() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 0));
    }
    
    public void UpdateText(string word) {
        StopAllCoroutines();
        StartCoroutine(TransitionWord(word));
        // text.text = word.ToLower();
    }

    private IEnumerator TransitionWord(string word) {
        if (group.alpha > .5f) {
            yield return text.FadeText(0, .2f, 0);
        }
        text.text = word.ToLower();
        yield return text.FadeText(1, .5f, 0);
    }

    private void OnEnable() {
        Player.OnCutsceneStart += HidePanel;
        Player.OnCutsceneEnd += ShowPanel;
        Legenda.OnFocus += UpdateText;
    }
    private void OnDisable() {
        Player.OnCutsceneStart -= HidePanel;
        Player.OnCutsceneEnd -= ShowPanel;
        Legenda.OnFocus -= UpdateText;
    }
}
