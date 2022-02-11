using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A physical object inside the room that can be changed. 
///</summary>
public class RoomObject : RoomEntity
{
    [SerializeField]
    private FlippingAxis flippingAxis = FlippingAxis.up;
    protected RoomObjectEventSender eventSender;
    public RoomObjectEventSender EventSender {
        get { 
            if (eventSender == null) eventSender = new RoomObjectEventSender(this);
            return eventSender;
            }
        set { eventSender = value; }
    }
    public delegate void OnMissingEvent();

    public override float CurrentScale { 
        get { return transform.localScale.x; }
        set {
            transform.localScale = Vector3.one * value;
        } 
    }

    protected virtual void Awake() {
        Debug.Log("awake!!!");
        eventSender = new RoomObjectEventSender(this);
    }

    private void Update() {
        EventSender.Update();
    }


    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public override void OnAppearing()
    {
        gameObject.SetActive(true);
        EventSender.SendAppearingEvent();
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
                    StartCoroutine(mat.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
                }
                yield return new WaitForSeconds(animationDuration);
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
                    StartCoroutine(mat.AnimatingDissolveMaterial(1, 0, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
                }
                yield return new WaitForSeconds(animationDuration);
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
        EventSender.SendMissingEvent();

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
        normalScale = transform.localScale.x;
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        EventSender.Active = false;
    }


    #region  shrinking/enlarging

    public override IEnumerator AnimateShrinking()
    {
        yield return this.AnimatingRoomObjectScale( shrinkScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
        OnShrinkingFinish();
    }

    public override void OnShrinkingFinish()
    {
        CurrentScale = shrinkScale;
    }

    public override IEnumerator AnimateShrinkRevert()
    {
        yield return this.AnimatingRoomObjectScale( normalScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
        OnShrinkingRevertFinish();
    }

    public override void OnShrinkingRevertFinish()
    {
        CurrentScale = normalScale;
    }

    //enlarging
    public override IEnumerator AnimateEnlarging()
    {
        yield return this.AnimatingRoomObjectScale( largeScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
        OnEnlargingFinish();
    }

    public override void OnEnlargingFinish()
    {
        CurrentScale = largeScale;
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        yield return this.AnimatingRoomObjectScale( normalScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
        OnEnlargeRevertFinish();
    }

    public override void OnEnlargeRevertFinish()
    {
        CurrentScale = normalScale;
    }
    public void OnDestroy() {
        EventSender.SendMissingEvent();
    }

    #endregion

    #region  flipped
    public override void OnFlipped()
    {
        base.OnFlipped();
    }

    public override IEnumerator AnimateFlipping()
    {
        Bounce();
        yield return StartCoroutine(transform.AnimatingFlip(GetObjectHeight(), animationDuration, flippingAxis));
        yield return base.AnimateFlipping();
    }

    public override void OnFlippingFinish()
    {
        if (tempParent != null) {
            transform.parent = tempParent.transform.parent;
            Destroy(tempParent);
        }
        base.OnFlippingFinish();
    }

    public override void OnFlippingRevert()
    {
        base.OnFlippingRevert();
    }

    public override IEnumerator AnimateFlippingRevert()
    {
        Bounce();
        yield return StartCoroutine(transform.AnimatingFlip(GetObjectHeight(), animationDuration, flippingAxis));
        yield return base.AnimateFlippingRevert();
    }

    public override void OnFlippingRevertFinish()
    {
        if (tempParent != null) {
            transform.parent = tempParent.transform.parent;
            Destroy(tempParent);
        }
        base.OnFlippingRevertFinish();
    }

    public Renderer GetObjectHeight() {
        // if (GetComponent<Collider>() != null) {
        //     return GetComponent<Collider>();
        // } else if (GetComponentInChildren<Collider>() != null) {
        //     return GetComponentInChildren<Collider>();
        if (GetComponent<Renderer>() != null) {
            return GetComponent<Renderer>();
        } else if (GetComponentInChildren<Renderer>() != null) {
            return GetComponentInChildren<Renderer>();
        }

        return default(Renderer);
    }

    private GameObject tempParent;
    public void Bounce() {
        tempParent = new GameObject("tempParent");
        tempParent.transform.parent = transform.parent;
        transform.parent = tempParent.transform;
        StartCoroutine(tempParent.transform.AnimatingPosBounce(2f, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
    }


    #endregion
}