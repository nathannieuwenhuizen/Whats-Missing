using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GhostAnimation {
    fountain,
    fireplace,
    swing,
    gazebo,
    ducks
}

[System.Serializable]
public class GhostPose {
    public Transform transform;
    public GhostAnimation ghosetAnimation;
    public int levelIndex;
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

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject book;
    [SerializeField]
    private GameObject marshmallow;

    [SerializeField]
    private GhostPose[] ghostPoses;
 
    private bool disappear = false;
    
    [SerializeField]
    private Room room;

    public bool InSpace { get; set; } = false;

    public override void OnAreaEnter(Player player) {
        base.OnAreaEnter(player);
    }

    public override void OnAreaExit(Player player) {
        base.OnAreaExit(player);
        if (disappear) return;
        disappear = true;
        Vanish();
    }

    public void OnRoomEnter()
    {
        SetGhostPose();
        if (!disappear) {
            //make sounds
        }
    }

    public void SetGhostPose() {

        book.SetActive(false);
        marshmallow.SetActive(false);

        int index = room.Area.Rooms.IndexOf(room);
        GhostPose newPose = new List<GhostPose>(ghostPoses).Find(x => x.levelIndex == index);
        if (newPose != default(GhostPose)) {
            transform.position = newPose.transform.position;
            transform.rotation = newPose.transform.rotation;
            animator.SetTrigger(animations[newPose.ghosetAnimation]);
            if (newPose.ghosetAnimation == GhostAnimation.gazebo) {
                book.SetActive(true);
            } 
            if (newPose.ghosetAnimation == GhostAnimation.fireplace) {
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
        //do vnaish animation
    }

}
