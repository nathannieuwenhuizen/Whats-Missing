using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A physical object inside the room that the player can interact with.
///It shows that it can be interacted by showing an outline arround the mesh.
///</summary>
public class InteractabelObject : RoomObject, IInteractable
{
    private bool focused;
    private Outline outline;
    public Outline Outline {
        get { 
            if (outline == null) outline = gameObject.AddComponent<Outline>();
            return outline;
        }
        set { outline = value; }
    }
    private float outlineWidth = 10;
    private float duration = .4f;
    private Coroutine focusedCoroutine;

    private bool outlineEnabled = true;
    public bool OutlineEnabled {
        get { return outlineEnabled;}
        set { outlineEnabled = value; }
    }

    public bool Interactable {get; set; } = true;

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
    public GameObject Gameobject { get => gameObject; }
    private Color focusedColor = Color.white;
    public Color FocusedColor { get => focusedColor; 
    set{
        focusedColor = value;
        Outline.OutlineColor = value;
    } }

    ///<summary>
    /// When the cursor hovers over the mesh of the object. It makes the outline appear.
    ///</summary>
    protected virtual void OnFocus() {
        if (!OutlineEnabled) return;

        Outline.enabled = true;
        if (focusedCoroutine != null) StopCoroutine(focusedCoroutine);
        focusedCoroutine = StartCoroutine(AnimateOutline(outlineWidth, false)); 
    }
    ///<summary>
    /// When the cursor unhovers the emesh of the pbject. Making the outline disappear.
    ///</summary>
    protected virtual void OnBlur() {
        if (!OutlineEnabled) return;
        
        if (outline != null) {
            if (focusedCoroutine != null) 
                StopCoroutine(focusedCoroutine);
            if (gameObject.activeSelf)
                focusedCoroutine = StartCoroutine(AnimateOutline(0, true)); 
        }
    }

    private IEnumerator AnimateOutline(float val, bool disableAfterAnimating = false) {
        float index = 0;
        float start = outline.OutlineWidth;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            outline.OutlineWidth = Mathf.Lerp(start, val, index / duration);
            yield return new WaitForEndOfFrame();
        }
        outline.OutlineWidth = val;
        if (disableAfterAnimating)
            outline.enabled = false;
    }

    public override void OnMissing()
    {
        base.OnMissing();
    }

    public virtual void Interact()
    {
    }
}
