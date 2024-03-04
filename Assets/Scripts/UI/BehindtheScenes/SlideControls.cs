using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlideControls : AnimatedPopup
{
        public delegate void SlidesAction();
    public static SlidesAction OnSlidesClose;

    [SerializeField] private TMP_Text slideCount;
    [SerializeField] private TMP_Text subText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button backToOverviewButton;

    private bool isInView = false;

    [HideInInspector]
    public SlideList currentSlideList;

    private void Awake() {
        nextButton.onClick.AddListener(NextSlide);
        previousButton.onClick.AddListener(PreviousSlide);
        backToOverviewButton.onClick.AddListener(BackToOverview);
    }

    public void OpenList(SlideList _list) {
        ControllerCheck.SelectUIGameObject(nextButton.gameObject);
        currentSlideList = _list;
        currentSlideList.OpenList();
        UpdateListCount();
        ShowAnimation(true);
        isInView = true;
    }

    private void OnEnable() {
        InputManager.OnMove += OnMove;
    }
    private void OnDisable() {
        InputManager.OnMove -= OnMove;
    }
    private bool clicked = true;

    public void OnMove(Vector2 _vector) {
        if (!isInView) return;

        if (!clicked) {
            if (_vector.x > 0.5f)  {
                NextSlide();
                clicked = true;
            } else if (_vector.x < -0.5f) {
                PreviousSlide();
                clicked = true;
            }
        }
        if (Mathf.Abs(_vector.x) < 0.1f && clicked) clicked = false;

    }

    public void BackToOverview() {
        currentSlideList.CloseList();
        ShowAnimation(false);
        OnSlidesClose?.Invoke();
        isInView = false;
    }

    public void NextSlide() {
        if (currentSlideList.slideIndex >= currentSlideList.ammountofSlides - 1) return;
        currentSlideList.NextSlide();
        UpdateListCount();
    }

    public void PreviousSlide() {
        if (currentSlideList.slideIndex == 0) return;
        currentSlideList.PreviousSlide();
        UpdateListCount();
    }

    public void UpdateListCount() {
        slideCount.text = (currentSlideList.slideIndex + 1) + " / " + currentSlideList.ammountofSlides;
        subText.text = currentSlideList.CurrentSlide.SubText;
        Color nextA =nextButton.GetComponent<Image>().color;
        nextA.a = (currentSlideList.slideIndex < currentSlideList.ammountofSlides - 1) ? 1 : .5f;
        nextButton.GetComponent<Image>().color = nextA;

        Color previousA =previousButton.GetComponent<Image>().color;
        previousA.a = (currentSlideList.slideIndex > 0) ? 1 : .5f;
        previousButton.GetComponent<Image>().color = previousA;
    }
}
