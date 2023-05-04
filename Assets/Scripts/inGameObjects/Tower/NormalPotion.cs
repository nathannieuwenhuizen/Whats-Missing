using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPotion : PickableRoomObject
{
    private bool canPlayAudio = false;
    private void OnCollisionEnter(Collision other) {
        if (rb.velocity.magnitude > 1f && canPlayAudio) {
            AudioHandler.Instance?.Play3DSound(SFXFiles.normal_potion, transform, .4f);
            StartCoroutine(DelayAudioEnabling());
        }
    }

    protected override void Awake() {
        base.Awake();
        StartCoroutine(DelayAudioEnabling());
    }

    public IEnumerator DelayAudioEnabling() {
        canPlayAudio = false;
        yield return new WaitForSeconds(2f);
        canPlayAudio = true;
    }

    private void Reset() {
        Word = "Potion";
        AlternativeWords = new string[] { "potions", "glass" };
    }
}
