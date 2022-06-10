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

    private float pathOffset;
    public float PathOffset {
        get { return pathOffset;}
        set { pathOffset = value; }
    } 
    ///<summary>
    /// The world Y position
    ///</summary>
    public float yPos;
    public Vector3 ToShapeVector(BossMountain _pathHandeler, float offset = 0) {
        Vector3 result = Vector3.zero;
        result = ToPrimitiveVector(_pathHandeler);
        if (offset != 0 || pathOffset != 0) result += Normal(_pathHandeler) * (offset+ pathOffset);
        return result;
    }
    public Vector3 ToPrimitiveVector(BossMountain _pathHandeler) {
        Vector3 result = Vector3.zero;

        float precentage = Mathf.Clamp(yPos - _pathHandeler.transform.position.y, 0, _pathHandeler.Height) / _pathHandeler.Height;
        float resultRadius = _pathHandeler.BottomRadius + (_pathHandeler.TopRadius - _pathHandeler.BottomRadius) * precentage;

        result.y = yPos;
        result.x = _pathHandeler.transform.position.x + Mathf.Cos(Angle * Mathf.Deg2Rad) * resultRadius;
        result.z = _pathHandeler.transform.position.z + Mathf.Sin(Angle * Mathf.Deg2Rad) * resultRadius;
        return result;
    }
    
    public Vector3 ToVector(BossMountain _pathHandeler, float offset = 0) {
        if (_pathHandeler.OnSurface) return CalculateRayCastedVector(_pathHandeler);
        return ToShapeVector(_pathHandeler, offset);
    }

    private Vector3 raycastedNormal;
    ///<summary>
    /// Returns the vector of the path that sticks at the mesh that is inside the shape
    ///</summary>
    public Vector3 CalculateRayCastedVector(BossMountain _pathHandeler) {
        Vector3 result = ToPrimitiveVector(_pathHandeler);
        RaycastHit hit;
        if (Physics.SphereCast(result, Boss.BOSS_SIZE * .9f, -PrimitiveNormal(_pathHandeler), out hit, _pathHandeler.BottomRadius)) {
            result = hit.point + PrimitiveNormal(_pathHandeler) * Boss.BOSS_SIZE;
            raycastedNormal = hit.normal;
            float size = 3.5f;
            if (Physics.SphereCast(result, Boss.BOSS_SIZE * .9f, Vector3.down, out hit, Boss.BOSS_SIZE * size)) {
                // result = hit.point + Vector3.up * bossSize;
                float precentage = ((Vector3.Distance(hit.point, result)) / Boss.BOSS_SIZE) / size - .5f / (1f / .7f);
                // Debug.Log("precentage " + precentage);
                raycastedNormal = Vector3.Lerp(raycastedNormal, hit.normal, 1 - precentage);
                // raycastedNormal = hit.normal;
            }
        }
        return result;
    }

    public static MountainCoordinate Lerp(MountainCoordinate begin, MountainCoordinate end, float index) {
        float _angle = Mathf.Lerp(begin.Angle, end.Angle, index);
        float _pathOffset = 0;
        if (index > 0 && index < 1 && Mathf.Abs(begin.angle - end.angle) > 5f) {
            _pathOffset = Mathf.Sin(index * Mathf.PI) * 10f;
        }
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
            yPos = Mathf.Lerp(begin.yPos,end.yPos, index),
            pathOffset = _pathOffset
        };
    }

    public Vector3 Normal(BossMountain _pathHandler) {
        if (_pathHandler.OnSurface) {
            CalculateRayCastedVector(_pathHandler);
            return raycastedNormal;
        }
        return PrimitiveNormal(_pathHandler);
    }
    
    public  Vector3 PathNormal(BossMountain _pathHandler, MountainPath _path) {
        Vector3 result= Vector3.zero;
        Vector3 a = _path.GetPathDirection(this, _pathHandler);
        // Vector3 b = _path.begin.ToVector(_pathHandler);
        Vector3 b = Vector3.up;
        result = Vector3.Cross(a, b).normalized;
        result.y = PrimitiveNormal(_pathHandler).y;

        Vector3 delta = ToVector(_pathHandler);

        bool dot = Vector3.Dot(delta, result) > 0;
        return result.normalized * (dot ? 1 : -1);
}
    private Vector3 PrimitiveNormal(BossMountain _pathHandler) {
        Vector3 result= Vector3.zero;
        Vector3 delta = ToPrimitiveVector(_pathHandler) - _pathHandler.transform.position;
        result.x = delta.x;
        result.z = delta.z;
        result = result.normalized;
        float _angle = 90 - Mathf.Atan2( _pathHandler.Height, (_pathHandler.BottomRadius - _pathHandler.TopRadius )) * Mathf.Rad2Deg;
        result.y = Mathf.Sin( _angle * Mathf.Deg2Rad);

        return result.normalized;
    }
    
    public bool NormalIsVisible(BossMountain _pathhandeler, Vector3 _viewPos) {
        Vector3 delta = _viewPos - ToVector(_pathhandeler);
        float dot = Vector3.Dot(delta, Normal(_pathhandeler));
        return dot > 0;
    }

    public bool PathNormalIsVisible(BossMountain _pathhandeler, MountainPath _path, Vector3 _viewPos) {
        Vector3 delta = _viewPos - ToVector(_pathhandeler);
        float dot = Vector3.Dot(delta, PathNormal(_pathhandeler, _path));
        return dot > 0;
    }

    public bool DirectionIsVisible(BossMountain _pathhandeler, Vector3 _viewPos, MountainPath _path) {
        
        Vector3 delta = _viewPos - ToVector(_pathhandeler);
        float dot = Vector3.Dot(delta, _path.GetPathDirection(this, _pathhandeler));
        return dot > 0;
    }


    public int getIndexFromArray(MountainCoordinate[] coords) {
        for (int i = 0; i < coords.Length; i++)
            if (coords[i].angle == angle && coords[i].yPos == yPos) 
                return i;
        return -1;
    }

    public static MountainCoordinate FromPosition(BossMountain _pathHandeler, Vector3 _pos) {
        
        Vector3 delta = _pos - _pathHandeler.transform.position;
        delta.y = 0;
        float angle = Vector3.Angle(delta, Vector3.right);
        if (_pos.z < _pathHandeler.transform.position.z) {
            angle = 360 - angle;
        }
        return new MountainCoordinate() {
            yPos = _pos.y,
            Angle = angle
        };
    }
}

}