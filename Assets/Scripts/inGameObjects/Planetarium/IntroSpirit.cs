using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSpirit : MonoBehaviour
{
    [SerializeField] private GameObject spirit1;
    [SerializeField] private GameObject spirit2;
    [SerializeField] private Transform rippleParticle;
    private bool spirit1Enabled = false;
    private bool spirit2Enabled = false;

    private void Awake() {
        spirit1.SetActive(false);
        spirit2.SetActive(false);
        // rippleParticle.gameObject.SetActive(false);

    }

    public void Spirit1Animation() {
        if (spirit1Enabled) return;
        spirit1Enabled = true;
        StartCoroutine(Spirit1Animating());
    }
    private IEnumerator Spirit1Animating() {
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpiritSFX(.4f, 1f));
        spirit1.SetActive(true);
        Destroy(spirit1, 8f);
    }
    public void Spirit2Animation() {
        if (spirit2Enabled) return;
        spirit2Enabled = true;

        StartCoroutine(SpiritSFX(.2f, 1f));
        StartCoroutine(CheckRippleParticle());
        if (spirit1 != null) spirit1.SetActive(false);
        spirit2.SetActive(true);
        Destroy(spirit2, 8f);
    }

    private IEnumerator SpiritSFX(float volume = .4f, float totalDuration = 1f) {
        SFXInstance instance = AudioHandler.Instance.PlaySound(SFXFiles.boss_general_talking, 0);
        float fadeDuration = 1f;
        float timePassed = 0f;
        while (timePassed < fadeDuration) {
            yield return new WaitForEndOfFrame();
            instance.Volume = Mathf.Lerp(0f, volume, timePassed/fadeDuration);
            timePassed += Time.deltaTime;
       }
        yield return new WaitForSeconds(totalDuration);
        fadeDuration = 1f;
        timePassed = 0f;
        while (timePassed < fadeDuration) {
            yield return new WaitForEndOfFrame();
            instance.Volume = Mathf.Lerp(volume, 0f, timePassed/fadeDuration);
            timePassed += Time.deltaTime;
       }
       instance.Stop();
    }

    private  IEnumerator CheckRippleParticle(float duration = 7f) {
    bool emitted = false;
       float timePassed = 0f;
        while (timePassed < duration && !emitted) {
           yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            if (spirit2 != null)
            Debug.Log("distance = " + Vector3.Distance(spirit2.transform.position, rippleParticle.position));
            if (Vector3.Distance(spirit2.transform.position, rippleParticle.position) < 3.5f) {
                emitted = true;
                rippleParticle.gameObject.SetActive(true);
            }

       }
    }
}
