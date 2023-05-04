using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BossMirror : Mirror, ITriggerArea
{
    
    public delegate void BossMirrorEvent(BossMirror bossMirror);
    public static BossMirrorEvent OnBossMirrorShardAttached;
    public static BossMirrorEvent OnMirrorShardAmmountUpdate;
    public static BossMirrorEvent OnMirrorShake;
    public static BossMirrorEvent OnMirrorComplete;
    public static BossMirrorEvent OnMirrorExplode;
    public static BossMirrorEvent OnAskingHint;

    private Rigidbody rb;

    [SerializeField]
    private Player player;
    [SerializeField]
    private ForcefieldDemo.Forcefield Forcefield;

    private bool followPlayer = false;

    private float mirrorHeight = 2.8f;

    private Vector3 startPos;
    
    [SerializeField]
    private GameObject bossStencilBuffer;

    [SerializeField]
    private MirrorShard[] shards;
    public MirrorShard[] Shards {
        get { return shards;}
    }

    [SerializeField]
    private ParticleSystem explosionSmoke;
    [SerializeField]
    private ParticleSystem explosionShards;

    [Header("testing")]
    [SerializeField]
    private bool skipIntro = false;
    public bool SkippingIntro {
        get { return skipIntro;}
    }
    [SerializeField]
    private int ammountOfShardAlreadyCollected = 0;

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

    protected override void Awake() {
        base.Awake();
        #if !UNITY_EDITOR
            skipIntro = false;
        #endif

        bossStencilBuffer.SetActive(false);
        InSpace = true;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        
        for (int i = 0; i < shards.Length; i++) {
            shards[i].BossMirror = this;
        }
        MirrorData = GetShardMirrorData();
        SetupCanvas();
        
        MirrorCanvas.IsInteractable = false;

        //assign the correct letter classes to the shards
        List<Letter> temp = new List<Letter>(MirrorCanvas.Letters);
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
        // foreach(MirrorShard shard in shards) {
        //     shard.PlanarReflection.SetRenderTexture(PlanarReflection.reflectionCamera.targetTexture);
        // }


        if (skipIntro) StartCoroutine(SkipIntro());
        else {
            AudioHandler.Instance.AudioManager.AudioListenerVolume = 0;
            AudioHandler.Instance.FadeListener(1f);
            AudioHandler.Instance.PlayMusic(MusicFiles.hidden_room, .5f, 5.5f);

            TogleVisibilityOfLetters(0);
        }
    }

    public IEnumerator SkipIntro() {
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 0;
        AudioHandler.Instance?.FadeListener(1f);
        AudioHandler.Instance.PlayMusic(MusicFiles.boss, .5f, 0f);
        yield return new WaitForSeconds(.1f);
        introCutscene = true;
        for (int i = 0; i < shards.Length; i++) {
            shards[i].animated = false;
            shards[i].DisconnectedFromMirror();
        }
        // followPlayer = true;
        for(int i = 0 ; i < ammountOfShardAlreadyCollected; i++) {
            yield return new WaitForSeconds(.1f);
            shards[i].invokeBossReaction = i >= ammountOfShardAlreadyCollected - 1; //purely for testing
            shards[i].transform.position = transform.position;
            shards[i].Release();
        }
        Forcefield.IsOn = true;
        OnMirrorShardAmmountUpdate?.Invoke(this);
        StartCoroutine(transform.parent.AnimatingLocalRotation(Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, transform.parent.eulerAngles.z + 90), AnimationCurve.EaseInOut(0,0,1,1), .1f));
        startPos = transform.position;
        StartCoroutine(transform.parent.AnimatingPos(startPos + new Vector3(0,-mirrorHeight,0), AnimationCurve.EaseInOut(0,0,1,1), .1f));

    }

    public void TogleVisibilityOfLetters(float alpha) {
        foreach (Letter letter in MirrorCanvas.Letters)
        {
            Color temp = letter.Color;
            temp.a = alpha;
            letter.Color = temp;
        }
    }
    public void OnlyShowLettersOfWord(string _word) {
        foreach (Letter letter in MirrorCanvas.Letters)
        {
            bool partOfWord = _word.Contains(letter.LetterValue);
            Color temp = letter.Color;
            temp.a = partOfWord ? 1 : 0;
            letter.Color = temp;
            letter.Interactable = partOfWord;
        }
    }

    private void Update() {
        if (followPlayer && player != null) {
            Vector3 delta = transform.position - player.transform.position;
            delta.y = 0;
            Quaternion aimRotation = Quaternion.LookRotation(delta, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, aimRotation, Time.deltaTime);
            // transform.LookAt(player.transform.position, Vector3.up);
        }
    }


    public void Explode() {
        // rb.isKinematic = false;
        // rb.velocity = transform.forward * 10;
        AudioHandler.Instance?.Play3DSound(SFXFiles.miror_break, transform);

        MirrorCanvas.DeselectLetters();
        TogleVisibilityOfLetters(1);
        for (int i = 0; i < shards.Length; i++) {
            shards[i].DisconnectedFromMirror();
        }
        explosionSmoke.Emit(100);
        explosionShards.Emit(60);
        OnMirrorExplode?.Invoke(this);

    }

    public void AttachMirrorShard(MirrorShard shard) {
        if (shard.invokeBossReaction) OnBossMirrorShardAttached?.Invoke(this);
        OnMirrorShardAmmountUpdate?.Invoke(this);
        MirrorCanvas.IsInteractable = true;

        if (AmmountOfShardsAttached() == shards.Length) {
            // OnMirrorComplete?.Invoke(this);
        }

        MirrorCanvas.DeselectLetters();
        Confirm();
        UpdateMirrorHeader();
        bool showNext = true;
        for (int i = 0 ; i < shards.Length; i++) {
            if (!shards[i].Attached) {
                shards[i].ToggleLetterVisibilty(showNext);
                showNext = false;
            }
        }

    }

    private void UpdateMirrorHeader() {
        switch (AmmountOfShardsAttached()) {
            case 1:
            MirrorData.changeType = ChangeType.tooSmall;
            break;
            case 2:
            MirrorData.changeType = ChangeType.tooBig;
            break;
            case 3:
            MirrorData.changeType = ChangeType.missing;
            break;
            case 4:
            MirrorData.changeType = ChangeType.missing;
            break;
            case 5:
            MirrorData.changeType = ChangeType.missing;
            // OnlyShowLettersOfWord("spirit");
            break;
        }
        MirrorCanvas.SetupText(MirrorData.changeType);
    }

    public bool MirrorIsComplete() {
        foreach(MirrorShard shard in shards) {
            if (shard.Attached == false) return false;
        }
        return true;
    }
    public int AmmountOfShardsAttached() {
        return shards.Where( s => s.Attached == true).ToArray().Length;
    }
    
    public void OnAreaEnter(Player _player)
    {
        if (introCutscene || skipIntro) return;
        introCutscene = true;
        OnMirrorShake?.Invoke(this);
        StartCoroutine(ShakeBeforeExplosion());
        player = _player;
    }


    public void OnAreaExit(Player player)
    {

    }

    private float shakeDuration = 4f;
    public float ShakeDuration {
        get { return shakeDuration;}
    }
    private Coroutine shakeCoroutine;

    public bool InsideArea { get; set; }
    private bool introCutscene;

    private IEnumerator ShakeBeforeExplosion() {
        startPos = transform.position;
        AudioHandler.Instance?.PlaySound(SFXFiles.mirror_shake);
        AudioHandler.Instance?.FadeMusic(MusicFiles.boss, 5f, true);

        //activate stencil buffer
        bossStencilBuffer.SetActive(true);
        Quaternion startRotation = transform.localRotation;
        shakeCoroutine = StartCoroutine(transform.ShakeZRotation(3f, 5f, shakeDuration * 2));
        foreach(MirrorShard shard in shards) {
            shard.Shake(shakeDuration);
        }
        StartCoroutine(transform.parent.AnimatingLocalRotation(Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, transform.parent.eulerAngles.z + 90), AnimationCurve.EaseInOut(0,0,1,1), shakeDuration));
        yield return new WaitForSeconds(shakeDuration);
        foreach(MirrorShard shard in shards) {
            shard.StopShake();
        }
        StopCoroutine(shakeCoroutine);
        transform.localRotation = startRotation;
        Explode();

        StartCoroutine(transform.parent.AnimatingPos(startPos + new Vector3(0,-mirrorHeight,0), AnimationCurve.EaseInOut(0,0,1,1), 1.5f));

        //wait a little bit to deactivate the stencil buffer mask
        yield return new WaitForSeconds(1f);
        bossStencilBuffer.SetActive(false);
        yield return new WaitForSeconds(6f);
        OnMirrorShardAmmountUpdate?.Invoke(this);

    }

    public void RepositionMirror() {
        if (followPlayer == false) StartCoroutine(RepositioningMirror());
    }

    private IEnumerator RepositioningMirror() {
        Debug.Log("reposition mirror");
        //position bossmirror to original state
        // StartCoroutine(transform.parent.AnimatingPos(startPos, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        // yield return StartCoroutine(transform.parent.AnimatingLocalRotation(Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, transform.parent.eulerAngles.z - 90), AnimationCurve.EaseInOut(0,0,1,1), 1f));
        // followPlayer = true;
        yield return new WaitForEndOfFrame();
    }

    private void OnEnable() {
        MirrorShard.OnPickedUp += MirrorShardRecolectReaction;
    }

    private void OnDisable() {
        MirrorShard.OnPickedUp -= MirrorShardRecolectReaction;
    }

    public void MirrorShardRecolectReaction(MirrorShard _shard) {

        //remove the gravity when the shard at ceiling has been picked up.
        switch (_shard.ShardIndex) {
            case 3:    
                MirrorCanvas.DeselectLetters();
                Confirm();
            break;        

        }
    }
    public void HintToggleClick() {
        OnAskingHint?.Invoke(this);
    }

}
