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
    [SerializeField] private Button selectedButtonForController;

    public void BackToMenu() {
        ShowAnimation(false);
        OnBehindTheScenesClose?.Invoke();
    }

    private void OnEnable() {
        Menu.OnBehindThescenesOpen += Show;
        SlideControls.OnSlidesClose += Show;
        InputManager.OnBack += Back;

    }

    private void OnDisable() {
        Menu.OnBehindThescenesOpen -= Show;
        SlideControls.OnSlidesClose -= Show;
        InputManager.OnBack -= Back;

    }

    public void Show() {
        ShowAnimation(true);
        ControllerCheck.SelectUIGameObject(selectedButtonForController.gameObject);
    }

    public void GoToSlideList(SlideList _list) {
        ShowAnimation(false);
        slideControls.OpenList(_list);
    }

    public void Awake() {
        closeButton.onClick.AddListener(BackToMenu);
    }
    public void Back() {
        if(alpha == 1) {
            BackToMenu();
        }

    }


}
