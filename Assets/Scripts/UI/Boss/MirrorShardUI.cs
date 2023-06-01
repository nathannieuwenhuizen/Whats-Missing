using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MirrorShardUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private CanvasGroup group;

    private Coroutine wordCoroutine;
    private void Awake() {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    private void HidePanel() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(0, 1f, 0));
    }
    private void ShowPanel() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 0));
    }
    
    public void UpdateUI(BossMirror _bossMirror) {
        ShowPanel();
        int _ammountOfShards = _bossMirror.AmmountOfShardsAttached() ;
        int _totalAmmountOfShards = _bossMirror.Shards.Length;
        if (wordCoroutine != null)
            StopCoroutine(wordCoroutine);
        wordCoroutine = StartCoroutine(TransitionWord(_ammountOfShards.ToString(), _totalAmmountOfShards.ToString()));
    }

    private IEnumerator TransitionWord(string _value, string _total) {
        if (group.alpha > .5f) {
            yield return text.AnimateTextAlpha(0, .2f, 0);
        }
        text.text = _value + " / " + _total + " shards";
        yield return text.AnimateTextAlpha(1, .5f, 0);
    }

    private void OnEnable() {
        // PauseScreen.OnPause += HidePanel;
        // PauseScreen.OnResume += ShowPanel;
        // CharacterAnimationPlayer.OnCutsceneStart += HidePanel;
        // CharacterAnimationPlayer.OnCutsceneEnd += ShowPanel;
        BossMirror.OnMirrorShardAmmountUpdate += UpdateUI;
    }
    private void OnDisable() {
        // PauseScreen.OnPause -= HidePanel;
        // PauseScreen.OnResume -= ShowPanel;
        // CharacterAnimationPlayer.OnCutsceneStart -= HidePanel;
        // CharacterAnimationPlayer.OnCutsceneEnd -= ShowPanel;
        BossMirror.OnMirrorShardAmmountUpdate -= UpdateUI;
    }
}
