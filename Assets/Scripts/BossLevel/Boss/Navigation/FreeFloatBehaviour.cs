using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    public class FreeFloatBehaviour : IMovementBehavior
    {
        public Transform desiredPos { get; set; }
        public bool MovementEnabled { get; set; } = true;
        public BodyOrientation bodyOrientation {get; set;} = BodyOrientation.toShape;

        private float speedScale = 1f;

        public float SpeedScale { 
            get => SpeedScale;
            set {
                speedScale = value;
            } 
        }
        private Transform transform;
        private Transform desiredTempPos;
        private SteeringBehaviour steeringBehaviour;

        public FreeFloatBehaviour(Transform _transform, Transform _desiredTempPos, SteeringBehaviour _steeringBehaviour) {
            transform = _transform;
            desiredTempPos = _desiredTempPos;
            steeringBehaviour = _steeringBehaviour;

            steeringBehaviour.target = transform;
            steeringBehaviour.desiredTarget = desiredTempPos;
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
            return Vector3.Distance(steeringBehaviour.target.position, steeringBehaviour.desiredTarget.position);
        }

        public bool ReachedDestination(float _distanceThreshhold)
        {
            if (Vector3.Distance(transform.position, desiredTempPos.position) > _distanceThreshhold) return false;
            if (steeringBehaviour.Velocity.magnitude > 1f) return false;
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

        public void UpdateRotation()
        {
            Vector3 pointDirection = steeringBehaviour.Velocity;
            pointDirection.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pointDirection, Vector3.up), Time.deltaTime);
        }

        public void UpdateTempDestination()
        {
            desiredTempPos.position = desiredPos.position;
        }

        public void Update()
        {
            if (MovementEnabled) {
                UpdateTempDestination();
                steeringBehaviour.UpdatePosition(speedScale);
                UpdateRotation();
            }    
        }
        public void DrawGizmo()
        {
            Debug.DrawLine(transform.position, steeringBehaviour.desiredTarget.position, Color.yellow);
        }
    }
}
