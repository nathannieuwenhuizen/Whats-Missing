using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/AudioLibrary", order = 1)]
public class AudioLibrary: ScriptableObject {

    [SerializeField]
    public List<SFXInstance> soundEffectInstances;

    [SerializeField]
    public List<MusicInstance> musicClips;

}
