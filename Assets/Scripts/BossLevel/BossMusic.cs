using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMusic : MonoBehaviour
{
    private void Awake() {
        
    }

    public void StartMusic(BossMirror m, Player p) {
        StartCoroutine(StartingMusic());

    }


    public IEnumerator UpdateMusicIntesnity() {
        while (BossAlive) {
            if (Player.INVINCIBLE) {
                MusicIntensity = 0.1f;
            } else {
                MusicIntensity = .9f;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void BossIsDead() {
        BossAlive = false;
    }


    public IEnumerator StartingMusic() {
        Debug.Log("boss music should start now");
        AudioHandler.Instance.AudioManager.AudioListenerVolume = 1;
        AudioHandler.Instance.PlayMusic(MusicFiles.boss, 1f, 0f);
        MusicIntensity = 0f;
        BossAlive = true;
        StartCoroutine(UpdateMusicIntesnity());
        yield return new WaitForSeconds(.1f);
    }
    private float musicIntensity;
    public float MusicIntensity {
        get { 
            if (AudioHandler.Instance?.AudioManager.Music != null) {
                float v  = 0;
                AudioHandler.Instance?.AudioManager.Music.FMODInstance.getParameterByName(FMODParams.BOSS_INTENSITY, out v);
                return v;
            }
            return musicIntensity;
        }
        set { 
            musicIntensity = value; 
            if (AudioHandler.Instance?.AudioManager.Music != null) {
                // Debug.Log(" Music intensity = " + value);
                AudioHandler.Instance?.AudioManager.Music.FMODInstance.setParameterByName(FMODParams.BOSS_INTENSITY, value, true);
            }
        }
    }

    private bool bossAlive;
    public bool BossAlive {
        get { 
            if (AudioHandler.Instance?.AudioManager.Music != null) {
                float v  = 0;
                AudioHandler.Instance?.AudioManager.Music.FMODInstance.getParameterByName(FMODParams.BOSS_ALIVE, out v);
                return v == 0;
            }
            return bossAlive;
        }
        set { 
            bossAlive = value; 
            if (AudioHandler.Instance?.AudioManager.Music != null) 
            {
                // Debug.Log(" boss alive = " + value);
                AudioHandler.Instance?.AudioManager.Music.FMODInstance.setParameterByName(FMODParams.BOSS_ALIVE, value ? 0 : 1, true);
            }
        }
    }
    private void Update() {
        // if (Input.GetKeyDown(KeyCode.L))  {
        //     BossAlive = false;
        // }
    }


    private void OnEnable() {
        BossMirror.OnMirrorShake += StartMusic;
        Boss.DieState.OnBossDieStart += BossIsDead;
        TimeProperty.onTimeMissing += BossIsDead;
    }

    private void OnDisable() {
        BossMirror.OnMirrorShake -= StartMusic;
        Boss.DieState.OnBossDieStart -= BossIsDead;
        TimeProperty.onTimeMissing -= BossIsDead;
    }
}
