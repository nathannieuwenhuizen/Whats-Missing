using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spirit : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ParticleSystem[] particles;

    private SFXInstance spiritSound;

    public void ToggleParticles(bool val) {
        foreach(ParticleSystem ps in particles) {
            if (val) {
                ps.Play();
            } else {
                ps.Stop();
            }
        }
    }

    public void PlayIntroScrene() {

        spiritSound = AudioHandler.Instance.Player3DSound(SFXFiles.evil_spirit, transform, 0f, 1f, true, true);
        StartCoroutine(spiritSound.AudioSource.FadeSFXVolume(.3f, AnimationCurve.EaseInOut(0,0,1,1), .2f));

        ToggleParticles(true);
        StartCoroutine(playAnimation("intro", 5f, () => {
            ToggleParticles(false);
            StartCoroutine(spiritSound.AudioSource.FadeSFXVolume(0, AnimationCurve.EaseInOut(0,0,1,1), 3f));
            Destroy(gameObject, 5f);
            Destroy(spiritSound.AudioSource.gameObject, 5f);
        }));
    }

    private IEnumerator playAnimation(string keyword, float animationDuration, Action callback) {
        animator.SetTrigger(keyword);
        yield return new WaitForSeconds(animationDuration);
        callback();
    }
}
