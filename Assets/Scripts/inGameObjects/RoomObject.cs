using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A physical object inside the room that can be changed. 
///</summary>
public class RoomObject : MonoBehaviour, IChangable, IRoomObject
{
    [SerializeField]
    private string word;

    [SerializeField]
    private string[] alternateWords;

    private Vector3 currentScale;

    public string Word {
        get { return word;}
        set {word = value;}
    }

    public bool Animated { 
        get; set; 
    }

    private bool inSpace = true;
    public bool InSpace { get => inSpace; }

    public Transform Transform => transform;

    public string[] AlternativeWords { get => alternateWords; set => alternateWords = value; }

    [SerializeField]
    private MissingChangeEffect missingChangeEffect = MissingChangeEffect.scale;
    public MissingChangeEffect MissingChangeEffect => missingChangeEffect;

    public int id {get; set; }

    public void AddChange(Change change) {
        switch (change.television.changeType) {
            case ChangeType.missing:
                OnMissing();
                break;
            case ChangeType.tooSmall:
                OnShrinking();
                break;
        }    
    }
    public void RemoveChange(Change change) {
        switch (change.television.changeType) {
            case ChangeType.missing:
                OnAppearing();
                break;
            case ChangeType.tooSmall:
                OnShrinkRevert();
                break;
        }
    }

    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void OnAppearing()
    {
        gameObject.SetActive(true);
        if (Animated) {
            StartCoroutine(AnimateAppearing());
        } else {
            OnAppearingFinish();
        }
    }

    ///<summary>
    /// Fires when the object starts to disappear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void OnMissing()
    {
        currentScale = transform.localScale;
        if (Animated) {
            StartCoroutine(AnimateMissing());
        } else {
            OnMissingFinish();
        }
    }

    ///<summary>
    /// Coroutine that animates the roomobject into oblivion. 
    ///</summary>
    public virtual IEnumerator AnimateMissing() {


        switch(MissingChangeEffect) {
            case MissingChangeEffect.none:
            break;
            case MissingChangeEffect.scale:
                AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
                yield return transform.AnimatingScale(Vector3.zero, curve, .5f);
                transform.localScale = Vector3.zero;
            break;
            case MissingChangeEffect.dissolve:
                foreach (Material mat in getMaterials())
                {
                    StartCoroutine(mat.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), 3f));
                }
                yield return new WaitForSeconds(3f);
            break;
        }
        OnMissingFinish();
    }

    protected virtual Material[] getMaterials() {
        List<Material> materials = new List<Material>();
        foreach (MeshRenderer item in GetComponentsInChildren<MeshRenderer>())
        {
            materials.AddRange(item.materials);
        } 
        return materials.ToArray();
    }

    ///<summary>
    /// Coroutine that animates the roomobject into existing. 
    ///</summary>
    public virtual IEnumerator AnimateAppearing() {

        switch(MissingChangeEffect) {
            case MissingChangeEffect.none:
            break;
            case MissingChangeEffect.scale:
                transform.localScale = Vector3.zero;
                AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
                yield return transform.AnimatingScale(currentScale, curve, .5f);
            break;
            case MissingChangeEffect.dissolve:
                foreach (Material mat in getMaterials())
                {
                    StartCoroutine(mat.AnimatingDissolveMaterial(1, 0, AnimationCurve.EaseInOut(0,0,1,1), 3f));
                }
                yield return new WaitForSeconds(3f);
            break;
        }
        OnAppearingFinish();
    }

    

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be missing.
    ///</summary>
    public virtual void OnMissingFinish()
    {
        gameObject.SetActive(false);
    }

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be appearing.
    ///</summary>
    public virtual void OnAppearingFinish()
    {
        transform.localScale = currentScale;
    }

    #endregion


    public virtual void OnRoomEnter()
    {
        normalScale = transform.localScale;
        inSpace = true;
    }

    public virtual void OnRoomLeave()
    {
        inSpace = false;
    }

    #region  shrinking/enlarging
    private Vector3 normalScale;
    public void OnShrinking()
    {
        if (Animated) {
            StartCoroutine(AnimateShrinking());
        } else {
            OnShrinkingFinish();
        }
    }

    public IEnumerator AnimateShrinking()
    {
        yield return transform.AnimatingScale(normalScale * .5f, AnimationCurve.EaseInOut(0,0,1,1), 3f);
        OnShrinkingFinish();
    }

    public void OnShrinkingFinish()
    {
        Debug.Log("on shrink finish");
        transform.localScale = normalScale * .5f;
    }

    public void OnShrinkRevert()
    {
        if (Animated) {
            StartCoroutine(AnimateShrinkRevert());
        } else {
            OnShrinkingRevertFinish();
        }
    }

    public IEnumerator AnimateShrinkRevert()
    {
        yield return transform.AnimatingScale(normalScale, AnimationCurve.EaseInOut(0,0,1,1), 3f);
        OnShrinkingRevertFinish();
    }

    public void OnShrinkingRevertFinish()
    {
        transform.localScale = normalScale;
    }
    #endregion
}