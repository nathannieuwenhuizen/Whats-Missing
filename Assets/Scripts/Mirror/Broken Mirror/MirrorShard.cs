using System.Collections;
using System.Collections.Generic;
using Custom.Rendering;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LetterCoords {
    [SerializeField]
    public string letterValue;
    [SerializeField]
    public Vector3 letterDelta;
    [HideInInspector]
    public Letter letter;
}

[System.Serializable]
public class ShardPositionInfo {
    public Transform fallPosition;
    public bool setAsParent = false;
    public bool useGravityAfterFall = true;
    public bool visibleAfterFalling = true;
    // public bool 
}

public class MirrorShard : PickableRoomObject
{
    public bool animated = true;
    public bool invokeBossReaction = true;
    public delegate void OnMirroShardPickEvent(MirrorShard _shard);
    public static OnMirroShardPickEvent OnPickedUp;
    private bool hasAlreadyBeenPickedUp = false;

    private float distanceToAttachShardToMirror = 8f;
    private float attachingDuration = 2f;
    [SerializeField]
    private LetterCoords[] letterCoords;

    private Vector3 startLocalPos;
    private Transform startParent;

    private Vector3 fallPositionStart = Vector3.zero;
    private Vector3 fallPositionMid;

    [SerializeField]
    private ShardPositionInfo positionInfo;
    [SerializeField]
    private AnimationCurve fallCurve;
    private float explosionuration = 5f;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private PlanarReflection planarReflection;

    [SerializeField]
    private ParticleSystem shineParticle;

    public PlanarReflection PlanarReflection {
        get { return planarReflection;}
    }

    private float grabAnimationDuration = 1f;

    public LetterCoords[] LetterCoords {
        get { return letterCoords;}
    }
    private bool attached = true;
    private bool animating = false;


    private bool focusedShard = false;
    public bool Focusedshard {
        get { return focusedShard;}
        set { focusedShard = value; }
    }

    ///<summary>
    /// If it is attached to the mirror or not
    ///</summary>
    public bool Attached {
        get { return attached;}
        set { 
            attached = value;
            Interactable = !value; 
            if (value) {
                DeactivateRigidBody();
                shineParticle.Stop();
            } else {
                ActivateRigidBody();
            }
            foreach(LetterCoords coords in letterCoords) coords.letter.Interactable = value;
            planarReflection.IsActive = !value;
            // planarReflection.IsActive = false;

        }
    }

    public bool lettersVisible {
        set { 
            Debug.Log("letters visible " + letterCoords.Length);
            foreach(LetterCoords coords in letterCoords) coords.letter.Visible = value;
        }
    }
    
    private BossMirror bossMirror;
    public BossMirror BossMirror {
        get { return bossMirror;}
        set { bossMirror = value; }
    }

    public int ShardIndex {
        get {return BossMirror.AmmountOfShardsAttached();}
    }

    private void Reset() {
        Word = "shard";
        AlternativeWords = new string[] {"shards"};
    }

    protected override void Awake() {
        base.Awake();
        planarReflection = GetComponent<PlanarReflection>();
        planarReflection.IsActive = false;
        HoldingOffset = new Vector3(0,-1.5f, 0);
        InteractableDistance = 10f;
    }
    

    private void Start() {
        grabSound = SFXFiles.mirorshard_grab;
        holdOrientation = HoldOrientation.shard;
        startLocalPos = transform.localPosition;
        startParent = transform.parent;
        holdingDistance = 5f;
        
        Attached = true;
        shineParticle.Stop();
        UpdateLetterPosition();
    }

    private void OnEnable() {
        RenderTexturePlane.OnTextureUpdating += UpdateTexture;
        ToggleVisibilty(true);
    }

    private void OnDisable() {
        RenderTexturePlane.OnTextureUpdating -= UpdateTexture;
        Outline.enabled = false;
        ToggleVisibilty(false);
    }

    private void UpdateTexture(RenderTexturePlane _plane) {
        if (planarReflection.IsActive) return;
        if (_plane != bossMirror.PlanarReflection) return;

        planarReflection.SetRenderTexture(_plane.reflectionCamera.targetTexture);
    }


    public void DisconnectedFromMirror() {
        transform.parent = BossMirror.transform.parent.parent;
        Attached = false;
        shineParticle.Play();
        ActivateRigidBody();
        SetBezierDestination(transform.position, positionInfo.fallPosition.position);
        StartCoroutine(Exploding());
    }
    private IEnumerator Exploding() {
        float index = 0;
        while (index < explosionuration && animated) {
            index += Time.deltaTime;
            transform.position = Extensions.CalculateQuadraticBezierPoint(fallCurve.Evaluate(index / explosionuration), fallPositionStart, fallPositionMid, positionInfo.fallPosition.position);
            if (rb) rb.velocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }
        SetToFallPosition();
    }
    public void SetToFallPosition() {
        transform.position = positionInfo.fallPosition.position;
        transform.rotation = positionInfo.fallPosition.rotation;
        if (positionInfo.setAsParent) {
            transform.SetParent(positionInfo.fallPosition);
        }
        rb = GetComponent<Rigidbody>();
        if (rb) {
            rb.useGravity = positionInfo.useGravityAfterFall;
            rb.isKinematic =  !positionInfo.useGravityAfterFall; //might change later
        }

        if (!positionInfo.visibleAfterFalling) {
            ToggleVisibilty(false);
        }
    }


    public void SetBezierDestination( Vector3 begin, Vector3 end) {
        fallPositionStart = begin;
        positionInfo.fallPosition.position = end;
        fallPositionMid = begin + ((end - begin) / 2f);
        fallPositionMid.y = begin.y + 150f;
    }
    private void Update() {
        if (!isVisible) return;

        if (!attached || animating) {
            UpdateLetterPosition();
        } 
        if (!animating && attached) {
            transform.localPosition = startLocalPos;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        
        UpdateOutline();
    }
    private float test = .1f;
    public Outline.Mode mode = Outline.Mode.OutlineVisible;
    private void UpdateOutline() {
    	if (!attached && !animating && focusedShard) {
            Outline.OutlineWidth = test;
            Outline.OutlineMode = mode;

        } else {
            Outline.OutlineWidth = 0;
            Outline.OutlineMode = mode;
        }
    }



    ///<summary>
    /// Updates the leer position to make it look like it is part of the parent
    ///</summary>w
    private void UpdateLetterPosition() {
        for (int i = 0; i < letterCoords.Length; i++) {
            Letter letter = letterCoords[i].letter;
            letter.transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);
            letter.transform.position = meshRenderer.transform.position + transform.TransformDirection(letterCoords[i].letterDelta);
        }
    }


    public override void Grab(Rigidbody connectedRigidBody)
    {
        base.Grab(connectedRigidBody); 
        AudioHandler.Instance?.Play3DSound(grabSound, transform);
        shineParticle.Stop();
        transform.parent  = null; // dont set ot the connected rigibdoy or else the update position will get laggy somehow
        if (!hasAlreadyBeenPickedUp) {
            hasAlreadyBeenPickedUp = true;
            OnPickedUp?.Invoke(this);
        }
        ToggleAllColliders(false);
    }

    private bool isVisible = true;

    public void ToggleVisibilty(bool _visible) {
        isVisible = _visible;
        for (int i = 0; i < letterCoords.Length; i++) {
            Letter letter = letterCoords[i].letter;
            if (letter != null) letter.gameObject.SetActive(_visible);
        }
        if (!_visible) Outline.OutlineWidth = 0;
        ToggleAllRenders(_visible);
    }

    public override void Release()
    {  
        if (!Attached && Vector3.Distance(transform.position, bossMirror.transform.position) < distanceToAttachShardToMirror) {
            base.Release();
            ReattachedToMirror();
        } else {
            shineParticle.Play();
        }
        ToggleAllColliders(false);
    }

    private void ToggleAllColliders(bool _val) {
        foreach(Collider col in GetComponentsInChildren<Collider>()){
            col.enabled = _val;
        }
    }
    private void ToggleAllRenders(bool _val) {
        meshRenderer.enabled = _val;
        foreach(Renderer col in GetComponentsInChildren<Renderer>()){
            col.enabled = _val;
        }
    }

    ///<summary>
    /// Reattaches a mirror shard towards the mirror
    ///</summary>
    public void ReattachedToMirror() {
        if (animating) return;
        animating = true;
        Interactable = false;
        transform.parent = startParent;
        DeactivateRigidBody();
        StartCoroutine(ReattachingToMirror());
    }

    private IEnumerator ReattachingToMirror() {
        StartCoroutine(transform.AnimatingLocalRotation(Quaternion.Euler(0,0,0), AnimationCurve.EaseInOut(0,0,1,1), attachingDuration));
        yield return StartCoroutine(transform.AnimatingLocalPos(startLocalPos, AnimationCurve.EaseInOut(0,0,1,1), attachingDuration));
        ToggleVisibilty(true);
        Attached = true;
        bossMirror.AttachMirrorShard(this);
        AudioHandler.Instance?.PlaySound(SFXFiles.mirorshard_place);
        animating = false;
    }

    private void OnDrawGizmosSelected() {
        if (letterCoords != null) {
            Gizmos.color = Color.white;
            for (int i = 0; i < letterCoords.Length; i++) {
                #if UNITY_EDITOR
                Handles.Label(meshRenderer.transform.position + transform.TransformDirection(letterCoords[i].letterDelta), letterCoords[i].letterValue);
                #endif
            }
        }
        if (positionInfo.fallPosition != null) {
            SetBezierDestination(fallPositionStart, positionInfo.fallPosition.position);
            DebugExtensions.DrawBezierCurve(fallPositionStart, fallPositionMid, positionInfo.fallPosition.position);
            #if UNITY_EDITOR
            Handles.Label(positionInfo.fallPosition.position, gameObject.name);
            #endif
        }
    }

    private Coroutine shakeCoroutine;
    public void Shake(float duration) {
        shakeCoroutine = StartCoroutine(Shaking(duration));
    }
    IEnumerator Shaking(float duration) {
        yield return new WaitForSeconds(Random.Range(0,.5f));
        yield return transform.ShakeLocalYPos(.05f, 2f, duration);
    }
    public void StopShake() {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            0,
            transform.localPosition.z
        );
        StopCoroutine(shakeCoroutine);
    }
    public override bool CanBeReleased()
    {
        return !Attached && Vector3.Distance(transform.position, bossMirror.transform.position) < distanceToAttachShardToMirror;
    }
}
