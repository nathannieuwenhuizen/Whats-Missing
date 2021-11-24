using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A physical object inside the room that can be changed. 
///</summary>
public class RoomObject : RoomEntity
{

    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public override void OnAppearing()
    {
        gameObject.SetActive(true);
        base.OnAppearing();
    }


    ///<summary>
    /// Coroutine that animates the roomobject into oblivion. 
    ///</summary>
    public override IEnumerator AnimateMissing() {
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
    public override IEnumerator AnimateAppearing() {

        switch(MissingChangeEffect) {
            case MissingChangeEffect.none:
            break;
            case MissingChangeEffect.scale:
                transform.localScale = Vector3.zero;
                AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
                yield return transform.AnimatingScale(startMissingScale, curve, .5f);
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
    public override void OnMissingFinish()
    {
        base.OnMissingFinish();
        gameObject.SetActive(false);
    }

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be appearing.
    ///</summary>
    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        transform.localScale = startMissingScale;
    }

    #endregion


    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        normalScale = transform.localScale;
    }


    #region  shrinking/enlarging
    private Vector3 normalScale;

    public override IEnumerator AnimateShrinking()
    {
        yield return transform.AnimatingScale(normalScale * .5f, AnimationCurve.EaseInOut(0,0,1,1), 3f);
        OnShrinkingFinish();
    }

    public override void OnShrinkingFinish()
    {
        transform.localScale = normalScale * .5f;
    }

    public override IEnumerator AnimateShrinkRevert()
    {
        yield return transform.AnimatingScale(normalScale, AnimationCurve.EaseInOut(0,0,1,1), 3f);
        OnShrinkingRevertFinish();
    }

    public override void OnShrinkingRevertFinish()
    {
        transform.localScale = normalScale;
    }
    #endregion
}