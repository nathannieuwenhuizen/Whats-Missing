using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsErrorMessage : AnimatedPopup
{
    protected bool visible = false;

    public delegate void ErrorAction();
    public static event ErrorAction OnErrorShow;
    public static event ErrorAction OnErrorHide;
    private bool hasBeenShown = false;

    private void OnEnable() {
        TextureProperty.OnTextureMissing += DelayPopup;
        InputManager.OnBack += Close;
        InputManager.OnClickDown += Close;
        InputManager.OnCancel += Close;

    }
    private void OnDisable() {
        TextureProperty.OnTextureMissing -= DelayPopup;
        InputManager.OnBack -= Close;
        InputManager.OnClickDown -= Close;
        InputManager.OnCancel -= Close;
    }
    private void DelayPopup() {
        if (hasBeenShown) return;
        hasBeenShown = true;
        StartCoroutine(DelayingPopup());

    } 
    private IEnumerator DelayingPopup() {
        yield return new WaitForSeconds(1f);
        ShowPopup();
    }

    private void ShowPopup() {
        visible = true;
        Time.timeScale = 0;
        OnErrorShow?.Invoke();
        AudioHandler.Instance.PlayUISound(SFXFiles.windows_error, 1f);
        ShowAnimation(true);
        AudioHandler.Instance.FadeListener(0, 1f);
        StartCoroutine(AutoClose());
    }

    private IEnumerator AutoClose() {
        yield return new WaitForSecondsRealtime(8f);
        Close();
    }
    public void Close() {
        if (!visible) return;
        visible = false;
        AudioHandler.Instance.FadeListener(1f);
        OnErrorHide?.Invoke();
        Time.timeScale = 1;
        ShowAnimation(false);

    }
}
