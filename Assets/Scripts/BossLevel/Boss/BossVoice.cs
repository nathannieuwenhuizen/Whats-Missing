using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

public class BossVoice
{

    private readonly string resourceDialoguePath = "Dialogues/boss";
    public delegate void BossVoiceEvent(Line line);
    public static BossVoiceEvent OnLineStart;
    public static BossVoiceEvent OnLineEnd;
    public Dialogue dialogue;

    private Transform transform;
    public BossVoice(Transform _transform) {
        transform = _transform;
        dialogue = DialogueLoader.LoadResourceTextfile(resourceDialoguePath);
        // Debug.Log("dialogue lengths" + dialogue.lines.Length);
    }

    public void Talk(string _id) {
        Line line = dialogue.GetLine(_id);
        // AudioHandler.Instance?.Play3DSound(line.fmod_audio_path, transform);
        OnLineStart?.Invoke(line);
    }
    public void Update()
    {
        // if (Input.GetKeyDown(KeyCode.L)) {
        //     string randomID = dialogue.lines[Mathf.FloorToInt(Random.Range(0, dialogue.lines.Length))].id;
        //     Talk(randomID);
        // }
    }

}
}