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

    public bool Paused {
        get { return paused;}
    }

    ///<summary>
    /// Pauses the game setting the timescale to 0.
    ///</summary>
    public void Pause()
    {

        if (paused) return;
        paused = true;

        SetGroupVisibility(true);
        Time.timeScale = 0;
        OnPause?.Invoke();
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

    ///<summary>
    /// Resumes the game hiding the pauses screen and setting the timescale to 1.
    ///</summary>
    public void Resume()
    {
        if (!paused) return;
        paused = false;

        SetGroupVisibility(false);
        Time.timeScale = 1;
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
