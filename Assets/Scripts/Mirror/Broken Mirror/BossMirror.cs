using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMirror : Mirror
{
    public delegate void BossMirrorEvent(BossMirror bossMirror);
    public static BossMirrorEvent OnBossMirrorShardAttached;

    [SerializeField]
    private MirrorShard[] shards;

    private MirrorData GetShardMirrorData() {
        string letters = "";
        MirrorData result = new MirrorData() {changeType = ChangeType.tooSmall, isOn = false, };
        foreach(MirrorShard shard in shards) {
            foreach(LetterCoords coords in shard.LetterCoords) {
                letters = letters + coords.letterValue;
            }
        }
        result.letters = letters;
        return result;
    }

    protected void Awake() {
        base.Awake();
        for (int i = 0; i < shards.Length; i++) {
            shards[i].BossMirror = this;
        }
        MirrorData = GetShardMirrorData();
        SetupCanvas();
        
        //assign the correct letter classes to the shards
        List<Letter> temp = new List<Letter>(MirrorCanvas.letterObjects);
        foreach(MirrorShard shard in shards) {
            foreach(LetterCoords coords in shard.LetterCoords) {
                Letter letter = temp.Find(x => x.LetterValue == coords.letterValue);
                if (letter != null)
                {
                    coords.letter = letter;
                    temp.Remove(letter);
                }
            }
        }
    }
    private void Start() {
        // foreach(MirrorShard shard in shards) {
        //     shard.PlanarReflection.SetRenderTexture(PlanarReflection.reflectionCamera.targetTexture);
        // }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Explode();
        }
    }


    public void Explode() {
        for (int i = 0; i < shards.Length; i++) {
            shards[i].DisconnectedFromMirror();
        }
    }

    public void AttachMirrorShard(MirrorShard shard) {
        OnBossMirrorShardAttached?.Invoke(this);
    }

    public bool MirrorIsComplete() {
        foreach(MirrorShard shard in shards) {
            if (shard.Attached == false) return false;
        }
        return true;
    }
    public int AmmountOfShardsAttached() {
        int result = 0;
        foreach(MirrorShard shard in shards) {
            if (shard.Attached) result++;
        }
        return result;
    }

}
