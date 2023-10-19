using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideList : MonoBehaviour
{
    public int slideIndex = 0;

    [SerializeField] private Slide[] slides;
    public int ammountofSlides {
         get { return slides.Length; }
    }

    public Slide CurrentSlide  {
         get { return slides[slideIndex]; }
    }

    private void Awake() {
        foreach(Slide slide in slides) {
            slide.gameObject.SetActive(false);
            slide.Show();
            slide.Hide();
        }
    }

    public void OpenList() {
        slideIndex = 0;
        StartCoroutine(ShowSlide());
    }

    public void CloseList() {
        slides[slideIndex].Hide();
    }

    public void NextSlide() {
        slides[slideIndex].Hide();
        slides[slideIndex].gameObject.SetActive(false);
        slideIndex++;
        StartCoroutine(ShowSlide());
    }
    private IEnumerator ShowSlide() {
        slides[slideIndex].gameObject.SetActive(true);
        yield return new WaitForSeconds(.1f);
        slides[slideIndex].Show();
    }

    public void PreviousSlide() {
        slides[slideIndex].Hide();
        slides[slideIndex].gameObject.SetActive(false);
        slideIndex--;
        StartCoroutine(ShowSlide());
    }

}
