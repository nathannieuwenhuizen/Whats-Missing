using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractabelObject : MonoBehaviour, IInteractable
{
    private bool focused;
    private Outline outline;
    private float outlineWidth = 10;
    private float duration = .4f;
    private Coroutine focusedCoroutine;

    public bool Focused { 
        get => focused; 
        set {
            if (value == focused) return;

            focused = value;
            if (focused) {
                OnFocus();
            } else {
                OnBlur();
            }
        } 
    }

    protected virtual void OnFocus() {
        if (outline == null) {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = true;
        if (focusedCoroutine != null) StopCoroutine(focusedCoroutine);
        focusedCoroutine = StartCoroutine(AnimateOutline(outlineWidth, false)); 
    }
    protected virtual void OnBlur() {
        if (outline != null) {
            if (focusedCoroutine != null) StopCoroutine(focusedCoroutine);
            focusedCoroutine = StartCoroutine(AnimateOutline(0, true)); 
        }
    }

    private IEnumerator AnimateOutline(float val, bool disableAfterAnimating = false) {
        float index = 0;
        float start = outline.OutlineWidth;
        while (index < duration) {
            index += Time.deltaTime;
            outline.OutlineWidth = Mathf.Lerp(start, val, index / duration);
            yield return new WaitForEndOfFrame();
        }
        outline.OutlineWidth = val;
        if (disableAfterAnimating)
            outline.enabled = false;
    }

    public virtual void Interact()
    {
        if (GetComponent<PickableRoomObject>() != null) {
            
        }
    }
}
