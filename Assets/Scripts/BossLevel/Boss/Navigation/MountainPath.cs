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

        private MountainCoordinate[] coords;
        public MountainCoordinate[] Coords {
            get { 
                if (coords == null) coords = new MountainCoordinate[]{ begin, end};
                return coords;
            }
        }


        public MountainCoordinate[] generatePathPoints(BossPathHandler pathHandeler) {
            List<MountainCoordinate> tempList = new List<MountainCoordinate>();
            for (int k = 0; k <= steps; k++) {
                tempList.Add( MountainCoordinate.Lerp(begin, end, (float)k/(float)steps));
            }
            coords = tempList.ToArray();
            return coords;

        }
        public Vector3 GetPathDirection(MountainCoordinate coord, BossPathHandler pathHandler) {
            Vector3 result = Vector3.forward;
            int index = coord.getIndexFromArray(coords);
            if (index != -1) {
                if (index < coords.Length - 1) {
                    result = coords[index + 1].ToVector(pathHandler) - coords[index].ToVector(pathHandler);
                } else if (index > 0) {
                    result = coords[index].ToVector(pathHandler) - coords[index - 1].ToVector(pathHandler);
                }
            }
            return result.normalized;
        }

        public MountainCoordinate GetClosestMountainCoord(Vector3 _pos, BossPathHandler _pathHandeler) {
            if (coords.Length == 0) return default(MountainCoordinate);

            if (_pathHandeler.OnSurface == false) {
                for (int j = coords.Length - 1; j >= 0; j--) 
                    if (coords[j].NormalIsVisible(_pathHandeler, _pos)) 
                        return coords[j];
            } else {
                int index = GetClosestCoordOnDistance(_pathHandeler, _pos).getIndexFromArray(coords);
                return coords[Mathf.Clamp(index + 1, 0, coords.Length - 1)];
            }


            //boss is most likely inside the shape, so go towards closest point
            return GetClosestCoordOnDistance(_pathHandeler, _pos);
        }

        public MountainCoordinate GetClosestCoordOnDistance(BossPathHandler _pathHandeler, Vector3 _pos) {
            MountainCoordinate chosen = coords[coords.Length - 1];
            float closestDistance = Mathf.Infinity;
            for (int j = 0; j < coords.Length; j++) {
                float dist = (coords[j].ToPrimitiveVector(_pathHandeler) - _pos).magnitude;
                if (dist < closestDistance) {
                    closestDistance = dist;
                    chosen = coords[j];
                }
            }
            return chosen;
        }

        public void DrawGizmo(BossPathHandler pathHandeler) {
            Vector3 oldPos = begin.ToVector(pathHandeler);
            generatePathPoints(pathHandeler);
            foreach(MountainCoordinate coord in coords) {
                // Debug.DrawLine(oldPos, coord.ToVector(pathHandeler), pathHandeler.DebugColor);
                // oldPos = coord.ToVector(pathHandeler);

                Color normalColor = normalColor = coord.NormalIsVisible(pathHandeler, pathHandeler.Boss.transform.position) ? Color.green : Color.red;
                Color directionColor = coord.DirectionIsVisible(pathHandeler, pathHandeler.Boss.transform.position, this) ? Color.green : Color.red;
                 

                Debug.DrawLine(coord.ToVector(pathHandeler), coord.ToVector(pathHandeler) + coord.Normal(pathHandeler) * 5f, normalColor);
                Debug.DrawLine(coord.ToVector(pathHandeler), coord.ToVector(pathHandeler) + GetPathDirection(coord, pathHandeler) * 5f, directionColor);
                Gizmos.DrawSphere(coord.ToVector(pathHandeler), .5f);
            }
        }
    }
}