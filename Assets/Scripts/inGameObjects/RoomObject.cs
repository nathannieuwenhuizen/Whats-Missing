using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A physical object inside the room that can be changed. and has thus a transform that will be changed (scale or rotation)
///</summary>
public class RoomObject : RoomEntity
{
    public static int PARTICLES_ACTIVE = 0;
    ///<summary>
    /// To make sure the game performance wont suffer because too many objects are animating
    ///</summary>
    public static int MAX_PARTICLES = 8;
    
    private bool active = true;

    [Header("room object info")]

    [SerializeField]
    protected FlippingAxis flippingAxis = FlippingAxis.up;
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
        get { return Mathf.Abs(transform.localScale.x); }
        set {
            transform.localScale = new Vector3(
                transform.localScale.x >= 0 ? 1 : -1,
                transform.localScale.y >= 0 ? 1 : -1,
                transform.localScale.z >= 0 ? 1 : -1) * value;
        } 
    }

    [SerializeField]
    private bool affectedByPotions = true;
    public bool AffectedByPotions {
        get { return affectedByPotions;}
        set { affectedByPotions = value; }
    }

    protected virtual void Awake() {
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
        // gameObject.SetActive(true);
        SetActive(true);
        EventSender.SendAppearingEvent();
        base.OnAppearing();
    }


    ///<summary>
    /// Coroutine that animates the roomobject into oblivion. 
    ///</summary>
    public override IEnumerator AnimateMissing() {
        switch(MissingChangeEffect) {
            case MissingChangeEffect.none:
                yield return new WaitForSeconds(animationDuration);
            break;
            case MissingChangeEffect.scale:
                AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
                yield return transform.AnimatingLocalScale(Vector3.zero, curve, .5f);
                transform.localScale = Vector3.zero;
            break;
            case MissingChangeEffect.dissolve:
                foreach (Material mat in getMaterials()) StartCoroutine(mat.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
                ShowDisovleParticles();
                Debug.Log("disappear");
                if (currentChange.changeCausation == ChangeCausation.potion)
                    AudioHandler.Instance?.Play3DSound(SFXFiles.object_disappear, transform, 1f);
                yield return new WaitForSeconds(animationDuration);
            break;
        }
        OnMissingFinish();
    }
    public void ShowDisovleParticles(bool _flamableParticlesEnabled = false) {
        if (PARTICLES_ACTIVE > MAX_PARTICLES) return;
        PARTICLES_ACTIVE++;
        Debug.Log("show particles" + PARTICLES_ACTIVE);
        foreach (MeshRenderer item in GetComponentsInChildren<MeshRenderer>()) StartCoroutine(DisolvingParticles(item, _flamableParticlesEnabled));
    }

    protected virtual Material[] getMaterials() {
        List<Material> materials = new List<Material>();
        foreach (MeshRenderer item in GetComponentsInChildren<MeshRenderer>())
        {
            Debug.Log("add material" + item.name);
            materials.AddRange(item.materials);
        } 
        // if (!materials.Contains(GetComponent<MeshRenderer>().material))
        // materials.Add(GetComponent<MeshRenderer>().material);
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
                yield return this.AnimatingRoomObjectScale(DesiredScale(), AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
            break;
            case MissingChangeEffect.dissolve:
                foreach (Material mat in getMaterials()) StartCoroutine(mat.AnimatingDissolveMaterial(1, 0, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
                ShowDisovleParticles();
                if (currentChange.changeCausation == ChangeCausation.potion)
                    AudioHandler.Instance?.Play3DSound(SFXFiles.object_reappear, transform, 1f);
                yield return new WaitForSeconds(animationDuration);
            break;
        }
        OnAppearingFinish();
    }

    private IEnumerator DisolvingParticles(MeshRenderer renderer, bool _flamableParticlesEnabled) {
        ParticleSystem particleSystem = Instantiate(Resources.Load<GameObject>(
            _flamableParticlesEnabled ? "RoomPrefabs/burn_particle" : "RoomPrefabs/dissolve_particle"
            )).GetComponent<ParticleSystem>();
        particleSystem.transform.position = renderer.transform.position;
        particleSystem.transform.rotation = renderer.transform.rotation;
        particleSystem.transform.localScale = renderer.transform.localScale;
        // particleSystem.shape.shapeType = ParticleSystemShapeType.MeshRenderer;
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.meshRenderer = renderer;
        shape.shapeType = ParticleSystemShapeType.MeshRenderer;
        shape.mesh = renderer.GetComponent<MeshFilter>().mesh;

        particleSystem.Play();
        float maxEmission = 100;
        float index = 0;
        while(index < animationDuration * .5f) {
            index += Time.deltaTime;
            particleSystem.emissionRate = Mathf.Sin((index/ animationDuration *.5f) * Mathf.PI) * maxEmission;
            yield return new WaitForEndOfFrame();
        }
        particleSystem.Stop();
        PARTICLES_ACTIVE--;
        Destroy(particleSystem.gameObject, particleSystem.main.startLifetime.Evaluate(0));
    }
    

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be missing.
    ///</summary>
    public override void OnMissingFinish()
    {
        base.OnMissingFinish();
        // gameObject.SetActive(false);
        SetActive(false);
        EventSender.SendMissingEvent();

    }

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be appearing.
    ///</summary>
    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        switch(MissingChangeEffect) {
            case MissingChangeEffect.none:
            break;
            case MissingChangeEffect.scale:
            break;
            case MissingChangeEffect.dissolve:
                foreach (Material mat in getMaterials()) mat.SetFloat("Dissolve", 0);
            break;
        }
        CurrentScale = DesiredScale();
    }

    ///<summary>
    /// The desired scale the object wants to have based on its state. Used in the appearing animation calls because there the scale might be temporary zero.
    ///</summary>
    private float DesiredScale() {
        return (IsShrinked ? ShrinkScale : (IsEnlarged ? LargeScale : NormalScale));
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
        Debug.Log("shrinking animate");
        AudioHandler.Instance?.Play3DSound(SFXFiles.object_shrink, transform, 1f);
        yield return this.AnimatingRoomObjectScale( shrinkScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration);
        OnShrinkingFinish();
    }

    public override void OnShrinkingFinish()
    {
        Debug.Log("shrinking finish");
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
    public virtual void OnDestroy() {
        if (Room != null) Room.allObjects?.Remove(this);

        if (InSpace) EventSender.SendMissingEvent();
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

    ///<summary>
    /// Enables all the components inside a gameobject and its children, only works on renderers, colliders and particle systems.
    ///</summary>
    private List<Renderer> disabledRenderers = new List<Renderer>();
    public virtual void SetActive(bool _active) {
        if (active == _active) return;
        active = _active;
        if (!_active) {
            disabledRenderers = gameObject.SetAllComponentsActive<Renderer>(_active, null);
        } 
        else {
            gameObject.SetAllComponentsActive<Renderer>(_active, disabledRenderers);
            disabledRenderers = new List<Renderer>();
        }
        gameObject.SetAllComponentsActive<Collider>(_active, null);
        gameObject.SetAllComponentsActive<Rigidbody>(_active, null);
        gameObject.SetAllComponentsActive<ParticleSystem>(_active, null);
        gameObject.SetAllComponentsActive<Light>(_active, null);
        
    }

    public Renderer GetObjectHeight() {
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