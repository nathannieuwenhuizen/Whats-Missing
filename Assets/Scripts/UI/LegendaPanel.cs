using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Febucci.UI;

public class LegendaPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private TextAnimatorPlayer animationPlayer;


    private CanvasGroup group;

    private Coroutine wordCoroutine;
    private void Awake() {
        animationPlayer = GetComponentInChildren<TextAnimatorPlayer>();
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
        if (wordCoroutine != null)
            StopCoroutine(wordCoroutine);

        wordCoroutine = StartCoroutine(TransitionWord(word));
    }

    private IEnumerator TransitionWord(string word) {
        // if (group.alpha > .5f) {
        //     yield return text.AnimateTextAlpha(0, .2f, 0);
        // }
        // text.text = word.ToLower();
        // yield return text.AnimateTextAlpha(1, .5f, 0);

        animationPlayer.StartDisappearingText();
        yield return new WaitForSeconds(.3f);
        if (word != "") {
            animationPlayer.ShowText(word);
        }

    }

    private void OnEnable() {
        PauseScreen.OnPause += HidePanel;
        PauseScreen.OnResume += ShowPanel;
        CharacterAnimationPlayer.OnCutsceneStart += HidePanel;
        CharacterAnimationPlayer.OnCutsceneEnd += ShowPanel;
        Legenda.OnFocus += UpdateText;
    }
    private void OnDisable() {
        PauseScreen.OnPause -= HidePanel;
        PauseScreen.OnResume -= ShowPanel;
        CharacterAnimationPlayer.OnCutsceneStart -= HidePanel;
        CharacterAnimationPlayer.OnCutsceneEnd -= ShowPanel;
        Legenda.OnFocus -= UpdateText;
    }
}
