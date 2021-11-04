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
    
    public void UpdateText(string value) {
        text.text = value.ToLower();
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
