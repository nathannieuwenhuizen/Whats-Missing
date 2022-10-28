using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    public class FreeFloatBehaviour : IMovementBehavior
    {
        public Transform desiredPos { get; set; }
        public bool MovementEnabled { get; set; } = true;
        public bool WithPathOffset { get; set; } = true;


        private float speedScale = 1f;

        public float SpeedScale { 
            get => SpeedScale;
        set {
                speedScale = value;
            } 
        }
        private Transform transform;
        private Transform desiredTempPos;
        private BossPositioner bossPositioner;
        public float BasePathOffset { get; set; } = 5f;

        public Vector3 Velocity { 
            get => bossPositioner.SteeringBehaviour.Velocity;
            set => bossPositioner.SteeringBehaviour.Velocity = value;
        }



        public FreeFloatBehaviour(Transform _transform, Transform _desiredTempPos, BossPositioner _bossPositioner) {
            bossPositioner = _bossPositioner;
            transform = _transform;
            desiredTempPos = _desiredTempPos;
            desiredTempPos.SetParent(transform.parent);
        }

        public Vector3 GetClosestPointOnPath()
        {
            return transform.position;
        }

        public Vector3 GetClosestPointOnPath(Vector3 _position)
        {
            return transform.position;
        }

        public float GetPathLength()
        {
            return Vector3.Distance(bossPositioner.SteeringBehaviour.target.position, bossPositioner.SteeringBehaviour.desiredTarget.position);
        }

        public bool ReachedDestination(float _distanceThreshhold)
        {
            if (Vector3.Distance(transform.position, desiredPos.position) > _distanceThreshhold) return false;
            if (bossPositioner.SteeringBehaviour.Velocity.magnitude > 1f) return false;
            return true;
        }

        public void SetDestinationPath(Vector3 _end, Vector3 _begin = default)
        {
            desiredPos.position = _end;
            UpdateTempDestination();
        }

        public void SetDestinationPath(Transform _target, Vector3 _begin = default)
        {
            desiredPos = _target;
            SetDestinationPath(desiredPos.position, _begin);
        }

        public void UpdateTempDestination()
        {
            desiredTempPos.position = desiredPos.position;
        }

        public void Update()
        {
            if (MovementEnabled) {
                UpdateTempDestination();
                bossPositioner.SteeringBehaviour.UpdatePosition(speedScale);
                // UpdateRotation();
            }    
        }
        public void DrawGizmo()
        {
            Debug.DrawLine(transform.position, bossPositioner.SteeringBehaviour.desiredTarget.position, Color.yellow);
        }

        public Quaternion PathRotation()
        {
            Vector3 pointDirection = bossPositioner.SteeringBehaviour.Velocity;
            pointDirection.y = 0;
            if (pointDirection.magnitude < .1f) {
                return transform.rotation;
            }
            return Quaternion.LookRotation(pointDirection, Vector3.up);
        }
    }
}
