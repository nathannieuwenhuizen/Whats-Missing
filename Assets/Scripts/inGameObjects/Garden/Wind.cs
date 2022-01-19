using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : Property
{

    public static OnPropertyToggle OnWindEnlarged;
    public static OnPropertyToggle OnWindNormal;
    [SerializeField]
    private Room room;

    [SerializeField]
    private ParticleSystem windParticles;
    private ParticleSystem.MainModule mainModule;
    private float startSpeed;
    private float emissionRate;
    private List<Rigidbody> allRigidBodies;
    private float windForce = 1000f;
    private float maxVelocity = 5;

    private SFXInstance windAudio;
    private void Awake() {
        mainModule = windParticles.main;
        startSpeed = mainModule.startSpeed.constant;
        emissionRate = windParticles.emissionRate;
    }


    public override void OnEnlarge()
    {
        room.Player.Movement.KinematicMovement = false;
        OnWindEnlarged?.Invoke();
        mainModule.startSpeed = 50f;
        windParticles.emissionRate = 200f;

        windAudio = AudioHandler.Instance.PlaySound(SFXFiles.wind, .5f, 1f, true);
        windAudio.AudioSource.Play();
        IsEnlarged = true;
        StartCoroutine(ApplyWindForce());
        base.OnEnlarge();
    }
    public override IEnumerator AnimateEnlarging()
    {
        yield return AudioHandler.Instance.FadeVolume(windAudio.AudioSource, 0, .5f, animationDuration);
        yield return base.AnimateEnlarging();
    }

    public override void OnEnlargeRevert()
    {
        room.Player.Movement.KinematicMovement = true;
        OnWindNormal?.Invoke();
        IsEnlarged = false;
        mainModule.startSpeed = startSpeed;
        windParticles.emissionRate = emissionRate;
        base.OnEnlargeRevert();
    }
    
    public override IEnumerator AnimateEnlargeRevert() {
        yield return AudioHandler.Instance.FadeVolume(windAudio.AudioSource, .5f, 0, animationDuration);
        yield return base.AnimateEnlargeRevert();

    }
    public override void OnEnlargeRevertFinish()
    {
        AudioHandler.Instance?.StopSound(SFXFiles.wind);
        base.OnEnlargeRevertFinish();
    }

    public IEnumerator ApplyWindForce() {
        allRigidBodies = room.GetAllObjectsInRoom<Rigidbody>();
        while (IsEnlarged) {
            Debug.Log("apply wind force");
            Vector3 forceDirection = Vector3.zero;
            for(int i = 0 ; i < allRigidBodies.Count; i++) {
                forceDirection = allRigidBodies[i].position - transform.position;
                bool isPlayer = allRigidBodies[i] == room.Player.Movement.RB;
                if (isPlayer) { 
                    forceDirection *= 2f;
                    forceDirection.y = 0;
                }
                allRigidBodies[i].AddForce(forceDirection.normalized * windForce * Time.deltaTime, ForceMode.Acceleration);
                if(allRigidBodies[i].velocity.magnitude > maxVelocity && !isPlayer) {
                    allRigidBodies[i].velocity = allRigidBodies[i].velocity.normalized * maxVelocity;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }


    private void Reset() {
        Word = "wind";
        AlternativeWords = new string[] { "winds", "air", "weather", "breeze"};
    }

}
