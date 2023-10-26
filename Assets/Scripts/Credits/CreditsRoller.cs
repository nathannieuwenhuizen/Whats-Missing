using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoller : MonoBehaviour
{

    public static bool FROM_CREDIT_SCREEN {
         get { return PlayerPrefs.GetInt("FROM_CREDIT_SCREEN", 0) == 1; }
         set {  PlayerPrefs.SetInt("FROM_CREDIT_SCREEN", value ? 1 : 0); }
    }
    [SerializeField]
    private RectTransform end;
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private BlackScreenOverlay blackScreenOverlay;

    public delegate void CreditsFinishEvent();
    public static CreditsFinishEvent OnCreditsStart;
    public static CreditsFinishEvent OnCreditsFinish;

    private bool rolling = false;
    public void StartRolling() {
        FROM_CREDIT_SCREEN = true;
        Debug.Log("start rolling");
        SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.Acceptance);
        StartCoroutine(Rolling());
    }
    public IEnumerator Rolling() {
        OnCreditsStart?.Invoke();
        rolling = true;
        yield return StartCoroutine(rect.AnimateLocalPosition(new Vector3(0,- end.localPosition.y,0), 40f, 0));
        EndOfCredits();
    }
    private void EndOfCredits() {
        if (!rolling) return;
        rolling = false;

        blackScreenOverlay.FadeToBlack();
        OnCreditsFinish?.Invoke();
    }
    private void OnEnable() {
        InputManager.OnCancel += EndOfCredits;
    }
    private void OnDisable() {
        InputManager.OnCancel -= EndOfCredits;
        
    }
}
