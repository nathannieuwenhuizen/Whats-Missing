using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

///<summary>
/// A struct containing Mountain coordinates for the path handeler
///</summary>
[System.Serializable]
public struct MountainCoordinate {
    ///<summary>
    /// The angle in degrees
    ///</summary>
    public float angle;
    public float Angle {
        get { return angle % 360;}
        set {
            angle = value % 360;
        }
    }
    ///<summary>
    /// The world Y position
    ///</summary>
    public float yPos;
    public Vector3 ToVector(BossPathHandler _pathHandeler, float offset = 0) {
        Vector3 result = Vector3.zero;

        float precentage = Mathf.Clamp(yPos - _pathHandeler.transform.position.y, 0, _pathHandeler.Height) / _pathHandeler.Height;
        float resultRadius = _pathHandeler.BottomRadius + (_pathHandeler.TopRadius - _pathHandeler.BottomRadius) * precentage;

        result.y = yPos;
        result.x = _pathHandeler.transform.position.x + Mathf.Cos(Angle * Mathf.Deg2Rad) * resultRadius;
        result.z = _pathHandeler.transform.position.z + Mathf.Sin(Angle * Mathf.Deg2Rad) * resultRadius;

        if (offset != 0) result += Normal(_pathHandeler) * offset;

        return result;
    }

    public static MountainCoordinate Lerp(MountainCoordinate begin, MountainCoordinate end, float index) {
        float _angle = Mathf.Lerp(begin.Angle, end.Angle, index);
        if (Mathf.Abs(begin.angle - end.angle) > 180f)
        {
            float beginAngle = begin.Angle;
            float endAngle = end.Angle;
            if (beginAngle < 180)beginAngle += 360;
            if (endAngle < 180)endAngle += 360;
            _angle = Mathf.Lerp(beginAngle, endAngle, index);

        }
        return new MountainCoordinate() {
            angle = _angle,
            yPos = Mathf.Lerp(begin.yPos,end.yPos, index)
        };
    }
    public Vector3 Normal(BossPathHandler _pathHandler) {
        Vector3 result= Vector3.zero;
        Vector3 delta = ToVector(_pathHandler) - _pathHandler.transform.position;
        result.x = delta.x;
        result.z = delta.z;
        result = result.normalized;
        float _angle = 90 - Mathf.Atan2( _pathHandler.Height, (_pathHandler.BottomRadius - _pathHandler.TopRadius )) * Mathf.Rad2Deg;
        result.y = Mathf.Sin( _angle * Mathf.Deg2Rad);
        return result.normalized;
    }
    
    public bool IsVisible(BossPathHandler pathhandeler, Vector3 pos) {
        Vector3 delta = pos - ToVector(pathhandeler);
        float dot = Vector3.Dot(delta, Normal(pathhandeler));

        return dot > 0;
    }

    public static MountainCoordinate FromPosition(BossPathHandler pahtHandeler, Vector3 pos) {
        
        Vector3 delta = pos - pahtHandeler.transform.position;
        delta.y = 0;
        float angle = Vector3.Angle(delta, Vector3.right);
        if (pos.z < pahtHandeler.transform.position.z) {
            angle = 360 - angle;
        }
        return new MountainCoordinate() {
            yPos = pos.y,
            Angle = angle
        };
    }
}

///<summary>
/// This is a path containing the begin and end points
///</summary>
[System.Serializable]
public struct MountainPath {
    public MountainCoordinate begin;
    public MountainCoordinate end;
    public int steps;

    public MountainCoordinate[] generatePathPoints(BossPathHandler pathHandeler) {
        List<MountainCoordinate> coords = new List<MountainCoordinate>();
        for (int k = 0; k <= steps; k++) {
            coords.Add( MountainCoordinate.Lerp(begin, end, (float)k/(float)steps));
        }
        return coords.ToArray();

    }

    public MountainCoordinate getClosestMountainCoord(MountainCoordinate[] coords, Vector3 pos, BossPathHandler pathHandeler) {
        if (coords.Length == 0) return default(MountainCoordinate);

        for (int j = coords.Length - 1; j >= 0; j--) {
            if (coords[j].IsVisible(pathHandeler, pos)) {
                return coords[j];
            }
        }
        //boss is most likely inside the shape, so go towards closest point
        MountainCoordinate chosen = coords[coords.Length - 1];
        float closestDistance = Mathf.Infinity;
        for (int j = 0; j < coords.Length; j++) {
            float dist = (coords[j].ToVector(pathHandeler) - pos).magnitude;
            if (dist < closestDistance) {
                closestDistance = dist;
                chosen = coords[j];
            }
        }
        return chosen;
    }
    public void DrawGizmo(BossPathHandler pathHandeler) {
        Vector3 oldPos = begin.ToVector(pathHandeler);
        foreach(MountainCoordinate coord in generatePathPoints(pathHandeler)) {
            Debug.DrawLine(oldPos, coord.ToVector(pathHandeler), pathHandeler.DebugColor);
            oldPos = coord.ToVector(pathHandeler);

            Color col = coord.IsVisible(pathHandeler, pathHandeler.Boss.transform.position) ? Color.green : Color.red;
            Debug.DrawLine(coord.ToVector(pathHandeler), coord.ToVector(pathHandeler) + coord.Normal(pathHandeler) * 5f, col);
            Gizmos.DrawSphere(coord.ToVector(pathHandeler), .5f);
        }
    }
}



public class BossPathHandler : MonoBehaviour
{
    private Color debugColor = new Color(1,1,1,.5f);
    public Color DebugColor {
        get { return debugColor;}
    }

    [Range(4, 100)]
    [SerializeField]
    private int debugLines = 10;
    [SerializeField]
    private bool enableDebug = true;

    [SerializeField]
    private float topRadius = 20f;
    public float TopRadius {
        get { return topRadius;}
    }
    [SerializeField]
    private float bottomRadius = 50f;
    public float BottomRadius {
        get { return bottomRadius;}
    }

    [SerializeField]
    private float height = 100f;
    public float Height {
        get { return height;}
    }

    [Space]
    [Header("path")]

    [SerializeField]
    private MountainPath path;
    
    public MountainPath Path {
        get { return path;}
    }

    [SerializeField]
    private Boss boss;

    public Boss Boss {
        get { return boss;}
    }

    private void OnDrawGizmos() {
        if (!enableDebug) return;

        Gizmos.color = debugColor;
        DebugExtensions.DrawCircle(transform.position, bottomRadius, debugColor, 360, 20);
        DebugExtensions.DrawCircle(transform.position + new Vector3(0,height, 0), topRadius, debugColor, 360, 20);
        for (int j = 0; j < 360; j+= (360 / debugLines)) {
            MountainCoordinate coords = new MountainCoordinate() {Angle = j, yPos = transform.position.y};
            MountainCoordinate coordsEnd = new MountainCoordinate() {Angle = j, yPos = transform.position.y + height};
            Debug.DrawLine(coords.ToVector(this), coordsEnd.ToVector(this), debugColor);
        }

        path.begin = MountainCoordinate.FromPosition(this, boss.transform.position);
        // path.DrawGizmo(this);
    }

    


}

}