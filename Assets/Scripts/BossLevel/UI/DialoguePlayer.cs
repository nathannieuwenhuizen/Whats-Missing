using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;
using Boss;
using UnityEditor.Experimental.GraphView;

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

    private bool isPlaying = false;
    public bool IsPlaying {
        get { return isPlaying;}
    }

    List<SFXInstance> talkingInstances = new List<SFXInstance>();

    public void OnEnable() {
        // BossVoice.OnLineStart += PlayLine;
    }
    public void OnDisable() {
        // BossVoice.OnLineStart -= PlayLine;
        foreach(SFXInstance _instance in talkingInstances) {
            _instance.Stop();
        }
    }
    protected override void Awake()
    {
        backdrop.alpha = 0;
        text.text = "";
        base.Awake();
    }

    public void PlayLine(Line _line, bool forced = true, string SFX = "") {
        if (!forced && isPlaying) return;

        isPlaying = true;
        if (writingCoroutine != null)  {
            StopCoroutine(writingCoroutine);
            if (talkingInstances.Count > 0) StartCoroutine(FadeLineSFX(talkingInstances[0]));
        }
        StartCoroutine(backdrop.FadeCanvasGroup(1, .5f,_line.delay));
        writingCoroutine = StartCoroutine(PlayingLine(_line, SFX));
    }

    private IEnumerator PlayingLine(Line _line, string SFX = "") {
        yield return new WaitForSeconds(_line.delay);
        if (SFX != "") talkingInstances.Add(AudioHandler.Instance.PlaySound(SFX, AudioSetting.SFX * .3f));

        player.ShowText(getLineEffectTagStart(_line.lineEffect) + _line.text + getLineEffectTagEnd(_line.lineEffect));
        yield return new WaitForSeconds(_line.duration);
        if (SFX != "") StartCoroutine(FadeLineSFX(talkingInstances[0]));
        // yield return new WaitForSeconds(_line.duration);
        yield return StartCoroutine(DisappearingLine());
    }

    private  IEnumerator FadeLineSFX(SFXInstance _instance, float duration = 1) {
        talkingInstances.Remove(_instance);
        float start = _instance.Volume;
        float timePassed = 0f;
        while (timePassed < duration) {
           yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            _instance.Volume = Mathf.Lerp(start, 0, timePassed/duration);
        }
        _instance.Stop();
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
        isPlaying = false;
        StartCoroutine(backdrop.FadeCanvasGroup(0, .5f));
        yield return new WaitForSeconds(disappearingDuration);
    }
}
