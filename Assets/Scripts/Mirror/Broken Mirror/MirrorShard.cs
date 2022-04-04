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

public class MirrorShard : PickableRoomObject
{

    private float distanceToAttachShardToMirror = 8f;
    private float attachingDuration = 2f;
    [SerializeField]
    private LetterCoords[] letterCoords;

    private Vector3 startLocalPos;
    private Transform startParent;

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
            } else {
                ActivateRigidBody();
            }
            foreach(LetterCoords coords in letterCoords) coords.letter.Interactable = value;
            planarReflection.IsActive = !value;

        }
    }
    
    private BossMirror bossMirror;
    public BossMirror BossMirror {
        get { return bossMirror;}
        set { bossMirror = value; }
    }

    private void Reset() {
        Word = "shard";
        AlternativeWords = new string[] {"shards"};
    }

    protected override void Awake() {
        base.Awake();
        planarReflection = GetComponent<PlanarReflection>();
        planarReflection.IsActive = false;
    }
    

    private void Start() {
        looksWhenGrabbed = true;
        startLocalPos =transform.localPosition;
        startParent = transform.parent;
        holdingDistance = 5f;
        Attached = true;
        shineParticle.Stop();
        UpdateLetterPosition();
    }

    private void OnEnable() {
        RenderTexturePlane.OnTextureUpdating += UpdateTexture;
    }

    private void OnDisable() {
        RenderTexturePlane.OnTextureUpdating -= UpdateTexture;
    }

    private void UpdateTexture(RenderTexturePlane _plane) {
        if (planarReflection.IsActive) return;
        if (_plane != bossMirror.PlanarReflection) return;

        planarReflection.SetRenderTexture(_plane.reflectionCamera.targetTexture);
    }


    public void DisconnectedFromMirror() {
        transform.parent = BossMirror.transform.parent;
        Attached = false;
        ActivateRigidBody();
        shineParticle.Play();
        rb.velocity = Vector3.zero;
    }

    private void Update() {
        if (!attached || animating) {
            UpdateLetterPosition();
        } 
        if (!animating && attached) {
            transform.localPosition = startLocalPos;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }


    ///<summary>
    /// Updates the leer position to make it look like it is part of the parent
    ///</summary>
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
        shineParticle.Stop();
    }

    public override void Release()
    {  
        base.Release();
        if (!Attached && Vector3.Distance(transform.position, bossMirror.transform.position) < distanceToAttachShardToMirror) {
            ReattachedToMirror();
        } else {
            shineParticle.Play();
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
        Attached = true;
        animating = false;
    }

    private void OnDrawGizmos() {
        if (letterCoords != null) {
            Gizmos.color = Color.white;
            for (int i = 0; i < letterCoords.Length; i++) {
                #if UNITY_EDITOR
                Handles.Label(meshRenderer.transform.position + transform.TransformDirection(letterCoords[i].letterDelta), letterCoords[i].letterValue);
                #endif
                // Gizmos.DrawWireSphere(meshRenderer.transform.position + transform.TransformDirection(letterCoords[i].letterDelta), .5f);
            }
        }

    }

}
