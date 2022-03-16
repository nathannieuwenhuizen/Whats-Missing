using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMirror : Mirror
{
    [SerializeField]
    private MirrorShard[] shards;

    private MirrorData GetShardMirrorData() {
        string letters = "";
        MirrorData result = new MirrorData() {changeType = ChangeType.tooSmall, isOn = false };
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
            shards[i].bossMirror = this;
        }
    }
    private void Start() {
        
        SetupCanvas();
    }



    public void Explode() {
        for (int i = 0; i < shards.Length; i++) {
            shards[i].DisconnectedFromMirror();
        }
    }

    public void AttachMirrorShard(MirrorShard shard) {

    }

}
