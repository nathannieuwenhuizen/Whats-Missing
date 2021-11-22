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
    }
    private void OnDisable() {
        TextureProperty.OnTextureMissing -= DelayPopup;
        
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
    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            ShowPopup();
        }
    }

    private void ShowPopup() {
        visible = true;
        Time.timeScale = 0;
        AudioHandler.Instance.FadeListener(0);
        OnErrorShow?.Invoke();
        AudioHandler.Instance.PlayUISound(SFXFiles.windows_error, 1f);
        ShowAnimation(true);
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
