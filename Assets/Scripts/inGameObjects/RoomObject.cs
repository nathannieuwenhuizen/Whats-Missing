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
    ///<summary>
    /// Is true when the player is inside the same room as the object
    ///</summary>
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
                onMissing();
                break;
        }
    }
    public void RemoveChange(Change change) {
        switch (change.television.changeType) {
            case ChangeType.missing:
                onAppearing();
                break;
        }
    }

    #region  missing changes
    ///<summary>
    /// Fires when the object starts to appear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void onAppearing()
    {
        gameObject.SetActive(true);
        if (Animated) {
            StartCoroutine(AnimateAppearing());
        } else {
            onAppearingFinish();
        }
    }

    ///<summary>
    /// Fires when the object starts to disappear, here it will also chick if it has to animate or not.
    ///</summary>
    public virtual void onMissing()
    {
        currentScale = transform.localScale;
        if (Animated) {
            StartCoroutine(AnimateMissing());
        } else {
            onMissingFinish();
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
            case MissingChangeEffect.animation:
                // if(anim != null && disAppearing != null && Time.timeScale != 0) {
                //     yield return StartCoroutine(playAnimation(disAppearing));
                // } else {
                //     Debug.LogWarning("Animation has not been asigned or time is 0");
                // }
                break;
        }
        onMissingFinish();
    }

    private Material[] getMaterials() {
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
            case MissingChangeEffect.animation:
                // if(anim != null && appearing != null && Time.timeScale != 0) {
                //     yield return StartCoroutine(playAnimation(appearing));
                // } else {
                //     Debug.LogWarning("Animation has not been asigned or time is 0");
                // }
                break;
        }
        onAppearingFinish();
    }

    

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be missing.
    ///</summary>
    public virtual void onMissingFinish()
    {
        gameObject.SetActive(false);
    }

    ///<summary>
    /// Function that fires when the animation has finished. It makes the snap changes the object needs to be appearing.
    ///</summary>
    public virtual void onAppearingFinish()
    {
        transform.localScale = currentScale;
    }

    #endregion

    // public IEnumerator playAnimation(AnimationClip clip) {
    //     if (anim != null && clip != null) {
    //         Debug.Log("animate!");
    //         clip.legacy = true;
    //         anim.AddClip(clip, clip.name);
    //         anim.clip = clip;
    //         anim.Play();
    //         while (anim.IsPlaying(clip.name))
    //         {
    //             yield return new WaitForEndOfFrame();
    //         }
    //         clip.legacy = false;
    //     }
    // }

    public virtual void OnRoomEnter()
    {
        inSpace = true;
    }

    public virtual void OnRoomLeave()
    {
        inSpace = false;
    }
}