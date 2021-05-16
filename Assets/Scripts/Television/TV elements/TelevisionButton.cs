using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelevisionButton : MonoBehaviour
{
    public static TelevisionButton SELECTED_BUTTON;
    protected bool canBeClicked = true;

    public virtual void OnHover() {
        Debug.Log("on hover");
        if (!canBeClicked) return;
        TelevisionButton.SELECTED_BUTTON = this;
    }
    public virtual void OnUnhover() {
        if (!canBeClicked) return;
        if (TelevisionButton.SELECTED_BUTTON == this) TelevisionButton.SELECTED_BUTTON = null;
    }

}
