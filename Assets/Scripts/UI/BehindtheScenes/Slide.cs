using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Slide : AnimatedPopup
{
    [SerializeField] string subText;
    public string  SubText {
         get { return subText ; }
    }

    
    public override void ShowAnimation(bool visible)
    {
        base.ShowAnimation(visible);
        //
    }
    private void Start() {
        canvasGroup.alpha = 0;
        Show();
        Hide();
    }

    public void Show() {
        ShowAnimation(true);
        if (GetComponentInChildren<VideoPlayer>() != null) {
            Debug.Log("should now play!");
            GetComponentInChildren<VideoPlayer>().Play();
        }
    }

    public void Hide() {
        ShowAnimation(false);
        if (GetComponentInChildren<VideoPlayer>() != null) {
            Debug.Log("should now stop!");
            GetComponentInChildren<VideoPlayer>().Stop();
        }
    }
}
