using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Holds date about when the boss should go to a specific point to attack the player from. 
/// Only listened to certain boss states.
///</summary>
public class MountainAttackPose : MonoBehaviour, ITriggerArea
{
    [SerializeField]
    private Transform position;

    [SerializeField]
    private SkinnedMeshRenderer gizmosRenderer;

    public Vector3 Position {
        get { return position.position;}
    }

    public delegate void MountainAttackEvent(Vector3 position);
    public static MountainAttackEvent OnPlayerEnteringAttackArea;


    public bool InsideArea { get; set; } = false;

    public void OnAreaEnter(Player player)
    {
        OnPlayerEnteringAttackArea?.Invoke(Position);
        InsideArea = true;
    }

    public void OnAreaExit(Player player)
    {
        InsideArea = false;
    }
    private void OnDrawGizmosSelected() {
        //draw boss mesh for more visualisation
        if (gizmosRenderer != null) {
            // Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, poses[i].position.position + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.identity, gizmosRenderer.transform.lossyScale);//  Quaternion.LookRotation(-poses[i].position.position, -Vector3.right), 
            Vector3 aim = -Position; 
            aim.y = 0;
            Quaternion aimRot = Quaternion.LookRotation(aim, Vector3.up);
            // aimRot.y
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, Position + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.Euler(aimRot.eulerAngles.x,aimRot.eulerAngles.y - 90,aimRot.eulerAngles.z - 90), 
            gizmosRenderer.transform.lossyScale);
        }
    }


}
