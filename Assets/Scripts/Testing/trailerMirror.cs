using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using TMPro;

public class trailerMirror : MonoBehaviour
{
    private TMP_Text text;
    private TextAnimatorPlayer animationPlayer;


    private CanvasGroup group;

    private Coroutine wordCoroutine;
    private void Awake() {
        animationPlayer = GetComponentInChildren<TextAnimatorPlayer>();
        text = GetComponent<TMP_Text>();
        group = GetComponent<CanvasGroup>();
    }

    public IEnumerator Test() {
        // animationPlayer.ShowText("Challenge your mind!");
        yield return new WaitForSeconds(2.5f);
        animationPlayer.ShowText("Books can be missing... \n\nLamps can be too big...\n\nEven the colors are missing?");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            StartCoroutine(Test());
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            animationPlayer.StartDisappearingText();
        }
    }

}
