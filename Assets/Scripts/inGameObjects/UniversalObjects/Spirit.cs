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

    private bool behindPlayerPlayed = false;
    private bool intoMirrorPlayed = false;

    public void EnableSpirit(bool val) {
        if (val) {
            if (spiritSound == null)
                spiritSound = AudioHandler.Instance.Player3DSound(SFXFiles.evil_spirit, transform, 0f, 1f, true, true);
            else spiritSound.AudioSource.Play();
            StartCoroutine(spiritSound.AudioSource.FadeSFXVolume(.1f, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        }
        else {
            StartCoroutine(spiritSound.AudioSource.FadeSFXVolume(0, AnimationCurve.EaseInOut(0,0,1,1), 3f));
        }
        foreach(ParticleSystem ps in particles) {
            if (val) {
                ps.Play();
            } else {
                ps.Stop();
            }
        }
    }

    public void BehindPlayerDelay() {
        if (behindPlayerPlayed) return;
        behindPlayerPlayed = true;
        StartCoroutine(WaitBeforeBehindPlayerCutscene());
    }


    private IEnumerator WaitBeforeBehindPlayerCutscene() {
        yield return new WaitForSeconds(4f);
        BehindPlayer();
    }

    public void BehindPlayer() {
        EnableSpirit(true);
        StartCoroutine(playAnimation("behind_player", 2.5f, () => {
            if (!intoMirrorPlayed) EnableSpirit(false);
        }));
    }

    public void IntoMirror() {
        if (intoMirrorPlayed) return;
        intoMirrorPlayed = true;

        EnableSpirit(true);
        StartCoroutine(playAnimation("into_mirror", 2.5f, () => {
            EnableSpirit(false);
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