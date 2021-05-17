using System.Collections;
using UnityEngine;

public class RoomObject : MonoBehaviour, IChangable
{
    [SerializeField]
    private string word;


    private Vector3 currentScale;

    [SerializeField]
    private AnimationClip appearing;
    [SerializeField]
    private AnimationClip disAppearing;
    private Animation anim;

    public string Word {
        get { return word;}
        set {word = value;}
    }

    public bool animated { 
        get; set; 
    }

    private bool inSpace = true;
    public bool InSpace { get => inSpace; }

    public Transform Transform => transform;

    public void SetChange(ChangeType changeType) {
        switch (changeType) {
            case ChangeType.missing:
                onMissing();
                break;
        }
    }
    public void RemoveChange(ChangeType changeType) {
        switch (changeType) {
            case ChangeType.missing:
                onAppearing();
                break;
        }
    }

    protected virtual void Awake() {
        anim = GetComponent<Animation>();
    }
    public void onAppearing()
    {
        gameObject.SetActive(true);
        StartCoroutine(Appearing());
    }

    public void onMissing()
    {
        currentScale = transform.localScale;
        StartCoroutine(Dissappearing());
    }
    public virtual IEnumerator Dissappearing() {

        if (animated) {
            if(anim != null && disAppearing != null) yield return StartCoroutine(playAnimation(disAppearing));
            else {
                AnimationCurve curve = AnimationCurve.EaseInOut(0,1,3,0);

                float timePassed = 0f;
                while (transform.localScale.x > 0) {
                    yield return new WaitForFixedUpdate();
                    timePassed += Time.deltaTime;
                    transform.localScale = currentScale * curve.Evaluate(timePassed);
                }
            }
        }
        transform.localScale = new Vector3(0,0,0);
        gameObject.SetActive(false);
        
    }


    public virtual IEnumerator Appearing() {
        if (animated) {
            Debug.Log(anim != null);
            if(anim != null && appearing != null) {
                transform.localScale = currentScale;
                yield return StartCoroutine(playAnimation(appearing));
            }
            else {
                AnimationCurve curve = AnimationCurve.EaseInOut(0,0,3,1);

                float timePassed = 0f;
                while (transform.localScale.x < currentScale.x) {
                    yield return new WaitForFixedUpdate();
                    timePassed += Time.deltaTime;
                    transform.localScale = currentScale * curve.Evaluate(timePassed);
                }
            }
        } 
        transform.localScale = currentScale;
    }
    public IEnumerator playAnimation(AnimationClip clip) {
        if (anim != null && clip != null) {
            Debug.Log("animate!");
            clip.legacy = true;
            anim.AddClip(clip, clip.name);
            anim.clip = clip;
            anim.Play();
            while (anim.IsPlaying(clip.name))
            {
                yield return null;
            }
            clip.legacy = false;
        }
    }
}