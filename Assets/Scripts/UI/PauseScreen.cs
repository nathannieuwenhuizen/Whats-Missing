
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
/// Pause screen inside the game.
///</summary>
public class PauseScreen : MonoBehaviour
{
    
    
    public delegate void PauseAction(); 
    public static event PauseAction OnPause;
    public static event PauseAction OnResume;
    public static event PauseAction OnQuit;
    public static event PauseAction OnSettingsOpen;
    public static event PauseAction OnSettingsClose;
    [SerializeField]
    private CanvasGroup group;
    [Header("buttons")]
    [SerializeField]
    private GameObject resumeButton;
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private AnimatedPopup subPausePanel;
    [SerializeField] 
    private Button settingsButton;

    private bool paused = false;
    private bool canPause = true;

    private Animator animator;

    private Coroutine fadeCoroutine;

    ///<summary>
    /// Pauses the game setting the timescale to 0.
    ///</summary>
    public void Pause()
    {

        if (paused || !canPause) return;
        paused = true;

        AudioHandler.Instance.FadeListener(.2f);
        animator.SetBool("show", true);
        AudioHandler.Instance.PlayUISound(SFXFiles.pause_show);
        StartCoroutine(AnimateTimeScale(0));
        OnPause?.Invoke();
        OpenPausePanel();
    }

    public void OpenPausePanel() {
        OnSettingsClose?.Invoke();
        ControllerCheck.SelectUIGameObject(resumeButton);
        SetGroupVisibility(true);
        subPausePanel.ShowAnimation(true);
        StartCoroutine(SelectingGameObject());
    }

    IEnumerator SelectingGameObject() {
        yield return new WaitForEndOfFrame();
        ControllerCheck.SelectUIGameObject(resumeButton);

    }

    private void Awake() {
        animator = GetComponent<Animator>();
        resumeButton.GetComponent<Button>().onClick.AddListener(Resume);
        quitButton.onClick.AddListener(Quit);
        settingsButton.onClick.AddListener(GoToSettings);
    }

    private void Start()
    {
        // SetGroupVisibility(false);
    }

    ///<summary>
    /// Sets the visibility and interactebility of the group to the val.
    ///</summary>
    protected void SetGroupVisibility(bool val)
    {
        group.alpha = val ? 1 : 0;
        group.interactable = val;
        group.blocksRaycasts = val;
    }

    private IEnumerator AnimateTimeScale(float end) {
        float begin = Time.timeScale;
        
        float index = 0;
        float animationDuration = .5f;
        while (index < animationDuration) {
            index += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(begin, end, index/ animationDuration);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = end;
    }

    ///<summary>
    /// Resumes the game hiding the pauses screen and setting the timescale to 1.
    ///</summary>
    public void Resume()
    {
        if (!paused) return;
        paused = false;

        AudioHandler.Instance.FadeListener(1f);
        animator.SetBool("show", false);
        subPausePanel.ShowAnimation(false);
        OnSettingsClose?.Invoke();

        AudioHandler.Instance.PlayUISound(SFXFiles.pause_hide);
        EventSystem.current.SetSelectedGameObject(null);

        SetGroupVisibility(false);
        StartCoroutine(AnimateTimeScale(1));
        OnResume?.Invoke();
    }
    public void Quit() {
        OnQuit?.Invoke();
    }
    ///<summary>
    /// Toggles the pause screen.
    ///</summary>
    private void TogglePause()
    {
        if (paused) Resume();
        else Pause();
    }

    private void OnEnable()
    {
        InputManager.OnCancel += TogglePause;
        CharacterAnimationPlayer.OnCutsceneStart += DisablePause;
        CharacterAnimationPlayer.OnCutsceneEnd += EnablePause;
        SettingPanel.OnSettingsClose += OpenPausePanel;
    }
    private void OnDisable()
    {
        InputManager.OnCancel -= TogglePause;
        CharacterAnimationPlayer.OnCutsceneStart -= DisablePause;
        CharacterAnimationPlayer.OnCutsceneEnd -= EnablePause;
        SettingPanel.OnSettingsClose -= OpenPausePanel;
    }

    public void SelectResumeButton() {
        ControllerCheck.SelectUIGameObject(resumeButton);
    }

    public void GoToSettings() {
        OnSettingsOpen?.Invoke();
        subPausePanel.ShowAnimation(false);
    }

    private void EnablePause() {
        canPause = true;
    }
    private void DisablePause() {
        canPause = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

}
