using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;
using Boss;

public class DialoguePlayer : Singleton<DialoguePlayer>
{

    [SerializeField]
    private TextAnimatorPlayer player;
    [SerializeField]
    private CanvasGroup backdrop;
    [SerializeField]
    private TMP_Text text;

    private Coroutine writingCoroutine;
    private float disappearingDuration = .5f;

    private void OnEnable() {
        BossVoice.OnLineStart += PlayLine;
    }
    private void OnDisable() {
        BossVoice.OnLineStart -= PlayLine;
    }
    protected override void Awake()
    {
        backdrop.alpha = 0;
        text.text = "";
        base.Awake();
    }

    public void PlayLine(Line _line) {
        if (writingCoroutine != null) StopCoroutine(writingCoroutine);
        StartCoroutine(backdrop.FadeCanvasGroup(1, .5f,0));
        writingCoroutine = StartCoroutine(PlayingLine(_line));
    }

    public IEnumerator PlayingLine(Line _line) {
        player.ShowText(getLineEffectTagStart(_line.lineEffect) + _line.text + getLineEffectTagEnd(_line.lineEffect));
        yield return new WaitForSeconds(2);
        // yield return new WaitForSeconds(_line.duration);
        yield return StartCoroutine(DisappearingLine());
    }

    private string getLineEffectTagStart(LineEffect lineEffect) {
        if (lineEffect == LineEffect.none) return "";
        return "<" + System.Enum.GetName(typeof(LineEffect), lineEffect) + ">";
    }
    private string getLineEffectTagEnd(LineEffect lineEffect) {
        if (lineEffect == LineEffect.none) return "";
        return "</>";
    }
    
    public IEnumerator DisappearingLine() {
        player.StartDisappearingText();
        StartCoroutine(backdrop.FadeCanvasGroup(0, .5f));
        yield return new WaitForSeconds(disappearingDuration);
    }
}
