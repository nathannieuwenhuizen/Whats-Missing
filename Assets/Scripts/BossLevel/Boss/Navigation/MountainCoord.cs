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


    private float pathOffset;
    public float PathOffset {
        get { return pathOffset;}
        set { pathOffset = value; }
    } 


    public Vector3 ToShapeVector(BossMountain _mountain, float offset = 0) {
        Vector3 result = Vector3.zero;
        result = ToPrimitiveVector(_mountain);
        if (offset != 0 || pathOffset != 0) result += Normal(_mountain) * (offset+ pathOffset);
        return result;
    }
    public Vector3 ToPrimitiveVector(BossMountain _mountain) {
        Vector3 result = Vector3.zero;

        float precentage = Mathf.Clamp(yPos - _mountain.transform.position.y, 0, _mountain.ShapeData.Height) / _mountain.ShapeData.Height;
        float resultRadius = _mountain.ShapeData.BottomRadius + (_mountain.ShapeData.TopRadius - _mountain.ShapeData.BottomRadius) * precentage;

        result.y = yPos;
        result.x = _mountain.transform.position.x + Mathf.Cos(Angle * Mathf.Deg2Rad) * resultRadius;
        result.z = _mountain.transform.position.z + Mathf.Sin(Angle * Mathf.Deg2Rad) * resultRadius;
        return result;
    }
    
    public Vector3 ToVector(BossMountain _mountain, float offset = 0) {
        if (_mountain.OnSurface) return CalculateRayCastedVector(_mountain);
        return ToShapeVector(_mountain, offset);
    }
    public Vector3 ToPathVector(BossMountain _mountain, MountainPath _path, float offset) {
        Vector3 result = Vector3.zero;
        result = ToPrimitiveVector(_mountain);
        if (offset != 0 || pathOffset != 0) result += PathNormal(_mountain, _path) * (offset + pathOffset);
        return result;

    }

    private Vector3 raycastedNormal;
    ///<summary>
    /// Returns the vector of the path that sticks at the mesh that is inside the shape
    ///</summary>
    public Vector3 CalculateRayCastedVector(BossMountain _mountain) {
        Vector3 result = ToPrimitiveVector(_mountain);
        RaycastHit hit;
        if (Physics.SphereCast(result, Boss.BOSS_SIZE * .9f, -PrimitiveNormal(_mountain), out hit, _mountain.ShapeData.BottomRadius)) {
            result = hit.point + PrimitiveNormal(_mountain) * Boss.BOSS_SIZE;
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

    ///<summary>
    /// Returns the lerp of the mountain coordinate
    ///</summary>
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


    #region  Normals---------------------
    ///<summary>
    /// Retunrs hte primitive normal
    ///</summary>
    public Vector3 Normal(BossMountain _mountain) {
        if (_mountain.OnSurface) {
            CalculateRayCastedVector(_mountain);
            return raycastedNormal;
        }
        return PrimitiveNormal(_mountain);
    }
    
    ///<summary>
    /// Returns the normal of the path.
    ///</summary>
    public  Vector3 PathNormal(BossMountain _mountain, MountainPath _path) {
        Vector3 result= Vector3.zero;
        Vector3 a = _path.GetPathDirection(this, _mountain);
        // Vector3 b = _path.begin.ToVector(_pathHandler);
        Vector3 b = Vector3.up;
        result = Vector3.Cross(a, b).normalized;
        result.y = PrimitiveNormal(_mountain).y;

        Vector3 delta = ToVector(_mountain);

        bool dot = Vector3.Dot(delta, result) > 0;
        return result.normalized * (dot ? 1 : -1);
    }

    ///<summary>
    /// Returns the primitive normal of the basic mountain shape
    ///</summary>
    private Vector3 PrimitiveNormal(BossMountain _mountain) {
        Vector3 result= Vector3.zero;
        Vector3 delta = ToPrimitiveVector(_mountain) - _mountain.transform.position;
        result.x = delta.x;
        result.z = delta.z;
        result = result.normalized;
        float _angle = 90 - Mathf.Atan2( _mountain.ShapeData.Height, (_mountain.ShapeData.BottomRadius - _mountain.ShapeData.TopRadius )) * Mathf.Rad2Deg;
        result.y = Mathf.Sin( _angle * Mathf.Deg2Rad);

        return result.normalized;
    }
    
    public bool NormalIsVisible(BossMountain _mountain, Vector3 _viewPos) {
        Vector3 delta = _viewPos - ToVector(_mountain);
        float dot = Vector3.Dot(delta, Normal(_mountain));
        return dot > 0;
    }

    public bool PathNormalIsVisible(BossMountain _mountain, MountainPath _path, Vector3 _viewPos) {
        Vector3 delta = _viewPos - ToVector(_mountain);
        float dot = Vector3.Dot(delta, PathNormal(_mountain, _path));
        return dot > 0;
    }

    #endregion

    public bool DirectionIsVisible(BossMountain _mountain, Vector3 _viewPos, MountainPath _path) {
        Vector3 delta = _viewPos - ToVector(_mountain);
        float dot = Vector3.Dot(delta, _path.GetPathDirection(this, _mountain));
        return dot > 0;
    }


    public int getIndexFromArray(MountainCoordinate[] coords) {
        for (int i = 0; i < coords.Length; i++)
            if (coords[i].angle == angle && coords[i].yPos == yPos) 
                return i;
        return -1;
    }

    public static MountainCoordinate FromPosition(BossMountain _mountain, Vector3 _pos) {
        
        Vector3 delta = _pos - _mountain.transform.position;
        delta.y = 0;
        float angle = Vector3.Angle(delta, Vector3.right);
        if (_pos.z < _mountain.transform.position.z) {
            angle = 360 - angle;
        }
        return new MountainCoordinate() {
            yPos = _pos.y,
            Angle = angle
        };
    }
}

}