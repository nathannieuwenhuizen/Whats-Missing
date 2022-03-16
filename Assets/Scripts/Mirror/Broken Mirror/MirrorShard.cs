using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LetterCoords {
    [SerializeField]
    public string letterValue;
    [SerializeField]
    public Vector3 letterDelta;
    public Letter letter;
}

public class MirrorShard : PickableRoomObject
{

    private float attachingDuration = 2f;
    [SerializeField]
    private LetterCoords[] letterCoords;
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
            foreach(LetterCoords coords in letterCoords) coords.letter.Interactable = value;
        }
    }
    
    [HideInInspector]
    public BossMirror bossMirror;

    private void Reset() {
        Word = "shard";
        AlternativeWords = new string[] {"shards"};
    }
    

    private void Start() {
        Attached = true;
    }


    public void DisconnectedFromMirror() {
        transform.parent = bossMirror.transform.parent;
        Attached = false;
        ActivateRigidBody();
        rb.velocity = Extensions.RandomVector(40f);
    }

    private void Update() {
        if (!attached) {
            UpdateLetterPosition();
        }
    }


    ///<summary>
    /// Updates the leer position to make it look like it is part of the parent
    ///</summary>
    private void UpdateLetterPosition() {
        for (int i = 0; i < letterCoords.Length; i++) {
            letterCoords[i].letter.transform.rotation = transform.rotation;
            letterCoords[i].letter.transform.position = transform.position + transform.InverseTransformDirection(letterCoords[i].letterDelta);
        }
    }

    ///<summary>
    /// Reattaches a mirror shard towards the mirror
    ///</summary>
    public void ReattachedToMirror() {
        if (animating) return;
        animating = true;
        Interactable = false;
        transform.parent = bossMirror.transform;
        StartCoroutine(ReattachingToMirror());
    }

    private IEnumerator ReattachingToMirror() {
        StartCoroutine(transform.AnimatingLocalRotation(Quaternion.Euler(0,0,0), AnimationCurve.EaseInOut(0,0,1,1), attachingDuration));
        yield return StartCoroutine(transform.AnimatingLocalPos(Vector3.zero, AnimationCurve.EaseInOut(0,0,1,1), attachingDuration));
        Attached = true;
        animating = false;
    }

    private void OnDrawGizmos() {
        if (letterCoords != null) {
            Gizmos.color = Color.white;
            for (int i = 0; i < letterCoords.Length; i++) {
                #if UNITY_EDITOR
                Handles.Label(transform.position + transform.TransformDirection(letterCoords[i].letterDelta), letterCoords[i].letterValue);
                #endif
                Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(letterCoords[i].letterDelta), .5f);
            }
        }

    }

}
