using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableRoomObjectThatplaysSound : PickableRoomObject
{
    [SerializeField]
    protected string audioFile = "";

    private bool canPlayAudio = false;
    private void OnCollisionEnter(Collision other) {
        if (rb.velocity.magnitude > .5f && canPlayAudio && AudioFile() != "" && inSpace && isBurning) {
            AudioHandler.Instance?.Play3DSound(AudioFile(), transform, AudioVolume());
            StartCoroutine(DelayAudioEnabling());
        }
    }

    public virtual float AudioVolume() {
        return .4f;
    }
    public virtual string AudioFile() {
        return audioFile;
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
