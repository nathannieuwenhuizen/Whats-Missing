using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdAreaEndTrigger : AreaTrigger
{
    public delegate void EndOfCutsceneEvent();
    public static EndOfCutsceneEvent OnEndOfCutscene; 
    [SerializeField]
    private RoomObject floor;
    [SerializeField]
    private Room  room;
    private bool trigered = false;
    private Potion potion;
    [SerializeField]
    private Transform[] throwPoints;
    [SerializeField]
    private GameObject bossArm;
    [SerializeField]
    private Transform bossArmGrabPosition;
    [SerializeField]
    private Transform bossArmPotionParent;
    [SerializeField]
    private GameObject[] removedGameObjects;

    [SerializeField]
    private Transform test;

    private void Awake() {
        bossArm.SetActive(false);
    }


    public IEnumerator EndCutscene() {
        AudioHandler.Instance.FadeMusic(MusicFiles.hidden_room, 1f);

        floor.AffectedByPotions = true;
        StopPotion();
        SetupBossArm();
        AudioHandler.Instance.PlaySound(SFXFiles.boss_portal_noise, 1f, 1);
        room.Player.FPCamera.ShowAimCutscene(potion.transform, 4.5f, 40f);
        yield return new WaitForSeconds(.5f);
        PotionIsGrabbedByBoss();
        DialoguePlayer.Instance.PlayLine( new Line() {
            text = "ENOUGH!",
            duration = 2f,
            lineEffect = LineEffect.shake
        });
        yield return new WaitForSeconds(2.7f);
        ThrowPotionBack();
        yield return new WaitForSeconds(1f);
        DeactiveGameObjects();
        yield return new WaitForSeconds(1f);
        DeactivatePlayerCollission();
        yield return new WaitForSeconds(1f);
        OnEndOfCutscene?.Invoke();
        yield return new WaitForSeconds(1f);
        room.Area.EndOfArea();
    }


    private void DeactiveGameObjects() {
        foreach(GameObject go in removedGameObjects) {
            go.SetActive(false);
        }
    }

    public void TriggerCutscene(Potion _potion) {
        if (trigered) return;
        trigered = true;
        potion = _potion;
        StartCoroutine(EndCutscene());
    }

    private void StopPotion(){
        potion.RigidBody.velocity = Vector3.zero;
        potion.RigidBody.isKinematic = true;
        potion.FadeOutAimLine();
    }
    // private void Update() {
    //     if (Input.GetKeyDown(KeyCode.L)) {
    //         room.Player.FPCamera.ShowAimCutscene(test, 4.5f, 40f);
    //     }
    // }

    private void ThrowPotionBack() {
        potion.transform.SetParent(null);
        Vector3 aimVector = furthestThrowPoint().position - potion.transform.position;
        potion.RigidBody.isKinematic = false;
        potion.RigidBody.velocity = aimVector.normalized * 50f;
    }

    private Transform furthestThrowPoint() {
        Transform result = throwPoints[0];
        float dist = Vector3.Distance(room.Player.transform.position, result.position);
        for(int i = 1; i < throwPoints.Length; i++) {
            float tempDist = Vector3.Distance(room.Player.transform.position, throwPoints[i].position);
            if (tempDist > dist) {
                result = throwPoints[i];
                dist = tempDist;
            }
        }
        return result;
    }
    private void SetupBossArm() {
        bossArm.SetActive(true);
        //set boss arm same pos as potion
        Vector3 delta = potion.transform.position - bossArmGrabPosition.position;
        bossArm.transform.position += delta;
    }
    private void PotionIsGrabbedByBoss() {
        potion.transform.SetParent(bossArmPotionParent);
    }

    private void DeactivatePlayerCollission() {
        Collider[] colliders = room.Player.GetComponentsInChildren<Collider>() as Collider[];
        foreach(Collider col in colliders) {
            col.enabled = false;
        }
    }
}
