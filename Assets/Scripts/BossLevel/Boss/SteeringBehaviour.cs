using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SteeringBehaviour
{
    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity {
        get { return velocity;}
        set { velocity = value; }
    }
    private Vector3 desiredVelocity = Vector3.zero;
    public Vector3 DesiredVelocity {
        get { return desiredVelocity;}
        set { desiredVelocity = value; }
    }

    private Vector3 steering;

    public Vector3 Steering {
        get { return steering;}
        set { steering = value; }
    }

    [Header("steering behavior")]
    [SerializeField]
    private float maxVelocity = .2f;
    public float MaxVelocity {
        get { return maxVelocity;}
    }

    [SerializeField]
    private float maxForce = 003f;
    public float MaxForce {
        get { return maxForce;}
        set { maxForce = value; }
    }

    [SerializeField]
    private float mass = 5f;
    public float Mass {
        get { return mass;}
    }
    [SerializeField]
    float slowingRadius = 5f;
    public float SlowingRadius {
        get { return slowingRadius;}
    }

    [SerializeField]
    float slowingAmplitude = .5f;
    public float SlowingAmplitude {
        get { return slowingAmplitude;}
    }

    ///<summary>
    /// The transform that will have the steering behaviour
    ///</summary>
    [Header("can be zero")]
    public Transform target;
    public Transform desiredTarget;

    public SteeringBehaviour(Transform _target, Transform _desiredTarget) {
        if (_target != null) target = _target;
        if (_desiredTarget != null) desiredTarget = _desiredTarget;
    }


    ///<summary>
    /// Updates the position based on the sterring behaviour of the target transform
    ///</summary>
    public void UpdatePosition() {
        desiredVelocity = (desiredTarget.position - target.position);

        
        // Check the distance to detect whether the character
        float distance = desiredVelocity.magnitude;
        // is inside the slowing area
        if (distance < slowingRadius) {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * maxVelocity * ((distance / slowingRadius) * slowingAmplitude);
        } else {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * maxVelocity;
        }

        steering = desiredVelocity - velocity;
        if (steering.magnitude > maxForce) steering = steering.normalized * maxForce;
        steering /= mass;

        velocity += steering;

        target.position += truncate(velocity);
    }

    private Vector3 truncate(Vector3 vector) {
        float distance = (target.position - desiredTarget.position).magnitude;
        // if (distance < vector.magnitude) {
        //     vector = vector.normalized * distance;
        // }
        return vector;
    }

    ///<summary>
    /// Resets the velocity
    ///</summary>
    public void Reset() {
        velocity = Vector3.zero;
    }

}
