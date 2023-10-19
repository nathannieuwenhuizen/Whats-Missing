using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class AnimatedPopup : MonoBehaviour
{

    private Animator animator;
    protected CanvasGroup canvasGroup;
    private void OnEnable() {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void ShowAnimation(bool visible) {
        if (animator == null) animator = GetComponent<Animator>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
        animator.SetBool("show", visible);
    }
}
