using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaringState : IState
{

    public static float DUCK_DETECTION_DISTANCE = 10f;
    public Transform _transform;
    public Transform _player;
    public ILiveStateDelegate OnStateSwitch { get; set; }
    
    public float RotationSpeed = 10f;
 
    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;    
    
    public SwimState swimState;
    public void Exit()
    {
    }

    public void Run()
    {
        LookAtPlayer();
    }

    private bool PlayerIsClose() {
        if (_player != null) {
            if (Vector3.Distance(_player.position, _transform.position) < StaringState.DUCK_DETECTION_DISTANCE) {
                return true;
            }
        }
        return false;
    }

    private void LookAtPlayer() {
        if (!PlayerIsClose()) {
            OnStateSwitch(swimState);
            return;
        }

        //find the vector pointing from our position to the target
         _direction = (_player.position - _transform.position).normalized;
 
         //create the rotation we need to be in to look at the target
         _lookRotation = Quaternion.LookRotation(_direction);
 
         //rotate us over time according to speed until we are in the required rotation
         _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);    
    }

    public void Start()
    {
    }
}
public class SwimState : IState
{
    public StaringState staringState;

    private Vector3 velocity;
    public Transform _transform;
    public DuckSwimArea _swimArea;
    public Transform _player;
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public float Mass = 35;
    public float MaxVelocity = 3;
    public float MinVelocity = 1;
    public float MaxForce = 15;

    public void Exit()
    {
    }

    public void Run()
    {
        UpdateSteeringBehaviour();
    }

    private bool PlayerIsClose() {
        if (_player != null) {
            if (Vector3.Distance(_player.position, _transform.position) < StaringState.DUCK_DETECTION_DISTANCE) {
                return true;
            }
        }
        return false;
    }

    private void UpdateSteeringBehaviour() {
        if (PlayerIsClose()) {
            OnStateSwitch(staringState);
            return;
        }
        var desiredVelocity = _swimArea.Target - _transform.position;
        if (desiredVelocity.magnitude < .3f) {
            _swimArea.SetNewTarget();
            return;
        }

        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
        if (velocity.magnitude < MinVelocity) {
            velocity = velocity.normalized * MinVelocity;
        }
        _transform.position += velocity * Time.deltaTime;



        //find the vector pointing from our position to the target
         Vector3 _direction = (velocity).normalized;
 
         //create the rotation we need to be in to look at the target
         Quaternion _lookRotation = Quaternion.LookRotation(_direction);
 
         //rotate us over time according to speed until we are in the required rotation
         _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * 10f);    

        // _transform.forward = velocity.normalized;

        Debug.DrawRay(_transform.position, velocity.normalized * 2, Color.green);
        Debug.DrawRay(_transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }

    public void Start()
    {
        velocity = Vector3.zero;
        _swimArea.SetNewTarget();
    }
}
