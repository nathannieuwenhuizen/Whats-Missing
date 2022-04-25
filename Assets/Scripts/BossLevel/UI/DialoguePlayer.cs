using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Febucci.UI;

public class DialoguePlayer : MonoBehaviour
{
    private TextAnimatorPlayer player;
    private Coroutine writingCoroutine;
    private float disappearingDuration = .5f;

    // Start is called before the first frame update
    void Awake()
    {
        player = GetComponent<TextAnimatorPlayer>();
    }

    private void OnEnable() {
        BossVoice.OnLineStart += PlayLine;
    }
    private void OnDisable() {
        BossVoice.OnLineStart -= PlayLine;
    }

    public void PlayLine(Line _line) {
        if (writingCoroutine != null) StopCoroutine(writingCoroutine);
        writingCoroutine = StartCoroutine(PlayingLine(_line));
    }

    public IEnumerator PlayingLine(Line _line) {
        player.ShowText(_line.text);
        yield return new WaitForSeconds(2);
        // yield return new WaitForSeconds(_line.duration);
        yield return StartCoroutine(DisappearingLine());
    }
    public IEnumerator DisappearingLine() {
        player.StartDisappearingText();
        yield return new WaitForSeconds(disappearingDuration);
    }
}
