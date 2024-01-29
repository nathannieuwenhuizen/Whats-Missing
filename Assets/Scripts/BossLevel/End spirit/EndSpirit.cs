using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class EndSpirit : MonoBehaviour
{
    
    [SerializeField] private PathCreator pathCreator;
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private Player player;

    private SkinnedMeshToMesh skinnedMeshToMesh;

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;
    [SerializeField]
    private ParticleSystem dustParticles;
    [SerializeField]
    private ParticleSystem trailParticles;

    [SerializeField]
    private float dissappEarDurationInSeconds = .3f;

    [SerializeField]
    private GameObject deathCollider;
    [SerializeField]
    private BossSceneLoaderTrigger sceneCollider;



    private void OnDrawGizmos() {
        for(int i = 0; i < points.Length; i++) {
            Gizmos.DrawSphere(points[i].position, .5f);
        }
    }
    private int ClosestIndexToPlayer() {
        int result = -1;
        float minDist = Mathf.Infinity;
        for(int i = 0; i < points.Length; i++) {
            float dist = Vector3.Distance(player.transform.position, points[i].position);
            if (minDist > dist) {
                result = i;
                minDist = dist;
            }
        }
        return result;
    }
    private Coroutine fadeCoroutine;
    private int currentIndex;
    public int CurrentIndex {
        get { return currentIndex;}
        set { 
            if (currentIndex != value) {
                if (fadeCoroutine != null)StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeToNextPoint());
            }
            currentIndex = value; 
        }
    }
    private void Awake() {
        skinnedMeshToMesh = GetComponent<SkinnedMeshToMesh>();
        dustParticles.Stop();
        sceneCollider.gameObject.SetActive(false);
        // trailParticles.GetComponent<TrailRenderer>().enabled = false;

    }

    private void Start() {
        // skinnedMeshToMesh.StopVFX();
        // StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        meshRenderer.material.SetInt("Alpha", 0);
        skinnedMeshToMesh.StopVFX();

        // SpawnGhost();

    }

    private void UpdateFog() {
        float percentage = (float)CurrentIndex / ((float)points.Length - 1);
        RenderSettings.fogDensity = Mathf.Lerp(0.004f, 0.1f, percentage);
        dustParticles.transform.position = transform.position;
        dustParticles.emissionRate = Mathf.Lerp(5, 100, percentage); 
    }



    private IEnumerator SpawningGhost () {
        yield return new WaitForSeconds(2f);

        UpdateGhostPosition(false);
        StartCoroutine(CheckPlayerPosition());
        yield return StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 0, 1, AnimationCurve.EaseInOut(0,0,1,1), 1));
        skinnedMeshToMesh.StartVFX();

        CurrentIndex = points.Length - 2;
        dustParticles.Play();
        deathCollider.SetActive(false);
        sceneCollider.gameObject.SetActive(true);
        // trailParticles.transform.position = points[currentIndex].position;

    }
    private void SpawnGhost() {
        SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.Anger);
        StartCoroutine(SpawningGhost());
    }


    private IEnumerator FadeToNextPoint() {
        yield return new WaitForEndOfFrame();
        // skinnedMeshToMesh.StopVFX();
        // yield return StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        // StartCoroutine(MoveTrail(points[currentIndex].position));
        // transform.position = points[currentIndex].position;
        // skinnedMeshToMesh.StartVFX();
        // yield return StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 0, 1, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        // float cT = pathCreator.path.GetClosestTimeOnPath(transform.position);
        // float t = pathCreator.path.GetClosestTimeOnPath(points[currentIndex + 1].position);
        // Debug.Log("moving to: " + currentIndex + " | with T = " + t);
        // yield return StartCoroutine(Extensions.AnimateCallBack(cT, t, AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
        //     transform.position = pathCreator.path.GetPointAtTime(v);
        //     transform.rotation = pathCreator.path.GetRotation(v);
        // }, 2f));
    }

    private IEnumerator MoveTrail( Vector3 end) {
        // trailParticles.GetComponent<TrailRenderer>().enabled = true;
        yield return StartCoroutine(TransformExtensions.AnimatingPos(trailParticles.transform, end, AnimationCurve.EaseInOut(0,0,1,1), .5f));
        // trailParticles.GetComponent<TrailRenderer>().enabled = false;

    }
    private bool transporting = false;

    private float CurrentTime = 0;
    public IEnumerator CheckPlayerPosition() {
        while (true) {
            if (!transporting) UpdateGhostPosition();

            // transform.rotation = pathCreator.path.GetRotation(CurrentTime);

            // Debug.Log("closest index = " + ClosestIndexToPlayer());
            CurrentIndex = Mathf.Clamp(ClosestIndexToPlayer() + 1, 0, points.Length - 1 );
            // transform.LookAt(player.transform.position, Vector3.up);
            UpdateFog();
            yield return new WaitForEndOfFrame();
        }
    }
    float maxPlayerT = 0;

    public void UpdateGhostPosition(bool withLerp = true) {
        float pT = Mathf.Max(maxPlayerT, pathCreator.path.GetClosestTimeOnPath(player.transform.position));
        maxPlayerT = pT;
        float dist = 0.03f;
        if (withLerp) CurrentTime =  Mathf.Lerp(CurrentTime, pT + dist, Time.deltaTime * 3f);
        else CurrentTime = pT + dist;

        transform.position = transform.position = pathCreator.path.GetPointAtTime(CurrentTime);
        transform.LookAt(pathCreator.path.GetPointAtTime(CurrentTime + 0.01f), Vector3.up);

        if (maxPlayerT > pathCreator.path.GetClosestTimeOnPath(transform.position)) {
            StartCoroutine(FadeToNextPoint2());
        }
    }
    private IEnumerator FadeToNextPoint2() {
        transporting = true;
        yield return new WaitForEndOfFrame();
        skinnedMeshToMesh.StopVFX();
        yield return StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), .3f));
        UpdateGhostPosition(false);
        skinnedMeshToMesh.StartVFX();
        transporting = false;
        yield return StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 0, 1, AnimationCurve.EaseInOut(0,0,1,1), 1f));
    }




    private void OnEnable() {
        Boss.DieState.OnBossDie += SpawnGhost;
    }
    private void OnDisable() {
        Boss.DieState.OnBossDie -= SpawnGhost;
    }
}
