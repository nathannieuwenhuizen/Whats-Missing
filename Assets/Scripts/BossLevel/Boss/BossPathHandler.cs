using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MountainCoordinate {
    public float angle;
    public float yPos;
    public Vector3 ToVector(BossPathHandler _pathHandeler) {
        Vector3 result = Vector3.zero;

        float precentage = Mathf.Clamp(yPos - _pathHandeler.transform.position.y, 0, _pathHandeler.Height) / _pathHandeler.Height;
        float resultRadius = _pathHandeler.BottomRadius + (_pathHandeler.TopRadius - _pathHandeler.BottomRadius) * precentage;

        result.y = yPos;
        result.x = _pathHandeler.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * resultRadius;
        result.z = _pathHandeler.transform.position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * resultRadius;
        return result;
    }

    public static MountainCoordinate Lerp(MountainCoordinate begin, MountainCoordinate end, float index) {
        return new MountainCoordinate() {
            angle = Mathf.Lerp(begin.angle, end.angle, index),
            yPos = Mathf.Lerp(begin.yPos,end.yPos, index)
        };
    }
    public Vector3 Normal(BossPathHandler _pathHandler) {
        Vector3 result= Vector3.zero;
        Vector3 delta = ToVector(_pathHandler) - _pathHandler.transform.position;
        result.x = delta.x;
        result.z = delta.z;
        result = result.normalized;
        float angle = 90 - Mathf.Atan2( _pathHandler.Height, (_pathHandler.BottomRadius - _pathHandler.TopRadius )) * Mathf.Rad2Deg;
        result.y = Mathf.Sin( angle * Mathf.Deg2Rad);
        return result.normalized;
    }
    
    public bool IsVisible(BossPathHandler pathhandeler, Vector3 pos) {
        Vector3 delta = pos - ToVector(pathhandeler);
        float dot = Vector3.Dot(delta, Normal(pathhandeler));

        return dot > 0;
    }
}


[System.Serializable]
public struct MountainPath {
    public MountainCoordinate begin;
    public MountainCoordinate end;
    public int steps;

    public MountainCoordinate[] generatePathPoints(BossPathHandler pathHandeler) {
        List<MountainCoordinate> coords = new List<MountainCoordinate>();
        for (int k = 0; k < steps; k++) {
            coords.Add( MountainCoordinate.Lerp(begin, end, (float)k/(float)steps));
        }
        return coords.ToArray();

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

    [SerializeField]
    private Boss boss;

    public Boss Boss {
        get { return boss;}
    }

    private void OnDrawGizmos() {
        Gizmos.color = debugColor;
        DebugExtensions.DrawCircle(transform.position, bottomRadius, debugColor, 360, 20);
        DebugExtensions.DrawCircle(transform.position + new Vector3(0,height, 0), topRadius, debugColor, 360, 20);
        for (int j = 0; j < 360; j+= 90) {
            MountainCoordinate coords = new MountainCoordinate() {angle = j, yPos = transform.position.y};
            MountainCoordinate coordsEnd = new MountainCoordinate() {angle = j, yPos = transform.position.y + height};
            Debug.DrawLine(coords.ToVector(this), coordsEnd.ToVector(this), debugColor);
        }

        path.DrawGizmo(this);
    }

    


}
