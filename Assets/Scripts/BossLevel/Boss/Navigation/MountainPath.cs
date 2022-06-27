using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Boss {
    ///<summary>
    /// This is a path containing the begin and end points
    ///</summary>
    [System.Serializable]
    public struct MountainPath {
        public MountainCoordinate begin;
        public MountainCoordinate end;
        public int steps;
        public float basePathOffset;

        private bool hasPathOffset;

        private MountainCoordinate[] coords;
        public MountainCoordinate[] Coords {
            get { 
                if (coords == null) coords = new MountainCoordinate[]{};
                return coords;
            }
        }


        public MountainCoordinate[] generatePathPoints(BossMountain _mountain, bool _hasPathOffset = true, float _basePathOffset = 5f) {
            basePathOffset = _basePathOffset;
            hasPathOffset = _hasPathOffset;
            
            List<MountainCoordinate> tempList = new List<MountainCoordinate>();
            // steps = Mathf.RoundToInt(Mathf.Clamp(Mathf.Abs(begin.angle - end.angle) / 10f, 2f, 36f));
            steps = 20;
            for (int k = 0; k <= steps; k++) {
                MountainCoordinate coord = MountainCoordinate.Lerp(begin, end, (float)k/(float)steps);
                if (_hasPathOffset == false) coord.PathOffset = 0;
                tempList.Add( coord);
            }
            coords = tempList.ToArray();

            // begin.PathOffset = coords[0].PathOffset;
            // end.PathOffset = coords[coords.Length - 1].PathOffset;

            return coords;

        }
        public Vector3 GetPathDirection(MountainCoordinate coord, BossMountain _mountain) {
            Vector3 result = Vector3.forward;
            int index = coord.getIndexFromArray(coords);
            if (index != -1) {
                if (index < coords.Length - 1) {
                    result = coords[index + 1].ToVector(_mountain) - coords[index].ToVector(_mountain);
                } else if (index > 0) {
                    result = coords[index].ToVector(_mountain) - coords[index - 1].ToVector(_mountain);
                }
            }
            return result.normalized;
        }

        public MountainCoordinate GetClosestMountainCoord(Vector3 _pos, BossMountain _mountain) {
            if (Coords.Length == 0) return default(MountainCoordinate);

            if (_mountain.OnSurface == false) {
                // for (int j = coords.Length - 1; j >= 0; j--) 
                //     if (coords[j].NormalIsVisible(_pathHandeler, _pos)) 
                //         return coords[j];
                //begin with the start pos;
                MountainCoordinate coord = coords[0];
                for (int j = 0; j < coords.Length; j++) 
                //check if the normal is visible (path normal if it has a path offset)
                    if (coords[j].PathNormalIsVisible(_mountain, this, _pos) && hasPathOffset) coord = coords[j];
                    else if (coords[j].NormalIsVisible(_mountain, _pos) && !hasPathOffset) coord = coords[j];

                    return coord;
            } else {
                int index = GetClosestCoordOnDistance(_mountain, _pos).getIndexFromArray(coords);
                return coords[Mathf.Clamp(index + 1, 0, coords.Length - 1)];
            }
        }

        public MountainCoordinate GetClosestCoordOnDistance(BossMountain _mountain, Vector3 _pos) {
            MountainCoordinate chosen = coords[coords.Length - 1];
            float closestDistance = Mathf.Infinity;
            for (int j = 0; j < coords.Length; j++) {
                float dist = (coords[j].ToShapeVector(_mountain) - _pos).magnitude;
                if (dist < closestDistance) {
                    closestDistance = dist;
                    chosen = coords[j];
                }
            }
            return chosen;
        }

        public void DrawGizmo(BossMountain _mountain) {
            Vector3 oldPos = begin.ToVector(_mountain);
            generatePathPoints(_mountain);
            foreach(MountainCoordinate coord in coords) {
                // Debug.DrawLine(oldPos, coord.ToVector(pathHandeler), pathHandeler.DebugColor);
                // oldPos = coord.ToVector(pathHandeler);

                Color normalColor = normalColor = coord.PathNormalIsVisible(_mountain, this, _mountain.Boss.transform.position) ? Color.green : Color.red;
                Color directionColor = coord.DirectionIsVisible(_mountain, _mountain.Boss.transform.position, this) ? Color.green : Color.red;
                 

                // Debug.DrawLine(coord.ToVector(pathHandeler), coord.ToVector(pathHandeler) + coord.Normal(pathHandeler) * offset, normalColor);
                Debug.DrawLine(coord.ToVector(_mountain), coord.ToVector(_mountain) + coord.PathNormal(_mountain, this) * 5f, normalColor);
                Debug.DrawLine(coord.ToVector(_mountain), coord.ToVector(_mountain) + coord.Normal(_mountain) * basePathOffset, normalColor);
                Debug.DrawLine(coord.ToVector(_mountain), coord.ToVector(_mountain) + GetPathDirection(coord, _mountain) * 5f, directionColor);
                Gizmos.DrawSphere(coord.ToVector(_mountain), .5f);
            }
        }
    }
}