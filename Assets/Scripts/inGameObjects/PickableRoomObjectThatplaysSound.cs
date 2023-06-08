using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableRoomObjectThatplaysSound : PickableRoomObject
{
    private bool canPlayAudio = false;
    private void OnCollisionEnter(Collision other) {
        if (rb.velocity.magnitude > 1f && canPlayAudio && AudioFile() != "" && inSpace) {
            AudioHandler.Instance?.Play3DSound(AudioFile(), transform, AudioVolume());
            StartCoroutine(DelayAudioEnabling());
        }
    }

    public virtual float AudioVolume() {
        return .4f;
    }
    public virtual string AudioFile() {
        return "";
    }

    protected override void Awake() {
        base.Awake();
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        StartCoroutine(DelayAudioEnabling());
    }

    public IEnumerator DelayAudioEnabling() {
        canPlayAudio = false;
        yield return new WaitForSeconds(2f);
        canPlayAudio = true;
    }
}
