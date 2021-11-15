using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProperty : Property
{

    private float animationDuration = 1f;

    [SerializeField]
    private Texture2D mouseEmptyTexture;
    [SerializeField]
    private Texture2D bigMouseTexture;

    private List<CanvasGroup> ingameCanvasElements;
    private List<CanvasGroup> GetIngameCanvasElements() {
        List<CanvasGroup> list = new List<CanvasGroup>();
        list.Add(Canvas.FindObjectOfType<CursorUI>().GetComponent<CanvasGroup>());
        list.Add(Canvas.FindObjectOfType<LegendaPanel>().GetComponent<CanvasGroup>());
        return list;
    }

    

    public override void OnMissing()
    {
        ingameCanvasElements = GetIngameCanvasElements();
        base.OnMissing();
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
        }
        base.OnMissingFinish();
    }

    public override void OnAppearing()
    {
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

    private void OnDisable() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }


    private void Reset() {
        Word = "ui";
        AlternativeWords = new string[]{ "interface", "user interface", "interfaces", "cursor", "gui"};
    }
}
