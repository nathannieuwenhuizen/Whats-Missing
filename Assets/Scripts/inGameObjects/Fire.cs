using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{
    SFXInstance Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance =  AudioHandler.Instance.Player3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 20);
        // StartCoroutine(test());
    }

    public IEnumerator test() {
        yield return new WaitForSeconds(4f);
        AudioHandler.Instance.Stop3DSound(Instance);
    }

    private void Reset() {
        Word = "Fire";
    }
}
