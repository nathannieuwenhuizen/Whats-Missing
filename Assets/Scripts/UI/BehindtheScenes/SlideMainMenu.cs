using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideMainMenu : AnimatedPopup
{
    public delegate void BehindTheScenesAction();
    public static BehindTheScenesAction OnBehindTheScenesClose;

    [SerializeField] private Button closeButton;
    [SerializeField] private SlideControls slideControls;

    public void BackToMenu() {
        ShowAnimation(false);
        OnBehindTheScenesClose?.Invoke();
    }

    private void OnEnable() {
        Menu.OnBehindThescenesOpen += Show;
        SlideControls.OnSlidesClose += Show;
    }

    private void OnDisable() {
        Menu.OnBehindThescenesOpen -= Show;
        SlideControls.OnSlidesClose -= Show;
    }

    public void Show() {
        ShowAnimation(true);
    }

    public void GoToSlideList(SlideList _list) {
        ShowAnimation(false);
        slideControls.OpenList(_list);
    }

    public void Awake() {
        closeButton.onClick.AddListener(BackToMenu);
    }

}
