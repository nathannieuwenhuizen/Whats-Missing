using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaringState : IState
{

    public static float DUCK_DETECTION_DISTANCE = 15f;
    public Duck _duck;
    public Transform _player;
    public ILiveStateDelegate OnStateSwitch { get; set; }
    
    public float RotationSpeed = 10f;
 
    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;    
    
    public SwimState swimState;
    public void Exit()
    {
        _duck.Quack();
    }

    public void Run()
    {
        LookAtPlayer();
    }

    private bool PlayerIsClose() {
        if (_player != null) {
            if (Vector3.Distance(_player.position, _duck.transform.position) < StaringState.DUCK_DETECTION_DISTANCE) {
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
         _direction = (_player.position - _duck.transform.position).normalized;
         _direction.y = 0;
 
         //create the rotation we need to be in to look at the target
         _lookRotation = Quaternion.LookRotation(_direction);
 
         //rotate us over time according to speed until we are in the required rotation
         _duck.transform.rotation = Quaternion.Slerp(_duck.transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);    
    }

    public void Start()
    {
        _duck.Quack();
    }
}

public class SwimState : IState
{
    public StaringState staringState;

    public Duck _duck;
    public Transform _player;
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public float Mass = 35;
    public float MaxVelocity = 3;
    public float MinVelocity = 1;
    public float MaxForce = 15;

    private Coroutine quackCoroutine;
    
    public void Start()
    {
        _duck.Velocity = Vector3.zero;
        _duck.SwimArea.SetNewTarget();
        quackCoroutine = _duck.StartCoroutine(DoRandomQuacks());
    }

    
    public void Exit()
    {
        if (quackCoroutine != null) {
            _duck.StopCoroutine(quackCoroutine);
        }
    }

    public void Run()
    {
        UpdateSteeringBehaviour();
    }

    private bool PlayerIsClose() {
        if (_player != null) {
            if (Vector3.Distance(_player.position, _duck.transform.position) < StaringState.DUCK_DETECTION_DISTANCE) {
                return true;
            }
        }
        return false;
    }
    private IEnumerator DoRandomQuacks() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2, 5));
            _duck.Quack();
        }
    }

    private void UpdateSteeringBehaviour() {
        if (PlayerIsClose()) {
            OnStateSwitch(staringState);
            return;
        }
        var desiredVelocity = _duck.SwimArea.Target - _duck.transform.position;
        if (desiredVelocity.magnitude < .3f) {
            _duck.SwimArea.SetNewTarget();
            return;
        }

        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        var steering = desiredVelocity - _duck.Velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce * Room.TimeScale);
        steering /= (Mass / Room.TimeScale);

        _duck.Velocity = Vector3.ClampMagnitude(_duck.Velocity + steering, MaxVelocity);
        if (_duck.Velocity.magnitude < MinVelocity) {
            _duck.Velocity = _duck.Velocity.normalized * MinVelocity;
        }
        _duck.transform.position += _duck.Velocity * Time.deltaTime * Room.TimeScale;



        //find the vector pointing from our position to the target
         Vector3 _direction = (_duck.Velocity).normalized;
 
         //create the rotation we need to be in to look at the target
         Quaternion _lookRotation = Quaternion.LookRotation(_direction);
 
         //rotate us over time according to speed until we are in the required rotation
         _duck.transform.rotation = Quaternion.Slerp(_duck.transform.rotation, _lookRotation, Time.deltaTime * Room.TimeScale * 10f);    

        // _transform.forward = _duck.Velocity.normalized;

        Debug.DrawRay(_duck.transform.position, _duck.Velocity.normalized * 2, Color.green);
        Debug.DrawRay(_duck.transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }
}

public class FollowState : IState
{

    public List<Duck> _otherDucks;
    public Duck _duck;
    public ILiveStateDelegate OnStateSwitch { get; set; }
    
    public float RotationSpeed = 10f;
 
    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;    
    private Vector3 desiredVelocity;

    public float Mass = 5;
    public float MaxVelocity = 5;
    public float MinVelocity = 1;
    public float MaxForce = 1;

    public float cohesion = 1f;
    public float align = .5f;
    public float seperation = 2f;
    public float avoidEdge = .5f;
    public float attractor = 1f;
    public float swimSpeed = 2f;
    

    private float detectionRadius = 4f;
    private Coroutine quackCoroutine;

    public void Exit()
    {
    }

    public void Run()
    {
        desiredVelocity = Flock();

        var steering = desiredVelocity - _duck.Velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mathf.Max(1,  Mass / Room.TimeScale);

        _duck.Velocity = Vector3.ClampMagnitude(_duck.Velocity + steering, MaxVelocity);
        if (_duck.Velocity.magnitude < MinVelocity) {
            _duck.Velocity = _duck.Velocity.normalized * MinVelocity;
        }
        _duck.transform.position += _duck.Velocity * Time.deltaTime * Room.TimeScale * swimSpeed;



        //find the vector pointing from our position to the target
         Vector3 _direction = (_duck.Velocity).normalized;
 
         //create the rotation we need to be in to look at the target
         Quaternion _lookRotation = Quaternion.LookRotation(_direction);
 
         //rotate us over time according to speed until we are in the required rotation
         _duck.transform.rotation = Quaternion.Slerp(_duck.transform.rotation, _lookRotation, Time.deltaTime * Room.TimeScale * RotationSpeed);    

        // _transform.forward = _duck.Velocity.normalized;

        Debug.DrawRay(_duck.transform.position, Seperation().normalized * 2, Color.green);
        // Debug.DrawRay(_duck.transform.position, Seperation().normalized * 2, Color.blue);
        // Debug.DrawRay(_duck.transform.position, Allign().normalized * 2, Color.red);
    }

    private IEnumerator DoRandomQuacks() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2, 10));
            _duck.Quack();
        }
    }


    public Vector3 Cohesion() {
        Vector3 result = Vector3.zero;
        for(int i = 0 ; i < _otherDucks.Count; i++) {
            result += _otherDucks[i].transform.position;
        }
        result /= _otherDucks.Count;
        result = (result - _duck.transform.position);
        return result.normalized * cohesion;
    }
    public Vector3 Allign() {
        Vector3 result = Vector3.zero;
        for(int i = 0 ; i < _otherDucks.Count; i++) {
            if (InRadius(_otherDucks[i]))
                result += _otherDucks[i].Velocity;
        }
        result /= _otherDucks.Count;
        return result.normalized * align;
    }

    private bool InRadius(Duck otherDuck) {
        return Vector3.Distance(otherDuck.transform.position, _duck.transform.position) < detectionRadius;
    }
    public Vector3 Seperation() {
        Vector3 result = Vector3.zero;
        for(int i = 0 ; i < _otherDucks.Count; i++) {
            if (InRadius(_otherDucks[i]))
                result += _otherDucks[i].transform.position - _duck.transform.position;
        }
        result *= -1;

        return result.normalized * seperation;
    }

    public Vector3 AvoidEdge() {
        Vector3 result = Vector3.zero;
        result += _duck.transform.position - _duck.SwimArea.transform.position;
        result *= -1;
        return result.normalized * avoidEdge;
    }
    public Vector3 Attractor() {
        Vector3 result = Vector3.zero;
        result += _duck.SwimArea.Target - _duck.transform.position;
        return result.normalized * attractor;
    }
    public Vector3 Flock() {
        Vector3 result = Vector3.zero;
        result += Cohesion();
        result += Allign();
        result += Seperation();
        result += AvoidEdge();
        result += Attractor();
        result.y = 0;
        return result;
    }

    public void Start()
    {
        quackCoroutine = _duck.StartCoroutine(DoRandomQuacks());
        Vector3 startVelocity = Extensions.RandomVector(1f);
        startVelocity.y = 0;
        _duck.Velocity = startVelocity;
    }
}
