using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line {
    public string id;
    public string text;
    public string fmod_audio_path;
    public float duration;
}

[System.Serializable]
public class Dialogue {
    public Line[] lines;
    public Line GetLine(string id) {
        foreach (Line line in lines)
            if (line.id == id) return line;
        return null;
    }

}

public class DialogueLoader {
    public static Dialogue LoadResourceTextfile(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        Debug.Log("target file: " + targetFile.text);
        return JsonUtility.FromJson<Dialogue>(targetFile.text);
    }
}