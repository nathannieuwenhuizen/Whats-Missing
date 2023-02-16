using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterflies : RoomObject
{

    private ParticleSystem butterflyParticles;
    private ParticleSystem.SizeOverLifetimeModule sizeModule;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.NoiseModule noiseModule;
    protected Vector3 startNoise;
    protected override void Awake() {
        base.Awake();
        butterflyParticles = GetComponent<ParticleSystem>();
        sizeModule = butterflyParticles.sizeOverLifetime;
        mainModule = butterflyParticles.main;
        noiseModule = butterflyParticles.noise;
        startNoise = CurrentNoise;
    }
    public Butterflies () {
        largeScale = 5;
        // animationDuration = 1.5f;
    }

    protected float moduleSize = 1;
    public override float CurrentScale { 
        get { 
            return moduleSize;
        } 
        set {
            moduleSize = value;
            sizeModule.size = value;
        }  
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        mainModule.simulationSpeed = .5f;
        CurrentNoise = startNoise * 5f;

    }

    public Vector3 CurrentNoise {
        get {
            return new Vector3(
                noiseModule.strengthXMultiplier, 
                noiseModule.strengthYMultiplier, 
                noiseModule.strengthZMultiplier
                );
        }
        set {
            noiseModule.strengthXMultiplier = value.x;
            noiseModule.strengthYMultiplier = value.y;
            noiseModule.strengthZMultiplier = value.z;
        }
    }

    public override void OnEnlargeRevert()
    {
        base.OnEnlargeRevert();
        CurrentNoise = startNoise;
        mainModule.simulationSpeed = 1;
    }

    public override void OnRoomEnter()
    {
        butterflyParticles.Stop();
        butterflyParticles.Play();
        base.OnRoomEnter();
    }

    public void UpdateTimeSimulationSpeed(){
        // mainModule.simulationSpeed = Room.TimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeSimulationSpeed;
        TimeProperty.onTimeAppearing -= UpdateTimeSimulationSpeed;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeSimulationSpeed;
        TimeProperty.onTimeAppearing += UpdateTimeSimulationSpeed;
    }

    private void Reset() {
        Word = "butterflies";
        AlternativeWords = new string[] { "butterfly", "insect", "life", "animal", "animals" };
    }

}
