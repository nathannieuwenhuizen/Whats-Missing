using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WanderPose {
    [SerializeField]
    public Transform position;
    [SerializeField]
    public Transform aimPosition;
}

[System.Serializable]
public class WanderingPaths {
    public WanderingPath firstShardPath;
    public WanderingPath secondShardPath;
    public WanderingPath thirdShardPath;
    public WanderingPath fourthShardPath;
    public WanderingPath fifthShardPath;
}

public class WanderingPath : MonoBehaviour {
    
    public WanderPose[] poses;
    [SerializeField]
    private SkinnedMeshRenderer gizmosRenderer;
    [SerializeField]
    private Transform landingPosition;
    public Transform LandingPos {
        get { return landingPosition;}
    }
    [SerializeField]
    private Boss.BodyMovementType bossMovementType = Boss.BodyMovementType.airSteeringAtMountain;
    public Boss.BodyMovementType BossMovementType {
        get { return bossMovementType; }
    }

    [SerializeField]
    public bool showGizmo = true;
    

    private void Awake() {
        showGizmo = false;
    }
    private void DrawGizmo() {
        if (poses.Length < 1) return;
        
        if (poses[0].position == null) return;
        
        //draw landing pos
        if (LandingPos != null) {
            Gizmos.DrawWireSphere(LandingPos.position, 1f);
            #if UNITY_EDITOR
            Handles.Label(LandingPos.position, "landing pos");
            #endif
        }
        Gizmos.color = Color.yellow;
        Vector3 startpos = poses[0].position.position;
        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i].position != null) {
                
                //draw point
                Gizmos.DrawWireSphere(poses[i].position.position, .5f);
                
                //draw aim line
                if (poses[i].aimPosition != null) {
                    Debug.DrawLine(poses[i].position.position, poses[i].aimPosition.position);
                }


                //draw boss mesh for more visualisation
                if (gizmosRenderer != null) {
                    // Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, poses[i].position.position + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.identity, gizmosRenderer.transform.lossyScale);//  Quaternion.LookRotation(-poses[i].position.position, -Vector3.right), 
                    Vector3 aim = -poses[i].position.position; 
                    aim.y = 0;
                    Quaternion aimRot = Quaternion.LookRotation(aim, Vector3.up);
                    // aimRot.y
                    Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, poses[i].position.position + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.Euler(aimRot.eulerAngles.x,aimRot.eulerAngles.y - 90,aimRot.eulerAngles.z - 90), 
                    gizmosRenderer.transform.lossyScale);
                }

                //draw lines between points
                Vector3 endPos = startpos;
                if (poses.Length - 2 < i ) endPos = poses[0].position.position;
                else if (poses[i + 1].position != null) endPos = poses[i + 1].position.position;
                Debug.DrawLine(startpos, endPos, Color.yellow);
                Gizmos.DrawSphere(startpos, .5f);
                #if UNITY_EDITOR
                Handles.Label(poses[i].position.position, i.ToString());
                #endif
                startpos = endPos;
            }
        }
    }
    private void OnDrawGizmos() {
        if (showGizmo) DrawGizmo();
    }
    private void OnDrawGizmosSelected() {
        DrawGizmo();
    }
}
