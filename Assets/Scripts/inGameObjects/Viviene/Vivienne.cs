using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class GhostPose {
    public Transform transform;
    public GhostAnimation ghosetAnimation;
    public int levelIndex;
}

public enum GhostAnimation {
    fountain,
    fireplace,
    swing,
    gazebo,
    ducks
}

///<summary>
/// The garden ghost of Gregories daughter in the garden level
///</summary>
public class Vivienne : AreaTrigger, IRoomObject
{

    public readonly Dictionary <GhostAnimation, string> animations = new Dictionary<GhostAnimation, string>() {
        {GhostAnimation.fountain, "Memory_Fountain"},
        {GhostAnimation.fireplace, "Memory_Fireplace"},
        {GhostAnimation.swing, "Memory_Swing"},
        {GhostAnimation.gazebo, "Memory_Gazebo"},
        {GhostAnimation.ducks, "Memory_Ducks"}
    };
    
    private GhostPose ghostPose;

    private SkinnedMeshToMesh skinnedMeshToMesh;

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject book;
    [SerializeField]
    private GameObject marshmallow;

    [Space]
    [Header("disappear values")]
    [SerializeField]
    private bool disappearsWithContact = true;
    [SerializeField]
    private float dissappEarDurationInSeconds = 5f;

    [SerializeField]
    private GhostPose[] ghostPoses;
 
    private bool disappear = false;
    
    [SerializeField]
    private Room room;

    public bool InSpace { get; set; } = false;

    private void Awake() {
        skinnedMeshToMesh = GetComponent<SkinnedMeshToMesh>();
        // skinnedMeshToMesh.StopVFX();
    }

    private void Update() {
        
    }
    public override void OnAreaEnter(Player player) {
        base.OnAreaEnter(player);
        if (disappear) return;
        disappear = true;
        Vanish();
    }

    public override void OnAreaExit(Player player) {
        base.OnAreaExit(player);
    }

    public void OnRoomEnter()
    {
        SetGhostPose();
        if (!disappear) {
            //make sounds
        }
    }

    public void SetGhostPose() {
        // return;
        book.SetActive(false);
        marshmallow.SetActive(false);

        int index = room.LoadIndex;
        Debug.Log("level index = " + index);
        GhostPose newPose = new List<GhostPose>(ghostPoses).Find(x => x.levelIndex == index);

        if (newPose != default(GhostPose)) {
            ghostPose = newPose;
            transform.position = ghostPose.transform.position;
            transform.rotation = ghostPose.transform.rotation;
            animator.SetTrigger(animations[ghostPose.ghosetAnimation]);
            if (ghostPose.ghosetAnimation == GhostAnimation.gazebo) {
                book.SetActive(true);
            } 
            if (ghostPose.ghosetAnimation == GhostAnimation.fireplace) {
                marshmallow.SetActive(true);
            } 
        } else {
            gameObject.SetActive(false);
        }

    }

    public void OnRoomLeave()
    {
        if (!disappear) {
            StopSound();
        }
    }

    public void StopSound() {
        //stop sounds
    }
    

    public void Vanish() {
        //do vanish animation
        if (!disappearsWithContact) return;

        skinnedMeshToMesh.StopVFX();
        StartCoroutine(meshRenderer.material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        if (book.activeSelf) StartCoroutine(book.GetComponent<MeshRenderer>().material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        if (marshmallow.activeSelf) StartCoroutine(marshmallow.GetComponent<MeshRenderer>().material.AnimatingNumberPropertyMaterial("Alpha", 1, 0, AnimationCurve.EaseInOut(0,0,1,1), dissappEarDurationInSeconds));
        StartCoroutine(VanishCheckAchievement());
        Destroy(gameObject, dissappEarDurationInSeconds);
        
    }
    private IEnumerator VanishCheckAchievement() {
        yield return new WaitForSeconds(dissappEarDurationInSeconds - 1f);
        switch(ghostPose.ghosetAnimation) {
            case GhostAnimation.ducks:
                PlayerData.GARDEN_GHOST_VANISH_0 = true;
                break;
            case GhostAnimation.fireplace:
                PlayerData.GARDEN_GHOST_VANISH_1 = true;
                break;
            case GhostAnimation.fountain:
                PlayerData.GARDEN_GHOST_VANISH_2 = true;
                break;
            case GhostAnimation.gazebo:
                PlayerData.GARDEN_GHOST_VANISH_3 = true;
                break;
            case GhostAnimation.swing:
                PlayerData.GARDEN_GHOST_VANISH_4 = true;
                break;
        }
        Debug.Log("check chasing past");
        PlayerData.CheckChasingThePastAchievements();

    }


    private void OnDrawGizmos() {
        if (meshRenderer != null) {
            foreach(GhostPose pose in ghostPoses) {
                if (pose.transform != null) {
                    Gizmos.DrawWireMesh(meshRenderer.sharedMesh, pose.transform.position,  Quaternion.LookRotation(pose.transform.up, -pose.transform.right), meshRenderer.transform.lossyScale);
                }
            }
        }
    }
}
