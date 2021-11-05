using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlchemyItem : InteractabelObject
{
    private Vector3 startPos;
    [SerializeField]
    private float rotationSpeed = 2f;
    [SerializeField]
    private float sineWaveAmplitude = .5f;
    [SerializeField]
    private float sineWaveSpeed = 1f;
    [SerializeField]
    private BoxCollider coll;

    [SerializeField]
    private Room room;

    public delegate void AlchemyAction();
    public static event AlchemyAction OnPickingAlchemyItem;
    public static event AlchemyAction OnAlchemyEndScene;
    public delegate void AlchemyPulse(Transform origin);
    public static event AlchemyPulse OnPulsing;

    private Bloom colorAdjustments;
    private ChromaticAberration chromaticAttribution;


    private bool animate = true;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(Pulse());
    }

    private IEnumerator Pulse() {
        while(animate) {
            yield return new WaitForSeconds(3f);
                Debug.Log("pulse! " + Vector3.Distance(room.Player.transform.position, transform.position));
            if (Vector3.Distance(room.Player.transform.position, transform.position) < 10f && room.Player.transform.position.y < transform.position.y) {
                OnPulsing?.Invoke(transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            // StartCoroutine(room.Player.Camera.transform.Shake(5f, 10, 4));
        } 
        if (!animate) return;

        transform.Rotate(new Vector3(0,rotationSpeed,0));
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time *sineWaveSpeed) * sineWaveAmplitude, 0);
    }
    public override void Interact()
    {
        base.Interact();
        animate = false;
        OnPickingAlchemyItem?.Invoke();
        room.Player.PlayCutSceneAnimation("takingItem", true);
        coll.enabled = false;
        StartCoroutine(AudioHandler.Instance.FadeVolume(AudioHandler.Instance.MusicSource, AudioHandler.Instance.MusicSource.volume, 0, 2f));
        StartCoroutine(GetPickedUp());
    }


    private IEnumerator GetPickedUp() {
        
        yield return new WaitForSeconds(2.3f);
        AudioHandler.Instance.PlaySound(SFXFiles.grab_book, .2f, .5f);

        float index = 0;
        float duration = .5f;
        Vector3 start = transform.position;
        Quaternion rotStart = transform.rotation;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (index < duration) {
            index += Time.deltaTime;
            transform.position = Vector3.Lerp(start, room.Player.HandsPosition.position, curve.Evaluate(index / duration) );
            transform.rotation = Quaternion.Slerp(rotStart, room.Player.HandsPosition.rotation, curve.Evaluate(index / duration) );
            yield return new WaitForEndOfFrame();
        }
        transform.SetParent(room.Player.HandsPosition);
        transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(.5f);
        AudioHandler.Instance.PlaySound(SFXFiles.woosh, 1f);
        yield return new WaitForSeconds(1f);
        AudioHandler.Instance.PlaySound(SFXFiles.rumble_ground, 1f, .2f);

        StartCoroutine(room.Player.Camera.transform.Shake(5f, 10, 7));
        StartCoroutine(AnimateBloomIntensity(4f, 100));
        StartCoroutine(AnimateChromaticAttribution(1f, 1));
        yield return new WaitForSeconds(5f);
        AudioHandler.Instance.StopSound(SFXFiles.rumble_ground);

        OnAlchemyEndScene?.Invoke();
    }
    public IEnumerator AnimateBloomIntensity(float animationDuration, float end) {
        float start = bloomIntensity;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.deltaTime;
            bloomIntensity = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        bloomIntensity = end;
    }
    public IEnumerator AnimateChromaticAttribution(float animationDuration, float end) {
        float start = ChromaticIntesity;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.deltaTime;
            ChromaticIntesity = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame(); 
        }
        ChromaticIntesity = end;
    }


    public float bloomIntensity {
        set {
            if (colorAdjustments == null)
                room.Player.Volume.profile.TryGet<Bloom>(out colorAdjustments);
            colorAdjustments.intensity.value = value;
        }
        get {
            if (colorAdjustments == null)
                room.Player.Volume.profile.TryGet<Bloom>(out colorAdjustments);
            return colorAdjustments.intensity.value;
        }
    }
    public float ChromaticIntesity {
        set {
            if (chromaticAttribution == null)
                room.Player.Volume.profile.TryGet<ChromaticAberration>(out chromaticAttribution);
            chromaticAttribution.intensity.value = value;
        }
        get {
            if (chromaticAttribution == null)
                room.Player.Volume.profile.TryGet<ChromaticAberration>(out chromaticAttribution);
            return chromaticAttribution.intensity.value;
        }
    }

}
