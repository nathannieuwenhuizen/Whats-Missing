using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorProperty : Property
{

    [SerializeField]
    private Texture2D mouseEmptyTexture;
    [SerializeField]
    private Texture2D bigMouseTexture;

    private List<CanvasGroup> ingameCanvasElements;
    private List<CanvasGroup> GetIngameCanvasElements() {
        List<CanvasGroup> list = new List<CanvasGroup>();
        list.Add(Canvas.FindObjectOfType<CursorUI>().GetComponent<CanvasGroup>());
        return list;
    }

    public CursorProperty() {
        animationDuration = 1f;
    }

    #region Missing
    private void Awake() {
        largeScale = 10f;
    }

    public override IEnumerator AnimateMissing()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            StartCoroutine(uiGroup.FadeCanvasGroup(0, animationDuration));
        }
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        Cursor.SetCursor(mouseEmptyTexture, new Vector2(0,0), CursorMode.ForceSoftware);
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            uiGroup.alpha = 0;
            uiGroup.gameObject.SetActive(false);
        }
        base.OnMissingFinish();
    }

    public override void OnAppearing()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            uiGroup.gameObject.SetActive(true);
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        base.OnAppearing();
    }

    public override IEnumerator AnimateAppearing()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            StartCoroutine(uiGroup.FadeCanvasGroup(1, animationDuration));
        }
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateAppearing();
    }
    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            uiGroup.alpha = 1;
        }
    }

    #endregion

    #region  Enlarging

    public override IEnumerator AnimateEnlarging()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            RectTransform rt = uiGroup.GetComponent<RectTransform>();
            StartCoroutine(rt.AnimatingScale(Vector3.one * LargeScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        }
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateEnlarging();
    }

    public override void OnEnlargingFinish()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            RectTransform rt = uiGroup.GetComponent<RectTransform>();
            rt.localScale = Vector3.one * LargeScale;
        }
        Debug.Log( "set big cursor");
        Cursor.SetCursor(bigMouseTexture, Vector2.zero, CursorMode.ForceSoftware);
        base.OnEnlargingFinish();
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            RectTransform rt = uiGroup.GetComponent<RectTransform>();
            StartCoroutine(rt.AnimatingScale(Vector3.one * normalScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        }
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateEnlargeRevert();
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        foreach(CanvasGroup uiGroup in ingameCanvasElements) {
            RectTransform rt = uiGroup.GetComponent<RectTransform>();
            rt.localScale = Vector3.one * normalScale;
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }
    #endregion

    public override void OnRoomEnter()
    {
        ingameCanvasElements = GetIngameCanvasElements();
        base.OnRoomEnter();
    }

    private void OnDisable() {
        if (InSpace) Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void Reset() {
        Word = "cursor";
        AlternativeWords = new string[]{ "mouse", "ui", "intrface", "cursors", "mouses", "target", "crosshair", "pointer"};
    }
}
