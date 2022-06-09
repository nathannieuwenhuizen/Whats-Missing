using System.Collections;
using System.Collections.Generic;
using Boss;
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
    private Transform rangeBegin;
    [SerializeField]
    private Transform rangeEnd;

    [SerializeField]
    private bool showGizmo = false;

    [SerializeField]
    private SkinnedMeshRenderer gizmosRenderer;

    public Vector3 BeginPosition {
        get { return position.position;}
    }

    public Vector3 RangeBegin {
        get { return rangeBegin.position;}
    }

    public Vector3 RangeEnd {
        get { return rangeEnd.position;}
    }

    public delegate void MountainAttackEvent(MountainAttackPose pose);
    public static MountainAttackEvent OnPlayerEnteringAttackArea;


    public bool InsideArea { get; set; } = false;

    public void OnAreaEnter(Player player)
    {
        OnPlayerEnteringAttackArea?.Invoke(this);
        InsideArea = true;
    }

    public Vector3 PosClosestToPlayerButWithinRange(BossMountain _mountain, Player _player) {
        Vector3 result = RangeBegin;
        if (rangeBegin == null) return result;
        if (rangeEnd == null) return result;
        MountainCoordinate begin = MountainCoordinate.FromPosition(_mountain, RangeBegin);
        MountainCoordinate end = MountainCoordinate.FromPosition(_mountain, RangeEnd);
        MountainCoordinate player = MountainCoordinate.FromPosition(_mountain, _player.transform.position);
        float beginAngle = begin.Angle;
        float endAngle = end.Angle;

        MountainCoordinate a = begin;
        MountainCoordinate b = end;

        // if ((endAngle - beginAngle > 0 && endAngle - beginAngle < 180) || 
        // (endAngle < 180 && beginAngle > 180 && Mathf.Abs(begin.angle - end.angle) < 180f)) {
        //     a = begin;
        //     b = end;
        // }
        if (beginAngle > endAngle ) {
            a = end;
            b = begin;
        }
        float precentage = ((player.angle - a.angle) / (b.angle - a.angle));
        
        float playerAngle = player.angle;
        if (Mathf.Abs(a.angle - b.angle) > 180f ) {
            MountainCoordinate c = a;
            a = b;
            b = c;


            beginAngle = a.Angle;
            endAngle = b.Angle;
            if (beginAngle < 180)beginAngle += 360;
            if (endAngle < 180)endAngle += 360;
            if (playerAngle < 180)playerAngle += 360;
            precentage = ((playerAngle - beginAngle) / (endAngle - beginAngle));
        }

        Debug.Log("begin: " +a.angle + " |  end: " + b.angle + " |  playerange: " + playerAngle);
        Debug.Log( "precentage" + precentage);
        result = MountainCoordinate.Lerp(a,b, precentage).ToVector(_mountain);
        return result;
    }
    public void OnAreaExit(Player player)
    {
        InsideArea = false;
    }
    private void OnDrawGizmosSelected() {
        //draw boss mesh for more visualisation
        if (gizmosRenderer != null) {
            // Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, poses[i].position.position + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.identity, gizmosRenderer.transform.lossyScale);//  Quaternion.LookRotation(-poses[i].position.position, -Vector3.right), 
            Vector3 aim = -BeginPosition; 
            aim.y = 0;
            Quaternion aimRot = Quaternion.LookRotation(aim, Vector3.up);
            // aimRot.y
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, BeginPosition + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.Euler(aimRot.eulerAngles.x,aimRot.eulerAngles.y - 90,aimRot.eulerAngles.z - 90), gizmosRenderer.transform.lossyScale);
            aim = -RangeBegin; 
            aim.y = 0;
            aimRot = Quaternion.LookRotation(aim, Vector3.up);
            Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, RangeBegin + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.Euler(aimRot.eulerAngles.x,aimRot.eulerAngles.y - 90,aimRot.eulerAngles.z - 90), gizmosRenderer.transform.lossyScale);
            aim = -RangeEnd; 
            aim.y = 0;
            aimRot = Quaternion.LookRotation(aim, Vector3.up);
            Gizmos.DrawWireMesh(gizmosRenderer.sharedMesh, RangeEnd + Vector3.up * Boss.Boss.BOSS_HEIGHT, Quaternion.Euler(aimRot.eulerAngles.x,aimRot.eulerAngles.y - 90,aimRot.eulerAngles.z - 90), gizmosRenderer.transform.lossyScale);
            
        }
    }

    private void OnDrawGizmos() {
        if (showGizmo) {
            OnDrawGizmosSelected();
        }
    }


}
