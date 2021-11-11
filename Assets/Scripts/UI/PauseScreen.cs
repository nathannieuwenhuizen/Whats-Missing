using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Pause screen inside the game.
///</summary>
public class PauseScreen : MonoBehaviour
{

    public delegate void PauseAction();
    public static event PauseAction OnPause;
    public static event PauseAction OnResume;
    [SerializeField]
    private CanvasGroup group;

    private bool paused = false;

    private Animator animator;

    private Coroutine fadeCoroutine;

    ///<summary>
    /// Pauses the game setting the timescale to 0.
    ///</summary>
    public void Pause()
    {

        if (paused) return;
        paused = true;

        AudioHandler.Instance.FadeListener(.2f);

        SetGroupVisibility(true);
        animator.SetBool("show", true);
        AudioHandler.Instance.PlayUISound(SFXFiles.pause_show);

        // Time.timeScale = 0;
        StartCoroutine(AnimateTimeScale(0));

        OnPause?.Invoke();
    }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetGroupVisibility(false);
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
        AudioHandler.Instance.PlayUISound(SFXFiles.pause_hide);

        SetGroupVisibility(false);
        StartCoroutine(AnimateTimeScale(1));
        // Time.timeScale = 1;
        OnResume?.Invoke();
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
    }
    private void OnDisable()
    {
        InputManager.OnCancel -= TogglePause;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

}
