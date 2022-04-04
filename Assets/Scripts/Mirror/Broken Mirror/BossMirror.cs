using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMirror : Mirror, ITriggerArea
{
    public delegate void BossMirrorEvent(BossMirror bossMirror);
    public static BossMirrorEvent OnBossMirrorShardAttached;

    [SerializeField]
    private MirrorShard[] shards;

    [SerializeField]
    private ParticleSystem explosionSmoke;
    [SerializeField]
    private ParticleSystem explosionShards;

    private MirrorData GetShardMirrorData() {
        string letters = "";
        MirrorData result = new MirrorData() {changeType = ChangeType.missing, isOn = false, };
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
        Word = "spirit";
        TogleVisibilityUnselectedObj(0);
        MirrorCanvas.IsInteractable = false;
        // foreach(MirrorShard shard in shards) {
        //     shard.PlanarReflection.SetRenderTexture(PlanarReflection.reflectionCamera.targetTexture);
        // }
    }

    public void TogleVisibilityUnselectedObj(float alpha) {
        foreach (Letter letter in MirrorCanvas.letterObjects)
        {
            Color temp = letter.Color;
            temp.a = alpha;
            letter.Color = temp;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Explode();
        }
    }


    public void Explode() {
        MirrorCanvas.DeselectLetters();
        TogleVisibilityUnselectedObj(1);
        for (int i = 0; i < shards.Length; i++) {
            shards[i].DisconnectedFromMirror(4000);
        }
        explosionSmoke.Emit(100);
        explosionShards.Emit(60);
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

    public void OnAreaEnter(Player player)
    {
        if (introCutscene) return;
        introCutscene = true;
        StartCoroutine(ShakeBeforeExplosion());
    }

    public void OnAreaExit(Player player)
    {

    }

    private float shakeDuration = 4f;
    private Coroutine shakeCoroutine;

    public bool InsideArea { get; set; }
    private bool introCutscene;

    private IEnumerator ShakeBeforeExplosion() {
        Debug.Log("coroutine started");
        Quaternion startRotation = transform.localRotation;
        shakeCoroutine = StartCoroutine(transform.ShakeZRotation(10f, 3f, shakeDuration * 2));
        foreach(MirrorShard shard in shards) {
            shard.Shake(shakeDuration);
        }
        yield return StartCoroutine(transform.parent.AnimatingLocalRotation(Quaternion.Euler(0,0,90), AnimationCurve.EaseInOut(0,0,1,1), shakeDuration));
        // yield return new WaitForSeconds(shakeDuration);
        foreach(MirrorShard shard in shards) {
            shard.StopShake();
        }
        StopCoroutine(shakeCoroutine);
        transform.localRotation = startRotation;
        Explode();
    }


}
